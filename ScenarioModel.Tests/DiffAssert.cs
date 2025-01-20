using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ScenarioModelling.Tests;

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

        Assert.Fail("The diff failed, winmerge was opened with two versions of the text");
    }

    public static void DiffIfNotEqual(string left, string middle, string right, StringComparison stringComparison = StringComparison.Ordinal, [CallerArgumentExpression("left")] string leftName = "", [CallerArgumentExpression("middle")] string middleName = "", [CallerArgumentExpression("right")] string rightName = "")
    {
        bool leftToMiddle = string.Equals(left, middle, stringComparison);
        bool middleToRight = string.Equals(middle, right, stringComparison);
        if (leftToMiddle && middleToRight)
            return;

        // If the texts are very similar we'd like a bit more details since winmerge seems to be more tolerant than string.Equals
        var leftLines = left.Replace("\r", "").Split('\n').ToList();
        var middleLines = middle.Replace("\r", "").Split('\n').ToList();
        var rightLines = right.Replace("\r", "").Split('\n').ToList();

        if (leftLines.Count == middleLines.Count && middleLines.Count == rightLines.Count)
        {
            for (int i = 0; i < leftLines.Count; i++)
            {
                Debug.WriteLine("\nThe following lines are where the left text differs from the middle text :");
                if (!string.Equals(leftLines[i], middleLines[i], stringComparison))
                {
                    Debug.WriteLine($"({i}) {leftLines[i]}");
                    Debug.WriteLine($"({i}) {middleLines[i]}");
                    LinePositionDiffIndicator(leftLines[i], middleLines[i]);
                }
                
                Debug.WriteLine("\nThe following lines are where the middle text differs from the right text :");
                if (!string.Equals(middleLines[i], rightLines[i], stringComparison))
                {
                    Debug.WriteLine($"({i}) {middleLines[i]}");
                    Debug.WriteLine($"({i}) {rightLines[i]}");
                    LinePositionDiffIndicator(middleLines[i], rightLines[i]);
                }
            }
        }

        // Launch the system default diff tool
        var leftTemp = GetUniqueFilename(leftName);
        var middleTemp = GetUniqueFilename(middleName);
        var rightTemp = GetUniqueFilename(rightName);

        File.WriteAllText(leftTemp, left.Trim());
        File.WriteAllText(middleTemp, middle.Trim());
        File.WriteAllText(rightTemp, right.Trim());

        // Launch winmerge
        Process.Start(@"C:\Program Files\WinMerge\WinMergeU.exe", $"/dl {leftName} /dm {middleName} /dr {rightName} {leftTemp} {middleTemp} {rightTemp}");

        Assert.Fail("The diff failed, winmerge was opened with three versions of the text");
    }

    private static void LinePositionDiffIndicator(string v1, string v2)
    {
        for (int i = 0; i < Math.Min(v1.Length, v2.Length); i++)
        {
            if (v1[i] == v2[i])
                Debug.Write(" ");
            else
                Debug.Write("^");
        }
    }

    private static string GetUniqueFilename(string name = "")
    {
        string fullTemp = Path.GetTempFileName();
        string path = Path.GetDirectoryName(fullTemp);
        string filename = Path.GetFileNameWithoutExtension(fullTemp);

        return Path.Combine(path, $"{filename}_{name}");
    }
}
