# ViveConsole

ViveConsole 是一个简单的交互式前端工具，用于管理 [ViVeTool](https://github.com/thebookisclosed/ViVe) 的 Windows 功能配置。

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

- 仅用于Windows系统。
- 需要管理员权限。
- 需要自行安装ViveTool。

## 安装

- 建议作为nuget全局工具安装。
- clone或下载到本地后，进入项目目录，运行以下命令进行安装：

```powershell
# 首先打包项目
dotnet pack -c Release

# 然后安装为全局工具，建议指定版本号
dotnet tool install --global --add-source ./nupkg ViveConsole --version <版本号>
```

## 版本历史纪录

- v1.0.2 - 12-09-25 - 初始可用版本。

## 许可证

[MIT](LICENSE)