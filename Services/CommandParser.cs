using ViveConsole.Models;

namespace ViveConsole.Services;

public class CommandParser
{
    public List<InstructionRow> Parse(string input)
        => string.IsNullOrWhiteSpace(input)
            ? []
            : [.. input
                // 1. 分割指令段并预处理 (Map & Filter)
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                // 2. 将字符串转换为上下文对象 (Map)
                .Select(ParseSegmentContext)
                // 3. 过滤掉无效动作 (Filter)
                .Where(ctx => ctx.Action.HasValue)
                // 4. 解析参数并扁平化结果 (FlatMap / Bind)
                .SelectMany(ctx => ParseTokensToRows(ctx.Action!.Value, ctx.ParamPart))
            ];

    // 纯函数：解析动作和剩余参数字符串
    private static (ActionType? Action, string ParamPart) ParseSegmentContext(string segment)
    {
        if (string.IsNullOrEmpty(segment))
            return (null, string.Empty);

        var action = char.ToLower(segment[0]) switch
        {
            'q' => ActionType.Query,
            'e' => ActionType.Enable,
            'r' => ActionType.Reset,
            _ => (ActionType?)null
        };

        return (action, segment.Length > 1 ? segment[1..] : string.Empty);
    }

    // 纯函数：将参数部分转换为指令行列表
    private static IEnumerable<InstructionRow> ParseTokensToRows(ActionType action, string paramPart)
        => paramPart
            .Split([' ', ','], StringSplitOptions.RemoveEmptyEntries) // 1. 分割字符串
            .Select(ParseIdToken)                                     // 2. 解析，得到可空的 (uint, uint?)?
            .Where(t => t.HasValue)                                   // 3. 过滤掉格式错误的 Token
            .Select(t => t!.Value)                                    // 4. 拆箱，变成非空的 (uint, uint?)
            .Distinct()                                               // 5. 对元组进行去重
            .Select(t => new InstructionRow                           // 6. 转换为最终对象
            {
                Id = t.Id,
                Variant = t.Variant,
                Action = action,
                Status = action == ActionType.Query ? RowStatus.Skipped : RowStatus.Confirming
            });

    // 纯函数：解析单个 ID Token，返回可空元组
    private static (uint Id, uint? Variant)? ParseIdToken(string token)
    {
        string[] parts = token.Split('+');

        // 解析 ID (失败则返回 null)
        if (!uint.TryParse(parts[0], out uint id))
            return null;

        // 解析 Variant (如果没有部分则为null，如果有但解析失败则整个Token无效)
        uint? variant = null;
        if (parts.Length > 1)
        {
            if (uint.TryParse(parts[1], out uint v))
                variant = v;
            else
                return null; // 格式错误 (例如 "123+abc")
        }

        return (id, variant);
    }
}
