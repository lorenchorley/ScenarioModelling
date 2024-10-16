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
            """
        ),
        new(
            "One object",
            "E1",
            """
            ValueComposite { E1 }
            """
        ),
        new(
            "Subproperty of object",
            "E1.A1",
            """
            ValueComposite { E1, A1 }
            """
        ),
        new(
            "Subsubproperty of object",
            "E1.A1.State",
            """
            ValueComposite { E1, A1, State }
            """
        ),
        new(
            "Subsubsubproperty of object",
            "E1.A1.State.Other",
            """
            ValueComposite { E1, A1, State, Other }
            """
        ),
        new(
            "A string",
            @"""A string""",
            """
            ValueComposite { A string }
            """
        ),
        new(
            "And",
            @"A and B",
            """
            AndExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """
        ),
        new(
            "Or",
            @"A or B",
            """
            OrExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """
        ),
        new(
            "Or and",
            @"A or B and C",
            """
            AndExpression { Left = OrExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """
        ),
        new(
            "And or",
            @"A and B or C",
            """
            OrExpression { Left = AndExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """
        ),
        new(
            "==",
            @"A == B",
            """
            EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """
        ),
        new(
            "!=",
            @"A != B",
            """
            NotEqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """
        ),
        new(
            "<>",
            @"A <> B",
            """
            NotEqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }
            """
        ),
        new(
            "!= ==",
            @"A != B == C",
            """
            EqualExpression { Left = NotEqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """
        ),
        new(
            "== !=",
            @"A == B != C",
            """
            NotEqualExpression { Left = EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """
        ),
        new(
            "And ==",
            @"A and B == C",
            """
            AndExpression { Left = ValueComposite { A }, Right = EqualExpression { Left = ValueComposite { B }, Right = ValueComposite { C } } }
            """
        ),
        new(
            "== and",
            @"A == B and C",
            """
            AndExpression { Left = EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """
        ),
        new(
            "(==) and",
            @"(A == B) and C",
            """
            AndExpression { Left = EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """
        ),
        new(
            "== (and)",
            @"(A == B) and C",
            """
            AndExpression { Left = EqualExpression { Left = ValueComposite { A }, Right = ValueComposite { B } }, Right = ValueComposite { C } }
            """
        ),
        new(
            "Function not - 1 param",
            @"not(A)",
            """
            FunctionExpression { Name = not, Arguments = ArgumentList [ ValueComposite { A } ] }
            """
        ),
        new(
            "Function - 2 params",
            @"not(A, B != C)",
            """
            FunctionExpression { Name = not, Arguments = ArgumentList [ ValueComposite { A }, NotEqualExpression { Left = ValueComposite { B }, Right = ValueComposite { C } } ] }
            """
        ),
        new(
            "Related",
            @"A -> B",
            """
            HasRelationExpression { Name = , Left = A, Right = B }
            """
        ),
        new(
            "Related with named relation",
            @"A -> B : ""Relation name""",
            """
            HasRelationExpression { Name = Relation name, Left = A, Right = B }
            """
        ),
        new(
            "Not related",
            @"A -!> B",
            """
            DoesNotHaveRelationExpression { Name = , Left = A, Right = B }
            """
        ),
        new(
            "Not related with named relation",
            @"A -!> B : ""Relation name""",
            """
            DoesNotHaveRelationExpression { Name = Relation name, Left = A, Right = B }
            """
        ),
    };

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => TestData.Select(t => new object[] { t.Name, t.Expression, t.ExpectedResultRegex });

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
        => data?[0]?.ToString() ?? "";

}