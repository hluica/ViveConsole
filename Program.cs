using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Spectre.Console;

using ViveConsole.Services;
using ViveConsole.Services.Backend;
using ViveConsole.Utils;

namespace ViveConsole;

internal class Program
{
    static async Task Main(string[] args)
    {
        // 0. 全局操作系统检查
        if (!OperatingSystem.IsWindows())
        {
            Console.WriteLine("此工具仅支持 Windows 平台。");
            return;
        }

        // 1. 处理 --help
        if (args.Contains("--help"))
        {
            ConsoleHelper.ShowHelp();
            return;
        }

        // 2. 检查管理员权限
        if (!ConsoleHelper.IsAdministrator())
        {
            AnsiConsole.MarkupLine("[red]Error: This application requires Administrator privileges.[/]");
            return;
        }

        // 3. 检查 vivetool 是否在 Path 中
        if (!ConsoleHelper.IsVivetoolInPath())
        {
            AnsiConsole.MarkupLine("[red]Error: 'vivetool.exe' was not found in your PATH.[/]");
            AnsiConsole.MarkupLine("Please download it from: [blue]https://github.com/thebookisclosed/ViVe/releases[/]");
            return;
        }

        // 4. 配置 Host 和依赖注入
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddSingleton<CommandParser>();
        builder.Services.AddSingleton<StateManager>();
        builder.Services.AddSingleton<IVivetoolAdapter, VivetoolAdapter>();
        builder.Services.AddSingleton<UserInterface>();
        builder.Services.AddSingleton<App>();

        using IHost host = builder.Build();

        // 5. 运行 App
        var app = host.Services.GetRequiredService<App>();
        await app.RunAsync();
    }
}
