﻿using Newtonsoft.Json;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using System.Reflection;

namespace ScenarioModelling.TestDataAndTools.Expressions;

public class ExpectedValues
{
    public string Expected { get; set; }
    public bool IsValid { get; set; }
    public object? ExpectedEvaluatedValue { get; set; } = null;
    public Type? ExpectedReturnType { get; set; } = null;

    public ExpectedValues(string Expected, bool IsValid, object? ExpectedEvaluatedValue = null, Type? ExpectedReturnType = null)
    {
        this.Expected = Expected;
        this.IsValid = IsValid;
        this.ExpectedEvaluatedValue = ExpectedEvaluatedValue;
        this.ExpectedReturnType = ExpectedReturnType;
    }
}

public class ExpressionGrammarTestDataProviderAttribute : Attribute, ITestDataSource
{
    public readonly string MetaStateText = """
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
            CompositeValue { A }
            """,
            true,
            ExpectedEvaluatedValue: "A",
            ExpectedReturnType: typeof(Entity)
        ),
        new(
            "Reference to entity A's aspect D (A.D)",
            "A.D",
            """
            CompositeValue { A, D }
            """,
            true,
            ExpectedEvaluatedValue: "D",
            ExpectedReturnType: typeof(Aspect)
        ),
        new(
            "Reference to the state of entity A's aspect D (A.D.State)",
            "A.D.State",
            """
            CompositeValue { A, D, State }
            """,
            true,
            ExpectedEvaluatedValue: "DState",
            ExpectedReturnType: typeof(State)
        ),
        new(
            "A string",
            @"""A string""",
            """
            CompositeValue { A string }
            """,
            true,
            ExpectedEvaluatedValue: "A string",
            ExpectedReturnType: typeof(string)
        ),
        new(
            "And",
            @"A and B",
            """
            AndExpression { Left = CompositeValue { A }, Right = CompositeValue { B } }
            """,
            true
        ),
        new(
            "And (2)",
            @"true and false",
            """
            AndExpression { Left = CompositeValue { true }, Right = CompositeValue { false } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "Or",
            @"A or B",
            """
            OrExpression { Left = CompositeValue { A }, Right = CompositeValue { B } }
            """,
            true
        ),
        new(
            "Or (2)",
            @"true or false",
            """
            OrExpression { Left = CompositeValue { true }, Right = CompositeValue { false } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "Or and",
            @"A or B and C",
            """
            AndExpression { Left = OrExpression { Left = CompositeValue { A }, Right = CompositeValue { B } }, Right = CompositeValue { C } }
            """,
            true
        ),
        new(
            "Or and (2)",
            @"true or false and true",
            """
            AndExpression { Left = OrExpression { Left = CompositeValue { true }, Right = CompositeValue { false } }, Right = CompositeValue { true } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "And or",
            @"A and B or C",
            """
            OrExpression { Left = AndExpression { Left = CompositeValue { A }, Right = CompositeValue { B } }, Right = CompositeValue { C } }
            """,
            true
        ),
        new(
            "And or (2)",
            @"true and false or true",
            """
            OrExpression { Left = AndExpression { Left = CompositeValue { true }, Right = CompositeValue { false } }, Right = CompositeValue { true } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "==",
            @"A == B",
            """
            EqualExpression { Left = CompositeValue { A }, Right = CompositeValue { B } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "!=",
            @"A != B",
            """
            NotEqualExpression { Left = CompositeValue { A }, Right = CompositeValue { B } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "!= ==",
            @"A != B == C",
            """
            EqualExpression { Left = NotEqualExpression { Left = CompositeValue { A }, Right = CompositeValue { B } }, Right = CompositeValue { C } }
            """,
            true
        ),
        new(
            "!= == (2)",
            @"true != false == true",
            """
            EqualExpression { Left = NotEqualExpression { Left = CompositeValue { true }, Right = CompositeValue { false } }, Right = CompositeValue { true } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "== !=",
            @"A == B != C",
            """
            NotEqualExpression { Left = EqualExpression { Left = CompositeValue { A }, Right = CompositeValue { B } }, Right = CompositeValue { C } }
            """,
            true
        ),
        new(
            "== != (2)",
            @"true == false != true",
            """
            NotEqualExpression { Left = EqualExpression { Left = CompositeValue { true }, Right = CompositeValue { false } }, Right = CompositeValue { true } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "And ==",
            @"A and B == C",
            """
            AndExpression { Left = CompositeValue { A }, Right = EqualExpression { Left = CompositeValue { B }, Right = CompositeValue { C } } }
            """,
            true
        ),
        new(
            "And == (2)",
            @"true and false == true",
            """
            AndExpression { Left = CompositeValue { true }, Right = EqualExpression { Left = CompositeValue { false }, Right = CompositeValue { true } } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "== and",
            @"A == B and C",
            """
            AndExpression { Left = EqualExpression { Left = CompositeValue { A }, Right = CompositeValue { B } }, Right = CompositeValue { C } }
            """,
            true
        ),
        new(
            "== and (2)",
            @"true == false and true",
            """
            AndExpression { Left = EqualExpression { Left = CompositeValue { true }, Right = CompositeValue { false } }, Right = CompositeValue { true } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "(==) and",
            @"(A == B) and C",
            """
            AndExpression { Left = BracketsExpression { Expression = EqualExpression { Left = CompositeValue { A }, Right = CompositeValue { B } } }, Right = CompositeValue { C } }
            """,
            true
        ),
        new(
            "(==) and (2)",
            @"(A == B) and true",
            """
            AndExpression { Left = BracketsExpression { Expression = EqualExpression { Left = CompositeValue { A }, Right = CompositeValue { B } } }, Right = CompositeValue { true } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "== (and)",
            @"A == (B and C)",
            """
            EqualExpression { Left = CompositeValue { A }, Right = BracketsExpression { Expression = AndExpression { Left = CompositeValue { B }, Right = CompositeValue { C } } } }
            """,
            true
        ),
        new(
            "== (and) (2)",
            @"true == (false and true)",
            """
            EqualExpression { Left = CompositeValue { true }, Right = BracketsExpression { Expression = AndExpression { Left = CompositeValue { false }, Right = CompositeValue { true } } } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "Function not - 1 param",
            @"customfunction(A)",
            """
            FunctionExpression { Name = customfunction, Arguments = ArgumentList [ CompositeValue { A } ] }
            """,
            true
            // TODO
        ),
        new(
            "Function - 2 params",
            @"customfunction(A, B != C, A.D)",
            """
            FunctionExpression { Name = customfunction, Arguments = ArgumentList [ CompositeValue { A }, NotEqualExpression { Left = CompositeValue { B }, Right = CompositeValue { C } }, CompositeValue { A, D } ] }
            """,
            true
            // TODO
        ),
        new(
            "A related to B",
            @"A -?> B",
            """
            HasRelationExpression { Name = , Left = CompositeValue { A }, Right = CompositeValue { B } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "B related to C",
            @"B -?> C",
            """
            HasRelationExpression { Name = , Left = CompositeValue { B }, Right = CompositeValue { C } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "A related to B with named relation",
            @"A -?> B : NamedRelation",
            """
            HasRelationExpression { Name = Relation name, Left = CompositeValue { A }, Right = CompositeValue { B } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "B related to A with named relation",
            @"B -?> A : NamedRelation",
            """
            HasRelationExpression { Name = Relation name, Left = CompositeValue { B }, Right = CompositeValue { A } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "A not related to B",
            @"A -!> B",
            """
            DoesNotHaveRelationExpression { Name = , Left = CompositeValue { A }, Right = CompositeValue { B } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "B not related to A",
            @"B -!> A",
            """
            DoesNotHaveRelationExpression { Name = , Left = CompositeValue { B }, Right = CompositeValue { A } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "A not related to B with named relation",
            @"A -!> B : NamedRelation",
            """
            DoesNotHaveRelationExpression { Name = Relation name, Left = CompositeValue { A }, Right = CompositeValue { B } }
            """,
            true,
            ExpectedEvaluatedValue: true,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "B not related to A with named relation",
            @"B -!> A : NamedRelation",
            """
            DoesNotHaveRelationExpression { Name = Relation name, Left = CompositeValue { B }, Right = CompositeValue { A } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "-?> and",
            @"A -?> B and C",
            """
            AndExpression { Left = HasRelationExpression { Name = , Left = CompositeValue { A }, Right = CompositeValue { B } }, Right = CompositeValue { C } }
            """,
            true
        ),
        new(
            "-?> and (2)",
            @"A -?> B and true",
            """
            AndExpression { Left = HasRelationExpression { Name = , Left = CompositeValue { A }, Right = CompositeValue { B } }, Right = CompositeValue { true } }
            """,
            true,
            ExpectedEvaluatedValue: false,
            ExpectedReturnType: typeof(bool)
        ),
        new(
            "and -?>",
            @"A and B -?> C",
            """
            AndExpression { Left = CompositeValue { A }, Right = HasRelationExpression { Name = , Left = CompositeValue { B }, Right = CompositeValue { C } } }
            """,
            true
        ),
        new(
            "and -?> (2)",
            @"true and B -?> C",
            """
            AndExpression { Left = CompositeValue { true }, Right = HasRelationExpression { Name = , Left = CompositeValue { B }, Right = CompositeValue { C } } }
            """,
            true,
            ExpectedEvaluatedValue: true, // TODO
            ExpectedReturnType: typeof(bool)
        ),
    };

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        var finalData = TestData.Select(t => new object[] { t.Name, t.Expression, MetaStateText, JsonConvert.SerializeObject(new ExpectedValues(t.Expected, t.IsValid, t.ExpectedEvaluatedValue, t.ExpectedReturnType)) });

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

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => data?[0]?.ToString() ?? "";

}























