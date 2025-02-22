using System.Reflection;

namespace ScenarioModelling.TestDataAndTools.Serialisation;

public class CustomSerialiserGrammarTestDataProviderAttribute : Attribute, ITestDataSource
{
    public List<CustomSerialiserGrammarTestData> TestData = new()
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
            UnnamedLinkDefinition\(E1 -> E2\) NamedDefinition\(Entity, E1\) NamedDefinition\(Entity, E2\)
            """
        ),
        new(
            "Entity without name",
            """
            Entity {}
            """ ,
            """
            UnnamedDefinition\(Entity\)
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
            UnnamedDefinition\(Entity\)
            """
        ),
        new(
            "Two entities",
            """
            Entity "E1" {}
            Entity "E2" {}
            """ ,
            """
            NamedDefinition\(Entity, E1\) NamedDefinition\(Entity, E2\)
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
            NamedDefinition\(Entity, E1\) { NamedDefinition\(Aspect, A1\) }
            """
        ),
        new(
            "Link",
            """
            E1 -> E2
            """,
            """
            UnnamedLinkDefinition\(E1 -> E2\)
            """
        ),
        new(
            "Named Link",
            """
            E1 -> E2 : Link1
            """,
            """
            NamedLinkDefinition\(E1 -> E2, Link1\)
            """
        ),
        new(
            "Two links",
            """
            E1 -> E2
            E2 -> E1
            """,
            """
            UnnamedLinkDefinition\(E1 -> E2\) UnnamedLinkDefinition\(E2 -> E1\)
            """
        ),
        new(
            "Two link with strings for names",
            """
            "Entity 1" -> E2
            E2 -> "Entity 2"
            """,
            """
            UnnamedLinkDefinition\(Entity 1 -> E2\) UnnamedLinkDefinition\(E2 -> Entity 2\)
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
            NamedDefinition\(StateMachine, ST1\) { UnnamedLinkDefinition\(S1 -> S2\) }
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
            UnnamedDefinition\(a\) UnnamedDefinition\(a\) NamedDefinition\(a, b\) NamedDefinition\(a, b\) NamedDefinition\(a, b\) NamedDefinition\(a, b\) NamedDefinition\(a, b\) NamedDefinition\(a, b\) NamedDefinition\(a, b\) NamedDefinition\(a, b\)
            """
        ),
    };

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => TestData.Select(t => new object[] { t.Name, t.Expression, t.ExpectedResultRegex });

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => data?[0]?.ToString() ?? "";

}