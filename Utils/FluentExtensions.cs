using Spectre.Console;
using Spectre.Console.Rendering;

namespace ViveConsole.Utils;

public static class FluentExtensions
{
    // 函数式核心：执行副作用，但返回对象本身，保持链条不断
    public static T Tap<T>(this T self, Action<T> action)
    {
        action(self);
        return self;
    }

    // 条件分支：基于条件对对象进行不同处理
    public static T ApplyIf<T>(this T self, bool condition, Action<T> whenTrue, Action<T>? whenFalse = null)
    {
        if (condition) whenTrue(self);
        else whenFalse?.Invoke(self);
        return self;
    }

    // 集合折叠：把 foreach 变成 Aggregate，让数据流进 Table
    public static Table AddRowsFrom<T>(this Table table, IEnumerable<T> source, Func<T, IRenderable[]> rowMapper)
    {
        return source.Aggregate(table, (currentTable, item) => currentTable.AddRow(rowMapper(item)));
    }
}