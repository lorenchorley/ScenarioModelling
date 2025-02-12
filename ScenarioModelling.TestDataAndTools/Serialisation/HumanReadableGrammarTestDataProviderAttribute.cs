using System.Reflection;

namespace ScenarioModelling.TestDataAndTools.Serialisation;

public class HumanReadableGrammarTestDataProviderAttribute : Attribute, ITestDataSource
{
    public List<HumanReadableGrammarTestData> TestData = new()
    {
        new(
            "Empty",
            "",
            """
            
            """
        ),
        new(
            "Empty lines",
            """
              
              

            """,
            """
            
            """
        ),
        new(
            "One entity",
            """
            Entity "E1" {}
            """ ,
            """
            Definition\(Entity, E1\)
            """
        ),
        new(
            "Two related entities",
            """
            E1 -> E2
            Entity E1
            Entity E2
            """ ,
            """
            Link\(E1 -> E2\) Definition\(Entity, E1\) Definition\(Entity, E2\)
            """
        ),
        new(
            "Entity without name",
            """
            Entity {}
            """ ,
            """
            Definition\(Entity, _\)
            """
        ),
        new(
            "Entity named without quotes",
            """
            Entity E1 {}
            """ ,
            """
            Definition\(Entity, E1\)
            """
        ),
        new(
            "Entity named without braces",
            """
            Entity "E1"
            """ ,
            """
            Definition\(Entity, E1\)
            """
        ),
        new(
            "Entity without any details",
            """
            Entity
            """ ,
            """
            Definition\(Entity, _\)
            """
        ),
        new(
            "Two entities",
            """
            Entity "E1" {}
            Entity "E2" {}
            """ ,
            """
            Definition\(Entity, E1\) Definition\(Entity, E2\)
            """
        ),
        new(
            "Entity with aspect",
            """
            Entity "E1" 
            {
                Aspect "A1"
            }
            """,
            """
            Definition\(Entity, E1\) { Definition\(Aspect, A1\) }
            """
        ),
        new(
            "Link",
            """
            E1 -> E2
            """,
            """
            Link\(E1 -> E2\)
            """
        ),
        new(
            "Named Link",
            """
            E1 -> E2 : Link1
            """,
            """
            Link\(E1 -> E2, Link1\)
            """
        ),
        new(
            "Two links",
            """
            E1 -> E2
            E2 -> E1
            """,
            """
            Link\(E1 -> E2\) Link\(E2 -> E1\)
            """
        ),
        new(
            "Two link with strings for names",
            """
            "Entity 1" -> E2
            E2 -> "Entity 2"
            """,
            """
            Link\(Entity 1 -> E2\) Link\(E2 -> Entity 2\)
            """
        ),
        new(
            "Empty state machine",
            """
            StateMachine "ST1" 
            {
                
            }
            """,
            """
            Definition\(StateMachine, ST1\)
            """
        ),
        new(
            "State machine with two states and a transition",
            """
            StateMachine "ST1" 
            {
                S1 -> S2
            }
            """,
            """
            Definition\(StateMachine, ST1\) { Link\(S1 -> S2\) }
            """
        ),
        new(
            "Whitespace and new lines",
            """
            
              
                
            a
            a

            a b
            a b

            a b {} 

            a b { } 

            a b { 
            }

            a b 
            { 
            }

            a b 
            { }

            
            a b 

             {
            
             }

            """,
            """
            Definition\(a, _\) Definition\(a, _\) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\)
            """
        ),
    };

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => TestData.Select(t => new object[] { t.Name, t.Expression, t.ExpectedResultRegex });

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => data?[0]?.ToString() ?? "";

}