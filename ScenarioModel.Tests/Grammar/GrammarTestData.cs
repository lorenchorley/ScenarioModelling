using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ScenarioModel.Tests;

public record GrammarTestData(string Name, string Expression, [StringSyntax(StringSyntaxAttribute.Regex)] string ExpectedResultRegex) { }

public class GrammarTestDataProviderAttribute : Attribute, ITestDataSource
{
    public List<GrammarTestData> TestData = new()
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
            Definition\(Entity, \)
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
            "Empty StateType",
            """
            StateType "ST1" 
            {
                
            }
            """,
            """
            Definition\(StateType, ST1\)
            """
        ),
        new(
            "StateType with two states and a transition",
            """
            StateType "ST1" 
            {
                S1 -> S2
            }
            """,
            """
            Definition\(StateType, ST1\) { Link\(S1 -> S2\) }
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
            Definition\(a, \) Definition\(a, \) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\) Definition\(a, b\)
            """
        ),
    };

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => TestData.Select(t => new object[] { t.Name, t.Expression, t.ExpectedResultRegex });

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
        => data?[0]?.ToString() ?? "";

}