using FluentAssertions;
using ScenarioModel.Expressions.Evaluation;
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
        B -> C

        """;

    [DataTestMethod]
    [TestCategory("Expressions"), TestCategory("Grammar")]
    [ExpressionGrammarTestDataProvider]
    public void ExpressionGrammarTests(string name, string text, string expected, bool isValid, object? evaluatedExpression)
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
    public void ExpressionValidationTests(string name, string text, string expected, bool isValid, object? evaluatedExpression)
    {
        // Arrange 
        // =======
        ExpressionInterpreter interpreter = new();

        System system =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .LoadContext<HumanReadableSerialiser>(_system)
                   .Initialise().System;

        ExpressionValidator visitor = new(system);

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
    public void ExpressionSerialisationTests(string name, string text, string expected, bool isValid, object? evaluatedExpression)
    {
        // Arrange 
        // =======
        ExpressionInterpreter interpreter = new();

        System system =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .LoadContext<HumanReadableSerialiser>(_system)
                   .Initialise().System;

        ExpressionSerialiser visitor = new(system);

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

    [DataTestMethod]
    [TestCategory("Expressions"), TestCategory("Evaluation")]
    [ExpressionGrammarTestDataProvider]
    public void ExpressionEvaluationTests(string name, string text, string expected, bool isValid, object? evaluatedExpression)
    {
        // Arrange 
        // =======
        if (!isValid)
        {
            return;
        }

        ExpressionInterpreter interpreter = new();

        System system =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .LoadContext<HumanReadableSerialiser>(_system)
                   .Initialise().System;

        ExpressionEvalator evalator = new(system);

        var parsedExpression = interpreter.Parse(text);

        if (parsedExpression.HasErrors)
        {
            Assert.Inconclusive();
        }


        // Act
        // ===
        var result = parsedExpression.ParsedObject.Accept(evalator);


        // Assert
        // ======
        Assert.IsNotNull(result, "Valid expression must evaluated to non null value");

        result.Should().Be(evaluatedExpression);

    }

}