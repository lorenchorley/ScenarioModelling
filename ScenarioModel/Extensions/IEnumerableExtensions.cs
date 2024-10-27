using LanguageExt;
using System.Diagnostics;
using System.Xml;

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

    /// <summary>
    /// Determines how many unique object instances are in a list using ReferenceEquals
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [DebuggerNonUserCode]
    public static int UniqueObjectInstanceCount(this IEnumerable<object> list)
    {
        List<object> unique = new();

        foreach (object obj in list)
        {
            if (unique.Any(u => ReferenceEquals(u, obj)))
                continue;

            unique.Add(obj);
        }

        return unique.Count;
    }

    public static IEnumerable<T> DistinctByReference<T>(this IEnumerable<T> list)
    {
        List<T> distinct = new();

        foreach (T item in list)
        {
            if (distinct.Any(d => ReferenceEquals(d, item)))
                continue;

            distinct.Add(item);
        }

        return distinct;
    }

}
