# ViveConsole

[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![GitHub Release](https://img.shields.io/github/v/release/hluica/ViveConsole)](https://github.com/hluica/ViveConsole/releases/latest)
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/hluica/ViveConsole)

ViveConsole 是一个简单的交互式前端工具，用于管理 [ViVeTool](https://github.com/thebookisclosed/ViVe) 的 Windows 功能配置。

> [!Tip]
>
> 本工具已进入维护阶段，不会主动进行功能更新。新功能将首先在 [ViveGui](https://github.com/hluica/ViveGui) 上应用。向本工具的功能移植可能会滞后。建议您切换到 ViveGui。

## 功能

- **查询功能状态**：通过 `q <id>` 命令查询指定功能 ID 的当前状态。
- **启用功能**：通过 `e <id>[+var]` 命令添加启用请求到队列，支持可选的变体（Variant）。
- **重置功能**：通过 `r <id>` 命令添加重置请求到队列。
- **批量执行**：通过 `run` 命令批量执行所有队列中的启用和重置指令。

## 使用方法

### 基本说明

1. 运行程序以进入交互式 Shell。
2. 使用`--help`命令查看帮助信息

### 内部命令

- `q <id>`: 仅查询指定功能 ID 的状态，不加入执行队列。
- `e <id>[+var]`: 启用指定功能 ID，可选添加变体。
- `r <id>`: 重置指定功能 ID。
- `run`: 执行队列中的所有启用和重置请求。
- `clear`: 清空当前队列和被记录的查询信息。
- `exit`: 退出程序。

### 语法提示

- 功能ID为整数，变体编号为1到63之间的整数。使用`+`符号连接功能ID和变体编号，且两者间不应有空格。
- 可用空格、逗号或同时使用空格与逗号分割多个功能ID；可用分号分割多条命令。

## 系统要求

- 仅用于 Windows 系统。根据 .NET 工具的配置要求，本工具可以在其它系统上安装，但在运行时会提示不可用并直接退出。
- 需要管理员权限。
- 需要自行安装 ViveTool。

## 安装

ViveConsole 是一个 .NET 全局工具，尽管 ViveTool 仅在 Windows 系统可用。获取软件包 `.nupkg` 文件后，可以通过以下命令安装：
```powershell
dotnet tool install ViveConsole --global --add-source <Source_Path> [--version <Version>]
```
其中 `<Source_Path>` 是包含 `.nupkg` 文件的目录路径（允许使用相对路径）；`<Version>` 是 `.nupkg` 文件名中列出的版本，当 `<Source_Path>` 目录内只有一个 `.nupkg` 文件时可以省略。

您可以从 Release 页面下载 `.nupkg` 文件，或者手动构建：
- 安装 .NET SDK；
- Clone 存储库，进入仓库根目录；
- 执行 `dotnet pack` 指令。
- 生成的`.nupkg` 文件将存储在仓库的 `/nupkg` 目录下。

## 版本历史纪录

| 版本   | 日期     | 更新内容                                                           |
| ------ | -------- | ------------------------------------------------------------------ |
| v2.1.2 | 26-02-19 | 更新依赖。                                                         |
| v2.1.1 | 01-21-26 | 优化程序架构，完全使用依赖注入；应用新的代码样式；更新依赖库版本。 |
| v2.0.0 | 12-10-25 | 大规模重构执行逻辑，使用函数式编程风格重写代码。                   |
| v1.1.0 | 12-10-25 | 更改vivetool命令队列和执行逻辑。                                   |
| v1.0.2 | 12-09-25 | 初始可用版本。                                                     |

## 许可证

[MIT](LICENSE)
