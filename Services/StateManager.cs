using ViveConsole.Models;

namespace ViveConsole.Services;

public class StateManager
{
    private readonly List<InstructionRow> _rows = [];

    public List<InstructionRow> Rows => _rows;

    public void AddRange(IEnumerable<InstructionRow> newRows)
    {
        _rows.AddRange(newRows);
    }

    // 获取需要在 Run 阶段执行的行 (排除 Query)
    public List<InstructionRow> GetExecutableRows()
    {
        return [.. _rows.Where(r => r.Action == ActionType.Enable || r.Action == ActionType.Reset)];
    }

    public void Clear()
    {
        _rows.Clear();
    }
}