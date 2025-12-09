using Spectre.Console;
using ViveConsole.Models;

namespace ViveConsole.Services;

public class UserInterface
{
    public void RenderScreen(List<InstructionRow> rows)
    {
        AnsiConsole.Clear();

        // 1. 创建表格
        var table = new Table();
        table.Expand();
        table.Border(TableBorder.Square);

        table.AddColumn(new TableColumn("[blue bold]ID & Variant[/]").Centered().Width(20));
        table.AddColumn(new TableColumn("[white]State & Output[/]"));
        table.AddColumn(new TableColumn("[white]Status[/]").Centered().Width(20));

        // 2. 填充行
        if (rows.Count == 0)
        {
            table.AddRow(new Markup(""), new Rule("[grey]Waiting for input...[/]").RuleStyle("grey").LeftJustified(), new Markup(""));
        }
        else
        {
            foreach (var row in rows)
            {
                var idMarkup = new Markup($"[blue bold]{row.GetIdString()}[/]");
                var actionPrefix = row.Action.ToString().ToUpper();
                var contentMarkup = new Rows(
                    new Rule($"[blue][[{actionPrefix}]][/]").RuleStyle("grey").LeftJustified(),
                    new Markup($"{Markup.Escape(row.OutputText)}")
                );

                var statusMarkup = new Markup(GetStatusMarkup(row.Status));

                table.AddRow(idMarkup, contentMarkup, statusMarkup);
            }
        }

        // 3. 渲染
        AnsiConsole.Write(new Rule("[blue]ViVeTool Wrapper[/]").RuleStyle("grey").LeftJustified());
        AnsiConsole.Write(table);

        // 底部提示
        AnsiConsole.MarkupLine("[grey]Commands: [bold underline]q[/]uery, [bold underline]e[/]nable, [bold underline]r[/]eset, [bold underline]run[/], [bold underline]clear[/], [bold underline]exit[/][/]");
        AnsiConsole.MarkupLine("[grey]Example: 'q 1122; e 3344 5566, 7788+9'[/]");
        AnsiConsole.Write(new Rule().RuleStyle("grey"));
    }

    private static string GetStatusMarkup(RowStatus status)
    {
        return status switch
        {
            RowStatus.Initializing => "[grey]Checking...[/]",
            RowStatus.Skipped => "[grey]Skipped[/]",
            RowStatus.Confirming => "[yellow]Confirming[/]",
            RowStatus.Configured => "[green]Configured[/]",
            RowStatus.Error => "[red]Error[/]",
            _ => "Unknown"
        };
    }
}