using System.Reflection;

namespace ScenarioModel.Tests;

public class GrammarTestDataProviderAttribute : Attribute, ITestDataSource
{
    public List<(string name, string expression, string expectedResult)> TestData = new()
    {
        (
            "Empty",
            """

            """,
            ""
        ),
        (
            "One entity",
            """
            Entity "E1" {}
            """ ,
            @"Definition(Entity, ""E1"")"
        ),
        (
            "Entity without name",
            """
            Entity {}
            """ ,
            @"Definition(Entity, _)"
        ),
        (
            "Entity named without quotes",
            """
            Entity E1 {}
            """ ,
            @"Definition(Entity, E1)"
        ),
        (
            "Entity named without braces",
            """
            Entity "E1"
            """ ,
            @"Definition(Entity, ""E1"")"
        ),
        (
            "Entity without any details",
            """
            Entity
            """ ,
            """
            Definition(Entity, )
            """
        ),
        (
            "Two entities",
            """
            Entity "E1" {}
            Entity "E2" {}
            """ ,
            """
            Definition(Entity, "E1") Definition(Entity, "E2")
            """
        ),
        (
            "Entity with aspect",
            """
            Entity "E1" 
            {
                Aspect "A1"
            }
            """,
            """
            Definition(Entity, "E1") { Definition(Aspect, "A1") }
            """
        ),
        (
            "Link",
            """
            E1 -> E2
            """,
            """
            Link(E1 -> E2)
            """
        ),
        (
            "Named Link",
            """
            E1 -> E2 : Link1
            """,
            """
            Link(E1 -> E2, Link1)
            """
        ),
        (
            "Two links",
            """
            E1 -> E2
            E2 -> E1
            """,
            """
            Link(E1 -> E2) Link(E2 -> E1)
            """
        ),
        (
            "Two link with strings for names",
            """
            "Entity 1" -> E2
            E2 -> "Entity 2"
            """,
            """
            Link("Entity 1" -> E2) Link(E2 -> "Entity 2")
            """
        ),
        (
            "Empty StateType",
            """
            StateType "ST1" 
            {
                
            }
            """,
            """
            Definition(StateType, "ST1")
            """
        ),
        (
            "StateType with two states and a transition",
            """
            StateType "ST1" 
            {
                S1 -> S2
            }
            """,
            """
            Definition(StateType, "ST1") { Link(S1 -> S2) }
            """
        ),
    };

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => TestData.Select(t => new object[] { t.name, t.expression, t.expectedResult });

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
        => data?[0]?.ToString() ?? "";

}