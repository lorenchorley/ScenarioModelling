using ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public static class StringExtensions
{
    public static bool IsEqv(this string firstStr, string secondStr)
    {
        return string.Equals(firstStr, secondStr, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsEqvCountingNulls(this string? firstStr, string? secondStr)
    {
        if (firstStr == null || secondStr == null)
        {
            if (firstStr != null || secondStr != null)
                return false; // If only one is null
            else
                return true; // If both are null
        }

        return string.Equals(firstStr, secondStr, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsEqv(this string firstStr, StringValue secondStr)
    {
        return string.Equals(firstStr, secondStr.Value, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsEqv(this StringValue firstStr, string secondStr)
    {
        return string.Equals(firstStr.Value, secondStr, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsEqv(this StringValue firstStr, StringValue secondStr)
    {
        return string.Equals(firstStr.Value, secondStr.Value, StringComparison.CurrentCultureIgnoreCase);
    }

    public static string CommaSeparatedList(this IEnumerable<string> list)
    {
        return string.Join(", ", list);
    }

    public static string DotSeparatedList(this IEnumerable<string> list)
    {
        return string.Join(".", list);
    }

    public static string BulletPointList(this IEnumerable<string> list)
    {
        return string.Join("", list.Select(i => $"\n* {i}"));
    }

    public static string CommaSeparatedList<T>(this IEnumerable<T> list)
    {
        return string.Join(", ", list.Select(s => s?.ToString() ?? "null"));
    }

    public static string DotSeparatedList<T>(this IEnumerable<T> list)
    {
        return string.Join(".", list.Select(s => s?.ToString() ?? "null"));
    }

    public static IEnumerable<T> ThrowIfDuplicatesDetected<T>(this IEnumerable<T> list, Func<T, string> getKey)
    {
        var duplicates =
            list.GroupBy(getKey)
                .Where(g => g.Count() > 1)
                .Select(g => $@"""{g.Key}""")
                .ToList();

        if (duplicates.Any())
            throw new Exception($"Duplicate keys detected in list: {duplicates.CommaSeparatedList()}");

        return list;
    }

    public static string AddQuotes(this string str)
        => str.Contains(' ') ? $@"""{str}""" : str;
}
