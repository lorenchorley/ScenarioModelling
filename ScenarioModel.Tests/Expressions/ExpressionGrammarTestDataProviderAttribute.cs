using System.Reflection;

namespace ScenarioModel.Tests;

public class ExpressionGrammarTestDataProviderAttribute : Attribute, ITestDataSource
{
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
            "A"
        ),
        new(
            "Reference to entity A's aspect D (A.D)",
            "A.D",
            """
            ValueComposite { A, D }
            """,
            true,
            ExpectedEvaluation: "A.D"
        ),
        new(
            "Reference to the state of entity A's aspect D (A.D.State)",
            "A.D.State",
            """
            ValueComposite { A, D, State }
            """,
            true,
            ExpectedEvaluation: "DState"
        ),
        new(
            "A string",
            @"""A string""",
            """
            ValueComposite { A string }
            """,
            true,
            ExpectedEvaluation: "A string"
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
            "Or",
            @"A or B",
            """
            OrExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true
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
            "And or",
            @"A and B or C",
            """
            OrExpression { Left = AndExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """,
            true
        ),
        new(
            "==",
            @"A == B",
            """
            EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluation: false
        ),
        new(
            "!=",
            @"A != B",
            """
            NotEqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluation: true
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
            "== !=",
            @"A == B != C",
            """
            NotEqualExpression { Left = EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """,
            true
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
            "== and",
            @"A == B and C",
            """
            AndExpression { Left = EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """,
            true
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
            "== (and)",
            @"A == (B and C)",
            """
            EqualExpression { Left = ValueComposite { A }, Right = BracketsExpression { Expression = AndExpression { Left = ValueComposite { B }, Right = ValueComposite { C } } } }
            """,
            true
        ),
        new(
            "Function not - 1 param",
            @"not(A)",
            """
            FunctionExpression { Name = not, Arguments = ArgumentList [ ValueComposite { A } ] }
            """,
            true
        ),
        new(
            "Function - 2 params",
            @"fn(A, B != C, A.D)",
            """
            FunctionExpression { Name = fn, Arguments = ArgumentList [ ValueComposite { A }, NotEqualExpression { Left = ValueComposite { B }, Right = ValueComposite { C } }, ValueComposite { A, D } ] }
            """,
            true
        ),
        new(
            "Related",
            @"A -?> B",
            """
            HasRelationExpression { Name = , Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluation: false
        ),
        new(
            "Related with named relation",
            @"A -?> B : ""Relation name""",
            """
            HasRelationExpression { Name = Relation name, Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluation: false
        ),
        new(
            "Not related",
            @"A -!> B",
            """
            DoesNotHaveRelationExpression { Name = , Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluation: true
        ),
        new(
            "Not related with named relation",
            @"A -!> B : ""Relation name""",
            """
            DoesNotHaveRelationExpression { Name = Relation name, Left = ValueComposite { A }, Right = ValueComposite { B } }
            """,
            true,
            ExpectedEvaluation: false
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
            "and -?>",
            @"A and B -?> C",
            """
            AndExpression { Left = ValueComposite { A }, Right = HasRelationExpression { Name = , Left = ValueComposite { B }, Right = ValueComposite { C } } }
            """,
            true
        ),
    };

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => TestData.Select(t => new object[] { t.Name, t.Expression, t.Expected, t.IsValid, t.ExpectedEvaluation });

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
        => data?[0]?.ToString() ?? "";

}