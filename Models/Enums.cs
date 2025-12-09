namespace ViveConsole.Models;

public enum ActionType
{
    Query,
    Enable,
    Reset
}

public enum RowStatus
{
    Initializing, // 刚添加，尚未获取 Query 结果
    Skipped,      // (q 指令) 灰色，不做更改
    Confirming,   // (e/r 指令) 黄色，等待 Run
    Configured,   // 执行成功，绿色
    Error         // 执行失败，红色
}