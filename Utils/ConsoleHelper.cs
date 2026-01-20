using System.Runtime.Versioning;
using System.Security.Principal;

using Spectre.Console;

namespace ViveConsole.Utils;

public static class ConsoleHelper
{
    [SupportedOSPlatform("windows")]
    public static bool IsAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static bool IsVivetoolInPath()
    {
        string pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        string[] paths = pathEnv.Split(Path.PathSeparator);

        foreach (string path in paths)
        {
            string fullPath = Path.Combine(path, "vivetool.exe");
            if (File.Exists(fullPath))
                return true;
        }
        return false;
    }

    public static void ShowHelp()
    {
        // 1. 标题与头部信息
        // 使用 Rule 绘制一条带标题的横线
        AnsiConsole.Write(new Rule("[blue]ViveTool Wrapper[/]").RuleStyle("grey").LeftJustified());

        // 简要说明
        AnsiConsole.MarkupLine("A simple, interactive frontend for [cyan]ViVeTool[/].");
        AnsiConsole.MarkupLine("Manage Windows feature configurations with ease.");
        AnsiConsole.WriteLine();

        // 2. 用法说明 (使用 Panel 或简单的 Markup)
        AnsiConsole.MarkupLine("[blue bold]Usage:[/]");
        AnsiConsole.MarkupLine("  Run the program to enter the interactive shell.");
        AnsiConsole.MarkupLine("  Use [blue]--help[/] as a launch argument to see this message.");
        AnsiConsole.WriteLine();

        // 3. 内部命令列表
        AnsiConsole.MarkupLine("[blue bold]Internal Commands:[/]");

        var grid = new Grid();
        // 定义三列：命令、参数、说明
        _ = grid.AddColumn(new GridColumn().NoWrap().PadRight(2)); // 命令列，不换行
        _ = grid.AddColumn(new GridColumn().PadRight(4));          // 参数列
        _ = grid.AddColumn(new GridColumn());                      // 说明列，自动填充剩余空间

        // 添加行内容: 命令, 参数语法, 详细说明
        _ = grid
            .AddRow("[blue]q[/]", "[silver]<id>[/]", "Query the current status of a Feature ID. Executes immediately.")
            .AddRow("[blue]e[/]", "[silver]<id>[[+var]][/]", "Queue an [green]Enable[/] request. Supports optional variant (e.g., +1).")
            .AddRow("[blue]r[/]", "[silver]<id>[/]", "Queue a [yellow]Reset[/] request for the specified Feature ID.")
            .AddRow("[blue]run[/]", "", "Execute all queued [green]Enable[/] & [yellow]Reset[/] instructions in batch.")
            .AddRow("[blue]clear[/]", "", "Clear all pending instructions from the queue.")
            .AddRow("[blue]exit[/]", "", "Close the application.");

        // 渲染 Grid
        AnsiConsole.Write(grid);
        AnsiConsole.WriteLine();

        // 4. 语法提示
        AnsiConsole.MarkupLine("[blue bold]Syntax Notes:[/]");
        AnsiConsole.MarkupLine("  - Multiple IDs can be separated by spaces or commas or mix of both.");
        AnsiConsole.MarkupLine("  - Multiple commands can be chained using [yellow];[/] (semicolon).");
        AnsiConsole.MarkupLine("  - Variant separator [yellow]+[/] requires [red]no spaces[/] on either side.");

        // 底部结束线
        AnsiConsole.WriteLine();
    }
}
