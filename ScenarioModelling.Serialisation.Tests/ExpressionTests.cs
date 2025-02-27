using FluentAssertions;
using Newtonsoft.Json;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Expressions.Evaluation;
using ScenarioModelling.CoreObjects.Expressions.Initialisation;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.Exhaustiveness.Common;
using ScenarioModelling.Serialisation.Expressions;
using ScenarioModelling.Serialisation.Expressions.Interpreter;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;
using ScenarioModelling.TestDataAndTools.Expressions;
using ScenarioModelling.Tools.GenericInterfaces;
using System.Diagnostics;

namespace ScenarioModelling.Serialisation.Tests;

[TestClass]
public class ExpressionTests
{

    [AssemblyInitialize()]
    public static void AssemblyInit(TestContext context)
    {
        ExhaustivityFunctions.Active = true;
    }

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

        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();

        scope.Context
             .UseSerialiser<CustomContextSerialiser>()
             .LoadContext(systemText)
             .Initialise();

        ExpressionInterpreter interpreter = scope.GetService<ExpressionInterpreter>();
        ExpressionInitialiser validator = scope.GetService<ExpressionInitialiser>();

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

        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();

        MetaState system =
            scope.Context
                 .UseSerialiser<CustomContextSerialiser>()
                 .LoadContext(systemText)
                 .Initialise()
                 .MetaState;

        ExpressionInterpreter interpreter = scope.GetService<ExpressionInterpreter>();
        ExpressionSerialiser visitor = scope.GetService<ExpressionSerialiser>();

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
    [TestCategory("Expressions"), TestCategory("Expression Evaluation")]
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

        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();

        scope.Context
             .UseSerialiser<CustomContextSerialiser>()
             .LoadContext(systemText)
             .Initialise();

        ExpressionInterpreter interpreter = scope.GetService<ExpressionInterpreter>();
        ExpressionInitialiser initialiser = scope.GetService<ExpressionInitialiser>();
        ExpressionEvalator evalator = scope.GetService<ExpressionEvalator>();

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