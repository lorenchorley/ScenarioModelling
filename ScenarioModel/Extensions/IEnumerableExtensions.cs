using LanguageExt;
using System.Diagnostics;

public static class IEnumerableExtensions
{
    [DebuggerNonUserCode]
    public static IEnumerable<R> ChooseAndAssertAllSelected<T, R>(this IEnumerable<T> list, Func<T, Option<R>> selector, string messageTemplate)
    {
        var (chosen, remaining) = list.PartitionByChoose(selector);

        if (remaining.Any())
        {
            throw new Exception(string.Format(messageTemplate, remaining.CommaSeparatedList()));
        }

        return chosen;
    }

    [DebuggerNonUserCode]
    public static (IEnumerable<R>, IEnumerable<T>) PartitionByChoose<T, R>(this IEnumerable<T> list, Func<T, Option<R>> selector)
    {
        List<R> chosen = new();
        List<T> remaining = new();

        foreach (T item in list)
        {
            Option<R> evaluated = selector(item);
            if (evaluated.IsSome)
            {
                chosen.Add((R)evaluated.Case);
            }
            else
            {
                remaining.Add(item);
            }
        }

        return (chosen, remaining);
    }
}
