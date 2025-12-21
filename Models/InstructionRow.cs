namespace ViveConsole.Models;

public class InstructionRow
{
    public uint Id { get; set; }
    public uint? Variant { get; set; }
    public ActionType Action { get; set; }

    // UI 显示内容
    public string OutputText { get; set; } = "Initializing...";
    public RowStatus Status { get; set; } = RowStatus.Initializing;

    // 辅助属性：获取格式化的 ID 字符串
    public string GetIdString()
        => Variant.HasValue ? $"{Id} (Var: {Variant})" : $"{Id}";
}
