using Spectre.Console;
using Spectre.Console.Rendering;
using ViveConsole.Models;
using ViveConsole.Utils;

namespace ViveConsole.Services;

public class UserInterface
{
    public void RenderScreen(IReadOnlyList<InstructionRow> rows)
    {
        // 0. 清屏 (副作用)
        AnsiConsole.Clear();

        // 链式调用的起点：创建表格结构
        CreateTableStructure()
            // 1. 填充数据 (根据 rows 是否为空走进不同分支，保持 Table 实例流动)
            .ApplyIf(rows.Count == 0,
                whenTrue: t => t.AddRow(new Markup(""), new Rule("[grey]Waiting for input...[/]").LeftJustified(), new Markup("")),
                whenFalse: t => t.AddRowsFrom(rows, MapToRenderableRow) // 使用自定义扩展消除 foreach
            )
            // 2. 渲染 Header (副作用，使用 Tap 穿透)
            .Tap(_ => AnsiConsole.Write(new Rule("[blue]ViVeTool Wrapper[/]").RuleStyle("grey").LeftJustified()))
            // 3. 渲染 Table 本身 (副作用)
            .Tap(AnsiConsole.Write)
            // 4. 渲染 Footer (副作用，甚至不需要关心上一步是 Table，只要链条还在)
            .Tap(_ => RenderFooter());
    }

    // --- 纯函数区域 (Pure Functions) ---

    // 提取表格结构定义，只负责 "What"，不负责 "Data"
    private static Table CreateTableStructure() =>
        new Table()
            .Expand()
            .Border(TableBorder.Square)
            .AddColumn(new TableColumn("[blue bold]ID & Variant[/]").Centered().Width(20))
            .AddColumn(new TableColumn("[white]State & Output[/]"))
            .AddColumn(new TableColumn("[white]Status[/]").Centered().Width(20));

    // 将单个数据转换为 UI 组件数组 (Map 操作)
    private static IRenderable[] MapToRenderableRow(InstructionRow row) =>
        [
            new Markup($"[blue bold]{row.GetIdString()}[/]"),
            new Rows(
                new Rule($"[blue][[{row.Action.ToString().ToUpper()}]][/]").RuleStyle("grey").LeftJustified(),
                new Markup($"{Markup.Escape(row.OutputText)}")
            ),
            new Markup(GetStatusMarkup(row.Status))
        ];

    // --- 副作用区域 (Side Effects) ---

    private static void RenderFooter()
    {
        AnsiConsole.MarkupLine("[grey]Commands: [bold underline]q[/]uery, [bold underline]e[/]nable, [bold underline]r[/]eset, [bold underline]run[/], [bold underline]clear[/], [bold underline]exit[/][/]");
        AnsiConsole.MarkupLine("[grey]Example: 'q 1122; e 3344 5566, 7788+9'[/]");
        AnsiConsole.Write(new Rule().RuleStyle("grey"));
    }

    private static string GetStatusMarkup(RowStatus status) => status switch
    {
        RowStatus.Initializing => "[grey]Checking...[/]",
        RowStatus.Skipped => "[grey]Skipped[/]",
        RowStatus.Confirming => "[yellow]Confirming[/]",
        RowStatus.Configured => "[green]Configured[/]",
        RowStatus.Error => "[red]Error[/]",
        _ => "Unknown"
    };
}