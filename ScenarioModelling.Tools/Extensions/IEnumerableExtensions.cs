using LanguageExt;
using System.Diagnostics;

public static class IEnumerableExtensions
{
    public static IEnumerable<(R, S)> CombinePairwise<R, S>(this IEnumerable<R> first, IEnumerable<S> second)
    {
        var firstEnumerator = first.GetEnumerator();
        var secondEnumerator = second.GetEnumerator();

        bool MoveNext()
        {
            bool v = firstEnumerator.MoveNext();
            bool u = secondEnumerator.MoveNext();

            if (v != u)
            {
                throw new Exception("Sequence lengths do not match");
            }

            return v;
        }

        while (MoveNext())
        {
            yield return (firstEnumerator.Current, secondEnumerator.Current);
        }
    }

    [DebuggerNonUserCode]
    public static IEnumerable<R> ChooseAndAssertAllSelected<T, R>(this IEnumerable<T> list, Func<T, Option<R>> selector, string messageTemplate)
    {
        var (chosen, remaining) = list.PartitionByChoose(selector);

        if (remaining.Any())
        {
            throw new Exception(string.Format(messageTemplate, remaining.CommaSeparatedList()) + "\nTry registering a new ISemanticNodeProfile that corresponds to this definition");
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

    [DebuggerNonUserCode]
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
