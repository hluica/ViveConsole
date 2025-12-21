using Spectre.Console;

using ViveConsole.Models;
using ViveConsole.Services;
using ViveConsole.Services.Backend;

namespace ViveConsole;

public class App(CommandParser parser, StateManager state, IVivetoolAdapter adapter, UserInterface ui)
{
    private readonly CommandParser _parser = parser;
    private readonly StateManager _state = state;
    private readonly IVivetoolAdapter _adapter = adapter;
    private readonly UserInterface _ui = ui;

    public async Task RunAsync()
    {
        bool running = true;

        // Ctrl+C 捕获
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            running = false;
        };

        while (running)
        {
            // 1. 渲染界面
            _ui.RenderScreen(_state.Rows);

            // 2. 读取输入 (手动模拟提示符)
            AnsiConsole.Markup("[bold lime]Command > [/]");
            var input = Console.ReadLine();

            if (input == null) break; // EOF
            var trimmedInput = input.Trim();

            // 3. 处理基础指令
            if (string.Equals(trimmedInput, "exit", StringComparison.OrdinalIgnoreCase))
                break;

            if (string.Equals(trimmedInput, "run", StringComparison.OrdinalIgnoreCase))
            {
                await RunPendingInstructionsAsync();
                continue;
            }

            if (string.Equals(trimmedInput, "clear", StringComparison.OrdinalIgnoreCase))
            {
                _state.Clear();
                continue; // 跳过后续逻辑，直接进入下一次循环，RenderScreen 会自动刷新为空白表格
            }

            // 4. 解析新指令
            var newRows = _parser.Parse(trimmedInput);
            if (newRows.Count > 0)
            {
                _state.Upsert(newRows);
                // 5. 立即执行查询 (UI 刷新在下次循环，或者可以强制在这里刷新)
                await CheckNewRowsStatusAsync(newRows);
            }
        }
    }

    private async Task CheckNewRowsStatusAsync(List<InstructionRow> newRows)
    {
        // 显示正在加载
        _ui.RenderScreen(_state.Rows);

        foreach (var row in newRows)
        {
            // 每一个都查询，更新 OutputText
            // 对于 Query 类型的指令，虽然它在 Run 时会 Skipped，但现在我们也要查一下它的当前状态
            try
            {
                row.OutputText = await _adapter.QuerySingleAsync(row.Id);
            }
            catch (Exception ex)
            {
                row.OutputText = $"Check Failed: {ex.Message}";
            }
        }
    }

    private async Task RunPendingInstructionsAsync()
    {
        var executableRows = _state.GetExecutableRows();

        if (executableRows.Count == 0)
            return;

        AnsiConsole.MarkupLine("[yellow]Running instructions...[/]");

        await _adapter.ExecuteBatchAsync(executableRows);

        AnsiConsole.MarkupLine("[green]Done! Press Enter to refresh table.[/]");
        Console.ReadLine();
    }
}
