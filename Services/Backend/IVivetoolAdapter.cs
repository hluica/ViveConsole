using ViveConsole.Models;

namespace ViveConsole.Services.Backend;

public interface IVivetoolAdapter
{
    // 用于单条查询
    Task<string> QuerySingleAsync(uint id);
    // 用于批量执行 (Run)
    Task ExecuteBatchAsync(List<InstructionRow> rows);
}