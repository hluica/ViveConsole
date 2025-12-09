using System.Diagnostics;
using System.Text;
using ViveConsole.Models;

namespace ViveConsole.Services.Backend;

public class VivetoolAdapter : IVivetoolAdapter
{
    public async Task<string> QuerySingleAsync(uint id)
    {
        return await RunProcessAsync($"/query /id:{id}");
    }

    public async Task ExecuteBatchAsync(List<InstructionRow> rows)
    {
        // 1. 处理 Reset (可以全部合并)
        var resetRows = rows.Where(r => r.Action == ActionType.Reset).ToList();
        if (resetRows.Count != 0)
        {
            var ids = string.Join(",", resetRows.Select(r => r.Id));
            var output = await RunProcessAsync($"/reset /id:{ids}");

            // 更新所有相关行的状态
            var isError = output.Contains("Failed", StringComparison.OrdinalIgnoreCase) ||
                          output.Contains("Error", StringComparison.OrdinalIgnoreCase);

            foreach (var row in resetRows)
            {
                row.OutputText = output;
                row.Status = isError ? RowStatus.Error : RowStatus.Configured;
            }
        }

        // 2. 处理 Enable (需要按 Variant 分组)
        var enableRows = rows.Where(r => r.Action == ActionType.Enable).ToList();

        // 分组 Key: Variant (null 视为同一组)
        var groupedEnables = enableRows.GroupBy(r => r.Variant);

        foreach (var group in groupedEnables)
        {
            var currentVariant = group.Key;
            var ids = string.Join(",", group.Select(r => r.Id));

            var args = $"/enable /id:{ids}";
            if (currentVariant.HasValue)
            {
                args += $" /variant:{currentVariant.Value}";
            }

            var output = await RunProcessAsync(args);

            var isError = output.Contains("Failed", StringComparison.OrdinalIgnoreCase) ||
                          output.Contains("Error", StringComparison.OrdinalIgnoreCase);

            foreach (var row in group)
            {
                row.OutputText = output;
                row.Status = isError ? RowStatus.Error : RowStatus.Configured;
            }
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