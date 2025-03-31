using Microsoft.Z3;
using System.Reflection;
using static ScenarioModelling.Analysis.Tests.Z3.MicrosoftZ3Examples;

namespace ScenarioModelling.Analysis.Tests.Z3;

/// <summary>
/// https://github.com/Z3Prover/z3
/// https://smt.st/SAT_SMT_by_example.pdf
/// https://github.com/Z3Prover/z3/wiki/Slides
/// https://fstar-lang.org/tutorial/book/under_the_hood/uth_smt.html#understanding-how-f-uses-z3
/// https://z3prover.github.io/papers/z3internals.html
/// https://z3prover.github.io/papers/programmingz3.html
/// </summary>
[TestClass]
public sealed class MicrosoftZ3ExamplesTests
{
    private static new Dictionary<string, string> ModelGenerationSettings = new Dictionary<string, string>() { { "model", "true" } };
    public static IEnumerable<object[]> ModelGenerationExamples
        => [
            ["BasicTests"],
            ["CastingTest"],
            ["SudokuExample"],
            ["QuantifierExample1"],
            ["QuantifierExample2"],
            ["LogicExample"],
            ["ParOrExample"],
            ["FindModelExample1"],
            ["FindModelExample2"],
            ["PushPopExample1"],
            ["ArrayExample1"],
            ["ArrayExample3"],
            ["BitvectorExample1"],
            ["BitvectorExample2"],
            ["ParserExample1"],
            ["ParserExample2"],
            ["ParserExample5"],
            ["ITEExample"],
            ["EvalExample1"],
            ["EvalExample2"],
            ["FindSmallModelExample"],
            ["SimplifierExample"],
            ["FiniteDomainExample"],
            ["FloatingPointExample1"],
            ["FloatingPointExample2"]
        ];

    private static new Dictionary<string, string> ProofGenerationSettings = new Dictionary<string, string>() { { "proof", "true" } };
    public static IEnumerable<object[]> ProofGenerationExamples
        => [
            ["ProveExample1"],
            ["ProveExample2"],
            ["ArrayExample2"],
            ["TupleExample"],
            ["ParserExample3"],
            ["EnumExample"],
            ["ListExample"],
            ["TreeExample"],
            ["ForestExample"],
            ["UnsatCoreAndProofExample"],
            ["UnsatCoreAndProofExample2"]
        ];

    private static new Dictionary<string, string> ProofGenerationAndAutoConfigSettings = new Dictionary<string, string>() { { "proof", "true" }, { "auto-config", "false" } };
    public static IEnumerable<object[]> ProofGenerationAndAutoConfigExamples
      => [
            ["QuantifierExample3"],
            ["QuantifierExample4"]
        ];

    public static IEnumerable<object[]> NonParameterisedExamples
        => [
            ["SimpleExample"],
            ["TranslationExample"]
        ];

    [DataTestMethod]
    [TestCategory("Z3")]
    [DynamicData(nameof(ModelGenerationExamples), DynamicDataSourceType.Property)]
    [DoNotParallelize] // Add this attribute to ensure the tests run sequentially
    public void ModelGenerationTests(string methodName)
    {
        try
        {
            Global.ToggleWarningMessages(true);
            Log.Open("test.log");

            MethodInfo method = typeof(MicrosoftZ3Examples).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            // These examples need model generation turned on.
            using (Context ctx = new Context(ModelGenerationSettings))
            {
                method.Invoke(null, [ctx]);
            }

            Log.Close();
            if (Log.isOpen())
                Console.WriteLine("Log is still open!");
        }
        catch (Z3Exception ex)
        {
            Console.WriteLine("Z3 Managed Exception: " + ex.Message);
            Console.WriteLine("Stack trace: " + ex.StackTrace);
        }
        catch (TestFailedException ex)
        {
            Console.WriteLine("TEST CASE FAILED: " + ex.Message);
        }
    }

    [DataTestMethod]
    [TestCategory("Z3")]
    [DynamicData(nameof(ProofGenerationExamples), DynamicDataSourceType.Property)]
    [DoNotParallelize] // Add this attribute to ensure the tests run sequentially
    public void ProofGenerationTests(string methodName)
    {
        try
        {
            Global.ToggleWarningMessages(true);
            Log.Open("test.log");

            MethodInfo method = typeof(MicrosoftZ3Examples).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            // These examples need model generation turned on.
            using (Context ctx = new Context(ProofGenerationSettings))
            {
                method.Invoke(null, [ctx]);
            }

            Log.Close();
            if (Log.isOpen())
                Console.WriteLine("Log is still open!");
        }
        catch (Z3Exception ex)
        {
            Console.WriteLine("Z3 Managed Exception: " + ex.Message);
            Console.WriteLine("Stack trace: " + ex.StackTrace);
        }
        catch (TestFailedException ex)
        {
            Console.WriteLine("TEST CASE FAILED: " + ex.Message);
        }
    }

    [DataTestMethod]
    [TestCategory("Z3")]
    [DynamicData(nameof(ProofGenerationAndAutoConfigExamples), DynamicDataSourceType.Property)]
    [DoNotParallelize] // Add this attribute to ensure the tests run sequentially
    public void ProofGenerationAndAutoConfigTests(string methodName)
    {
        try
        {
            Global.ToggleWarningMessages(true);
            Log.Open("test.log");

            MethodInfo method = typeof(MicrosoftZ3Examples).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            // These examples need model generation turned on.
            using (Context ctx = new Context(ProofGenerationAndAutoConfigSettings))
            {
                method.Invoke(null, [ctx]);
            }

            Log.Close();
            if (Log.isOpen())
                Console.WriteLine("Log is still open!");
        }
        catch (Z3Exception ex)
        {
            Console.WriteLine("Z3 Managed Exception: " + ex.Message);
            Console.WriteLine("Stack trace: " + ex.StackTrace);
        }
        catch (TestFailedException ex)
        {
            Console.WriteLine("TEST CASE FAILED: " + ex.Message);
        }
    }

    [DataTestMethod]
    [TestCategory("Z3")]
    [DynamicData(nameof(NonParameterisedExamples), DynamicDataSourceType.Property)]
    [DoNotParallelize] // Add this attribute to ensure the tests run sequentially
    public void NonParameterisedTests(string methodName)
    {
        try
        {
            Global.ToggleWarningMessages(true);
            Log.Open("test.log");

            MethodInfo method = typeof(MicrosoftZ3Examples).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            // These examples need model generation turned on.
            method.Invoke(null, []);

            Log.Close();
            if (Log.isOpen())
                Console.WriteLine("Log is still open!");
        }
        catch (Z3Exception ex)
        {
            Console.WriteLine("Z3 Managed Exception: " + ex.Message);
            Console.WriteLine("Stack trace: " + ex.StackTrace);
        }
        catch (TestFailedException ex)
        {
            Console.WriteLine("TEST CASE FAILED: " + ex.Message);
        }
    }
}
