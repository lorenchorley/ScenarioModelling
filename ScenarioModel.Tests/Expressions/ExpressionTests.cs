using FluentAssertions;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Expressions.Validation;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using System.Diagnostics;

namespace ScenarioModel.Tests.Expressions;

[TestClass]
public class ExpressionTests
{
    private readonly string _system = """
        Entity A
        {
            Aspect D {
                State DState
            }
        }
        Entity B
        Entity C
        """;

    [DataTestMethod]
    [TestCategory("Expressions"), TestCategory("Grammar")]
    [ExpressionGrammarTestDataProvider]
    public void ExpressionGrammarTests(string name, string text, string expected, bool isValid)
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

        string serialisedResult = result.ParsedObject.ToString();

        Debug.WriteLine("Result :");
        Debug.WriteLine(serialisedResult);
        Debug.WriteLine("");

        serialisedResult.Should().Be(expected);
    }

    [DataTestMethod]
    [TestCategory("Expressions"), TestCategory("Grammar")]
    [ExpressionGrammarTestDataProvider]
    public void ExpressionValidationTests(string name, string text, string expected, bool isValid)
    {
        // Arrange 
        // =======
        ExpressionInterpreter interpreter = new();

        System system =
            Context.New()
                   .UseSerialiser<HumanReadablePromptSerialiser>()
                   .LoadContext<HumanReadablePromptSerialiser>(_system)
                   .Initialise().System;

        ValidatorVisitor visitor = new(system);

        var result = interpreter.Parse(text);


        // Act
        // ===
        result.ParsedObject.Accept(visitor);


        // Assert
        // ======
        Assert.AreEqual(isValid, visitor.Errors.Count == 0, visitor.Errors.CommaSeparatedList());
    }

    [DataTestMethod]
    [TestCategory("Expressions"), TestCategory("Serialisation")]
    [ExpressionGrammarTestDataProvider]
    public void ExpressionSerialisationTests(string name, string text, string expected, bool isValid)
    {
        // Arrange 
        // =======
        ExpressionInterpreter interpreter = new();

        System system =
            Context.New()
                   .UseSerialiser<HumanReadablePromptSerialiser>()
                   .LoadContext<HumanReadablePromptSerialiser>(_system)
                   .Initialise().System;

        ExpressionSerialisationVisitor visitor = new(system);

        var parsedExpression = interpreter.Parse(text);

        if (parsedExpression.HasErrors)
        {
            Assert.Inconclusive();
        }


        // Act
        // ===
        var result = parsedExpression.ParsedObject.Accept(visitor);


        // Assert
        // ======
        string resultString = (string)result;

        var actualCleaned = resultString.ToUpperInvariant().Replace("||", "OR").Replace("&&", "AND").Replace(" ", "");
        var expectedCleaned = text.ToUpperInvariant().Replace("||", "OR").Replace("&&", "AND").Replace(" ", "");

        Assert.AreEqual(expectedCleaned, actualCleaned);
    }
}