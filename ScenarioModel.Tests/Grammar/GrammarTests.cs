using FluentAssertions;
using ScenarioModel.Serialisation.HumanReadable.Interpreter;
using System.Diagnostics;

namespace ScenarioModel.Tests;

[TestClass]
public class GrammarTests
{
    [DataTestMethod]
    [TestCategory("Grammar")]
    [GrammarTestDataProvider]
    public void GrammarTests_Valid(string name, string text, string expectedResult)
    {
        // Arrange
        // =======
        HumanReadableInterpreter interpreter = new();

        // Act
        // ===
        var result = interpreter.Parse(text);

        // Assert
        // ======
        result.HasErrors.Should().BeFalse();

        string serialisedResult = result.Tree.ToString();

        Debug.WriteLine("Results :");
        Debug.WriteLine(result);

        Assert.AreEqual(expectedResult.Trim(), serialisedResult.Trim());
    }
}