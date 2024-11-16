using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ScenarioModel.Tests;

public static class DiffAssert
{
    public static void DiffIfNotEqual(string left, string right, StringComparison stringComparison = StringComparison.Ordinal, [CallerArgumentExpression("left")] string leftName = "", [CallerArgumentExpression("right")] string rightName = "")
    {
        if (string.Equals(left, right, stringComparison))
            return;

        // Launch the system default diff tool
        var leftTemp = GetUniqueFilename(leftName);
        var rightTemp = GetUniqueFilename(rightName);

        File.WriteAllText(leftTemp, left.Trim());
        File.WriteAllText(rightTemp, right.Trim());

        // Launch winmerge
        Process.Start(@"C:\Program Files\WinMerge\WinMergeU.exe", $"/dl {leftName} /dr {rightName} {leftTemp} {rightTemp}");

        Assert.Fail();
    }

    public static void DiffIfNotEqual(string left, string middle, string right, StringComparison stringComparison = StringComparison.Ordinal, [CallerArgumentExpression("left")] string leftName = "", [CallerArgumentExpression("middle")] string middleName = "", [CallerArgumentExpression("right")] string rightName = "")
    {
        if (string.Equals(left, right, stringComparison) && string.Equals(left, middle, stringComparison))
            return;

        // Launch the system default diff tool
        var leftTemp = GetUniqueFilename(leftName);
        var middleTemp = GetUniqueFilename(middleName);
        var rightTemp = GetUniqueFilename(rightName);

        File.WriteAllText(leftTemp, left.Trim());
        File.WriteAllText(middleTemp, middle.Trim());
        File.WriteAllText(rightTemp, right.Trim());

        // Launch winmerge
        Process.Start(@"C:\Program Files\WinMerge\WinMergeU.exe", $"/dl {leftName} /dm {middleName} /dr {rightName} {leftTemp} {middleTemp} {rightTemp}");

        Assert.Fail();
    }

    private static string GetUniqueFilename(string name = "")
    {
        string fullTemp = Path.GetTempFileName();
        string path = Path.GetDirectoryName(fullTemp);
        string filename = Path.GetFileNameWithoutExtension(fullTemp);

        return Path.Combine(path, $"{filename}_{name}");
    }
}
