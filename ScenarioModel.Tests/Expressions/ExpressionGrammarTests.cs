using FluentAssertions;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.States;
using System.Diagnostics;

namespace ScenarioModel.Tests.Expressions;

[TestClass]
public class ExpressionGrammarTests
{
    private readonly System _system = new System()
    {
        Entities = new List<Entity>()
        {
            new Entity()
            {
                Name = "E1",
                State = new State()
                {
                    Name = "S1"
                }
            }
        },
    };

    [DataTestMethod]
    [TestCategory("Expressions")]
    [ExpressionGrammarTestDataProvider]
    public void ExpressionGrammarTests_Succeeds(string name, string text, string expected)
    {
        // Arrange 
        // =======
        ExpressionInterpreter interpreter = new();

        // Act
        // ===
        var result = interpreter.Parse(text);

        // Assert
        // ======
        result.HasErrors.Should().BeFalse(because: string.Join("\n", result.Errors));

        string serialisedResult = result.Tree.ToString();

        Debug.WriteLine("Result :");
        Debug.WriteLine(serialisedResult);
        Debug.WriteLine("");

        serialisedResult.Should().Be(expected);
    }
}