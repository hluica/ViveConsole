using System.Diagnostics;
using System.Text;

using ViveConsole.Models;

namespace ViveConsole.Services.Backend;

public class VivetoolAdapter : IVivetoolAdapter
{
    public async Task<string> QuerySingleAsync(uint id)
        => await RunProcessAsync($"/query /id:{id}");

    public async Task ExecuteBatchAsync(List<InstructionRow> rows)
    {
        // 1. 定义核心副作用函数：执行命令并更新行状态
        // 使用本地函数（Local Function）封装重复逻辑
        static async Task RunAndUpdateAsync(string args, IEnumerable<InstructionRow> targetRows)
        {
            var output = await RunProcessAsync(args);

            var isError = output.Contains("Failed", StringComparison.OrdinalIgnoreCase) ||
                          output.Contains("Error", StringComparison.OrdinalIgnoreCase);

            var newStatus = isError ? RowStatus.Error : RowStatus.Configured;

            foreach (var row in targetRows)
            {
                row.OutputText = output;
                row.Status = newStatus;
            }
        }

        // 2. 声明式地构建任务列表 (Batch Plan)
        // 将 Reset 和 Enable 的处理逻辑统一抽象为 (Arguments, Rows) 的元组流

        // 2.1 准备 Reset 任务
        var resetTasks = rows
            .Where(r => r.Action == ActionType.Reset)
            .GroupBy(_ => 0) // 作为一个整体分组
            .Select(g => (
                Args: $"/reset /id:{string.Join(",", g.Select(r => r.Id))}",
                Rows: g as IEnumerable<InstructionRow>
            ));

        // 2.2 准备 Enable 任务
        var enableTasks = rows
            .Where(r => r.Action == ActionType.Enable)
            .GroupBy(r => r.Variant)
            .Select(g => (
                Args: $"/enable /id:{string.Join(",", g.Select(r => r.Id))}{(g.Key.HasValue ? $" /variant:{g.Key}" : "")}",
                Rows: g as IEnumerable<InstructionRow>
            ));

        // 3. 执行
        // 合并所有任务并执行
        var allTasks = resetTasks.Concat(enableTasks);

        foreach (var (Args, Rows) in allTasks)
        {
            await RunAndUpdateAsync(Args, Rows);
        }
    }

    private static async Task<string> RunProcessAsync(string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "vivetool.exe", // 假设已在 PATH 中
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = new Process { StartInfo = psi };
            var outputBuilder = new StringBuilder();

            process.OutputDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
            process.ErrorDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            // 清洗输出：移除前两行 (通常是版权信息)
            var lines = outputBuilder.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length >= 2)
            {
                return string.Join(Environment.NewLine, lines.Skip(1)).Trim();
            }
            return outputBuilder.ToString().Trim();
        }
        catch (Exception ex)
        {
            return $"EXECUTION ERROR: {ex.Message}";
        }
    }
}
