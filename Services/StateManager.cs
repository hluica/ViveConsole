using ViveConsole.Models;

namespace ViveConsole.Services;

public class StateManager
{
    private readonly List<InstructionRow> _rows = [];

    public IReadOnlyList<InstructionRow> Rows => _rows;

    public List<InstructionRow> Upsert(IEnumerable<InstructionRow> incomingRows)
    {
        // 1. 建立索引：将现有行转换为字典，以便 O(1) 查找 (Key: Id+Variant)
        var existingIndex = _rows.ToDictionary(r => (r.Id, r.Variant));

        // 2. 管道处理：输入流 -> 匹配现有项 -> 应用策略 -> 输出结果流
        return [.. incomingRows.Select(incoming =>
        {
            var key = (incoming.Id, incoming.Variant);

            // 使用模式匹配处理 "存在" vs "不存在" 的分支
            return existingIndex.TryGetValue(key, out var existing)
                ? MergeStrategy(existing, incoming)  // 存在：应用合并策略 (副作用在内部)
                : AppendStrategy(incoming);          // 不存在：应用追加策略 (副作用在内部)

        })];
    }

    // Upsert 策略辅助方法 1: 合并 (纯逻辑 + 局部状态更新)
    private static InstructionRow MergeStrategy(InstructionRow current, InstructionRow incoming)
    {
        // 业务规则：如果新指令不是 Query (即它是 E 或 R)，则覆盖旧动作并重置状态
        // 这是一个 "Mutating Projection" (变更投影)
        if (incoming.Action != ActionType.Query)
        {
            current.Action = incoming.Action;
            current.Status = RowStatus.Confirming;
        }

        // 如果是 Query，则保持原样 (Action不便，Status不变)
        // 始终返回当前对象，保持引用一致性
        return current;
    }

    // Upsert 策略辅助方法 2: 追加 (副作用)
    private InstructionRow AppendStrategy(InstructionRow incoming)
    {
        _rows.Add(incoming);
        return incoming;
    }

    // --- 其他状态管理方法 ---

    public List<InstructionRow> GetExecutableRows() =>
        [.. _rows.Where(r => r.Action is ActionType.Enable or ActionType.Reset)];

    public void Clear() => _rows.Clear();
}