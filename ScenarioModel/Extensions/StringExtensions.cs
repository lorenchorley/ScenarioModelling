﻿using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;

public static class StringExtensions
{
    public static bool IsEqv(this string firstStr, string secondStr)
    {
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
}