using FluentAssertions;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.Interpreter;
using ScenarioModelling.TestDataAndTools.Serialisation;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScenarioModelling.Serialisation.Tests;

[TestClass]
public class CustomSerialiserGrammarTests
{
    [DataTestMethod]
    [TestCategory("Grammar")]
    [CustomSerialiserGrammarTestDataProvider]
    public void GrammarTests_Valid(string name, string text, string expectedResultRegex)
    {
        // Arrange
        // =======
        CustomSerialiserInterpreter interpreter = new();

        // Act
        // ===
        var result = interpreter.Parse(text);

        // Assert
        // ======
        result.HasErrors.Should().BeFalse(because: string.Join("\n", result.Errors));

        string serialisedResult = result.ParsedObject!.ToString();

        Debug.WriteLine("Result :");
        Debug.WriteLine(result);
        Debug.WriteLine("");

        Regex regex = new(expectedResultRegex.Trim(), RegexOptions.Multiline);
        Assert.IsTrue(regex.IsMatch(serialisedResult.Trim()), $"The result did not correspond to the expected regex expression\n\nResult: {result}\nExpression: {expectedResultRegex}");
    }

    [TestMethod]
    [TestCategory("Grammar")]
    public void GrammarTestData()
    {
        // Arrange
        // =======
        CustomSerialiserGrammarTestDataProviderAttribute grammarTestDataProvider = new();

        // Act
        // ===
        var list = grammarTestDataProvider.TestData;

        // Assert
        // ======

        list.Should().NotBeNullOrEmpty();

        foreach (var item in list)
        {
            Debug.WriteLine($"{item}");
        }
    }


}