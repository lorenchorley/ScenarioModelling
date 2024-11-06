using Newtonsoft.Json;
using ScenarioModel.Expressions.Evaluation;
using System.Reflection;

namespace ScenarioModel.Tests;

public class ExpectedValues
{
    public string Expected { get; set; }
    public bool IsValid { get; set; }
    public object? ExpectedEvaluatedValue { get; set; } = null;
    public ExpressionValueType? ExpectedReturnType { get; set; } = null;

    public ExpectedValues(string Expected, bool IsValid, object? ExpectedEvaluatedValue = null, ExpressionValueType? ExpectedReturnType = null)
    {
        this.Expected = Expected;
        this.IsValid = IsValid;
        this.ExpectedEvaluatedValue = ExpectedEvaluatedValue;
        this.ExpectedReturnType = ExpectedReturnType;
    }
}

public class ExpressionGrammarTestDataProviderAttribute : Attribute, ITestDataSource
{
    public readonly string SystemText = """
        Entity A
        {
            Aspect D {
                State DState
            }
        }
        Entity B
        Entity C
        B -> A : NamedRelation
        B -> C

        """;

    public List<ExpressionGrammarTestData> TestData = new()
    {
        new(
            "Empty",
            "",
            """
            EmptyExpression { }
            """,
            false
        ),
        new(
            "Reference to entity A",
            "A",
            """
            ValueComposite { A }
            """,
            true,
            ExpectedEvaluatedValue: "A",
            ExpectedReturnType: ExpressionValueType.Entity
        ),
        new(
            "Reference to entity A's aspect D (A.D)",
            "A.D",
            """
            ValueComposite { A, D }
            """,
            true,
            ExpectedEvaluatedValue: "D",
            ExpectedReturnType: ExpressionValueType.Aspect
        ),
        new(
            "Reference to the state of entity A's aspect D (A.D.State)",
            "A.D.State",
            """
            ValueComposite { A, D, State }
            """,
            true,
            ExpectedEvaluatedValue: "DState",
            ExpectedReturnType: ExpressionValueType.State
        ),
        new(
            "A string",
            @"""A string""",
            """
            ValueComposite { A string }
            """,
            true,
            ExpectedEvaluatedValue: "A string",
            ExpectedReturnType: ExpressionValueType.String
        ),
        new(
            "And",
            @"A and B",
            """
            AndExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true
        ),
        new(
            "And (2)",
            @"true and false",
            """
            AndExpression { Left = ValueComposite { true }, Right = ValueComposite { false } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "Or",
            @"A or B",
            """
            OrExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true
        ),
        new(
            "Or (2)",
            @"true or false",
            """
            OrExpression { Left = ValueComposite { true }, Right = ValueComposite { false } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "Or and",
            @"A or B and C",
            """
            AndExpression { Left = OrExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """,
            true
        ),
        new(
            "Or and (2)",
            @"true or false and true",
            """
            AndExpression { Left = OrExpression { Left = ValueComposite { true }, Right = ValueComposite { false } }, Right = ValueComposite { true } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "And or",
            @"A and B or C",
            """
            OrExpression { Left = AndExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """,
            true
        ),
        new(
            "And or (2)",
            @"true and false or true",
            """
            OrExpression { Left = AndExpression { Left = ValueComposite { true }, Right = ValueComposite { false } }, Right = ValueComposite { true } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "==",
            @"A == B",
            """
            EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "!=",
            @"A != B",
            """
            NotEqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "!= ==",
            @"A != B == C",
            """
            EqualExpression { Left = NotEqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """,
            true
        ),
        new(
            "!= == (2)",
            @"true != false == true",
            """
            EqualExpression { Left = NotEqualExpression { Left = ValueComposite { true }, Right = ValueComposite { false } }, Right = ValueComposite { true } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "== !=",
            @"A == B != C",
            """
            NotEqualExpression { Left = EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """,
            true
        ),
        new(
            "== != (2)",
            @"true == false != true",
            """
            NotEqualExpression { Left = EqualExpression { Left = ValueComposite { true }, Right = ValueComposite { false } }, Right = ValueComposite { true } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "And ==",
            @"A and B == C",
            """
            AndExpression { Left = ValueComposite { A }, Right = EqualExpression { Left = ValueComposite { B }, Right = ValueComposite { C } } }
            """,
            true
        ),
        new(
            "And == (2)",
            @"true and false == true",
            """
            AndExpression { Left = ValueComposite { true }, Right = EqualExpression { Left = ValueComposite { false }, Right = ValueComposite { true } } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "== and",
            @"A == B and C",
            """
            AndExpression { Left = EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """,
            true
        ),
        new(
            "== and (2)",
            @"true == false and true",
            """
            AndExpression { Left = EqualExpression { Left = ValueComposite { true }, Right = ValueComposite { false } }, Right = ValueComposite { true } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "(==) and",
            @"(A == B) and C",
            """
            AndExpression { Left = BracketsExpression { Expression = EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } } }, Right = ValueComposite { C } }
            """,
            true
        ),
        new(
            "(==) and (2)",
            @"(A == B) and true",
            """
            AndExpression { Left = BracketsExpression { Expression = EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } } }, Right = ValueComposite { true } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "== (and)",
            @"A == (B and C)",
            """
            EqualExpression { Left = ValueComposite { A }, Right = BracketsExpression { Expression = AndExpression { Left = ValueComposite { B }, Right = ValueComposite { C } } } }
            """,
            true
        ),
        new(
            "== (and) (2)",
            @"true == (false and true)",
            """
            EqualExpression { Left = ValueComposite { true }, Right = BracketsExpression { Expression = AndExpression { Left = ValueComposite { false }, Right = ValueComposite { true } } } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "Function not - 1 param",
            @"customfunction(A)",
            """
            FunctionExpression { Name = customfunction, Arguments = ArgumentList [ ValueComposite { A } ] }
            """,
            true
            // TODO
        ),
        new(
            "Function - 2 params",
            @"customfunction(A, B != C, A.D)",
            """
            FunctionExpression { Name = customfunction, Arguments = ArgumentList [ ValueComposite { A }, NotEqualExpression { Left = ValueComposite { B }, Right = ValueComposite { C } }, ValueComposite { A, D } ] }
            """,
            true
            // TODO
        ),
        new(
            "A related to B",
            @"A -?> B",
            """
            HasRelationExpression { Name = , Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "B related to C",
            @"B -?> C",
            """
            HasRelationExpression { Name = , Left = ValueComposite { B }, Right = ValueComposite { C } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "A related to B with named relation",
            @"A -?> B : ""Relation name""",
            """
            HasRelationExpression { Name = Relation name, Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "B related to A with named relation",
            @"A -?> B : ""Relation name""",
            """
            HasRelationExpression { Name = Relation name, Left = ValueComposite { B }, Right = ValueComposite { A } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "A not related to B",
            @"A -!> B",
            """
            DoesNotHaveRelationExpression { Name = , Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "B not related to A",
            @"B -!> A",
            """
            DoesNotHaveRelationExpression { Name = , Left = ValueComposite { B }, Right = ValueComposite { A } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "A not related to B with named relation",
            @"A -!> B : ""Relation name""",
            """
            DoesNotHaveRelationExpression { Name = Relation name, Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "B not related to A with named relation",
            @"B -!> A : ""Relation name""",
            """
            DoesNotHaveRelationExpression { Name = Relation name, Left = ValueComposite { B }, Right = ValueComposite { A } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "-?> and",
            @"A -?> B and C",
            """
            AndExpression { Left = HasRelationExpression { Name = , Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """,
            true
        ),
        new(
            "-?> and (2)",
            @"A -?> B and true",
            """
            AndExpression { Left = HasRelationExpression { Name = , Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { true } }
            """,
            true,
            ExpectedEvaluatedValue: false, // TODO
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
        new(
            "and -?>",
            @"A and B -?> C",
            """
            AndExpression { Left = ValueComposite { A }, Right = HasRelationExpression { Name = , Left = ValueComposite { B }, Right = ValueComposite { C } } }
            """,
            true
        ),
        new(
            "and -?> (2)",
            @"true and B -?> C",
            """
            AndExpression { Left = ValueComposite { true }, Right = HasRelationExpression { Name = , Left = ValueComposite { B }, Right = ValueComposite { C } } }
            """,
            true,
            ExpectedEvaluatedValue: false, // TODO
            ExpectedReturnType: ExpressionValueType.Boolean
        ),
    };

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        var finalData = TestData.Select(t => new object[] { t.Name, t.Expression, SystemText, JsonConvert.SerializeObject(new ExpectedValues(t.Expected, t.IsValid, t.ExpectedEvaluatedValue, t.ExpectedReturnType)) });

        var groupedByNameWithMultipleNames =
            finalData.GroupBy(t => GetDisplayName(methodInfo, t))
                     .Where(g => g.Count() > 1)
                     .Select(g => g.Key);

        if (groupedByNameWithMultipleNames.Any())
        {
            throw new InvalidOperationException($"Duplicate names found: {groupedByNameWithMultipleNames.Select(n => $"'{n}'").CommaSeparatedList()}");
        }

        return finalData;
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
        => data?[0]?.ToString() ?? "";

}























