using FluentAssertions;
using Newtonsoft.Json;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Expressions.Validation;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using System.Diagnostics;

namespace ScenarioModel.Tests.Expressions;

[TestClass]
public class ExpressionTests
{
    [TestMethod]
    [TestCategory("Expressions"), TestCategory("Grammar")]
    [ExpressionGrammarTestDataProvider]
    public void ExpressionGrammarTests(string name, string text, string systemText, string expectedValuesJson)
    {
        // Arrange 
        // =======
        ExpectedValues expectedValues = JsonConvert.DeserializeObject<ExpectedValues>(expectedValuesJson);

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

        //serialisedResult.Should().Be(expectedValues.Expected);
    }

    [DataTestMethod]
    [TestCategory("Expressions"), TestCategory("Grammar")]
    [ExpressionGrammarTestDataProvider]
    public void ExpressionValidationTests(string name, string text, string systemText, string expectedValuesJson)
    {
        // Arrange 
        // =======
        ExpectedValues expectedValues = JsonConvert.DeserializeObject<ExpectedValues>(expectedValuesJson);

        if (expectedValues.ExpectedEvaluatedValue == null && expectedValues.ExpectedReturnType == null)
        {
            Assert.Inconclusive();
        }

        ExpressionInterpreter interpreter = new();

        System system =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .LoadContext(systemText)
                   .Initialise()
                   .System;

        ExpressionInitialiser validator = new(system);

        var parsedExpression = interpreter.Parse(text);


        // Act
        // ===
        parsedExpression.ParsedObject.Accept(validator);


        // Assert
        // ======
        Assert.AreEqual(expectedValues.IsValid, validator.Errors.Count == 0, validator.Errors.CommaSeparatedList());
    }

    [DataTestMethod]
    [TestCategory("Expressions"), TestCategory("Serialisation")]
    [ExpressionGrammarTestDataProvider]
    public void ExpressionSerialisationTests(string name, string text, string systemText, string expectedValuesJson)
    {
        // Arrange 
        // =======
        ExpectedValues expectedValues = JsonConvert.DeserializeObject<ExpectedValues>(expectedValuesJson);

        ExpressionInterpreter interpreter = new();

        System system =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .LoadContext(systemText)
                   .Initialise()
                   .System;

        ExpressionSerialiser visitor = new(system);

        var parsedExpression = interpreter.Parse(text);

        if (parsedExpression.HasErrors)
        {
            Assert.Inconclusive();
        }


        // Act
        // ===
        var result = parsedExpression.ParsedObject!.Accept(visitor);


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
    public void ExpressionEvaluationTests(string name, string text, string systemText, string expectedValuesJson)
    {
        // Arrange 
        // =======
        ExpectedValues expectedValues = JsonConvert.DeserializeObject<ExpectedValues>(expectedValuesJson);

        if (expectedValues.ExpectedEvaluatedValue == null && expectedValues.ExpectedReturnType == null)
        {
            Assert.Inconclusive();
        }

        ExpressionInterpreter interpreter = new();

        System system =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .LoadContext(systemText)
                   .Initialise()
                   .System;

        ExpressionInitialiser initialiser = new(system);
        ExpressionEvalator evalator = new(system);

        var parsedExpression = interpreter.Parse(text);

        if (parsedExpression.HasErrors)
        {
            Assert.Inconclusive();
        }


        // Act
        // ===
        parsedExpression.ParsedObject.Accept(initialiser); // Needed to set return types
        var result = parsedExpression.ParsedObject!.Accept(evalator);


        // Assert
        // ======
        Assert.IsNotNull(result, "Valid expression must evaluated to non null value");

        parsedExpression.ParsedObject.Type.Should().Be(expectedValues.ExpectedReturnType);

        if (result is IIdentifiable nameful)
        {
            nameful.Name.Should().Be((string)expectedValues.ExpectedEvaluatedValue);
        }
        else if (result is Aspect aspect)
        {
            aspect.Name.Should().Be((string)expectedValues.ExpectedEvaluatedValue);
        }
        else
        {
            result.Should().Be(expectedValues.ExpectedEvaluatedValue);
        }

    }

}