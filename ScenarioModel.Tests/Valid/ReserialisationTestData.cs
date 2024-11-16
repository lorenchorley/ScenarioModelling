namespace ScenarioModel.Tests.Valid;

public interface IContextReserialisationTestCase
{
    public string Name { get; }
}

public record CompleteTestCase(string Name, string text) : IContextReserialisationTestCase;
public record IncompleteTestCase(string Name, string originalText, string expectedFinalText) : IContextReserialisationTestCase;

public static class ReserialisationTestData
{
    public static IEnumerable<IContextReserialisationTestCase> GetTestCases()
    {
        yield return new CompleteTestCase(
            "Entity with type",
            """
            Entity E1 {
              EntityType EntityType1
            }
            
            EntityType EntityType1 {
            }
            """);

        yield return new CompleteTestCase(
            Name: "Entity without type",
            text:
            """
            Entity E1 {
            }
            """
            );

        yield return new CompleteTestCase(
            Name: "State machine",
            """
            SM SM1 {
            }
            """);

        yield return new IncompleteTestCase(
            Name: "State machine without name",
            originalText:
            """
            SM {
            }
            """,
            expectedFinalText:
            """
            SM StateMachine1 {
            }
            """
            );

        yield return new CompleteTestCase(
            Name: "Entity with type and state machine",
            """
            Entity E1 {
              EntityType EntityType1
            }

            EntityType EntityType1 {
              SM SM1
            }

            SM SM1 {
            }
            """);
        
        yield return new CompleteTestCase(
            Name: "Entity with state but no other direct link to state machine",
            text: """
            Entity E1 {
              State S1
            }

            SM SM1 {
              State S1
            }
            """);
        
        //yield return new IncompleteTestCase(
        //    Name: "Entity with state but no other direct link to state machine",
        //    originalText: """
        //    Entity E1 {
        //      State S1
        //    }

        //    SM SM1 {
        //      State S1
        //    }
        //    """,
        //    expectedFinalText: """
        //    Entity E1 {
        //      EntityType EntityType1
        //      State S1
        //    }

        //    EntityType EntityType1 {
        //      SM SM1
        //    }

        //    SM SM1 {
        //      State S1
        //    }
        //    """);

        yield return new CompleteTestCase(
            Name: "Entity with state, and type with state machine",
            """
            Entity E1 {
              EntityType EntityType1
              State S1
            }

            EntityType EntityType1 {
              SM SM1
            }

            SM SM1 {
              State S1
            }
            """);

        yield return new CompleteTestCase(
            Name: "Entity with state and type, and state machine with two states",
            """
            Entity E1 {
              EntityType EntityType1
              State S1
            }

            EntityType EntityType1 {
              SM SM1
            }

            SM SM1 {
              State S1
              State S2
            }
            """);

        yield return new CompleteTestCase(
            Name: "Entity with state and type, and state machine with two states and a transition",
            """
            Entity E1 {
              EntityType EntityType1
              State S1
            }

            EntityType EntityType1 {
              SM SM1
            }

            SM SM1 {
              State S1
              State S2
              S1 -> S2
            }
            """);

        yield return new CompleteTestCase(
            Name: "Entity with state and type, and state machine with two states and a named transition",
            """
            Entity E1 {
              EntityType EntityType1
              State S1
            }

            EntityType EntityType1 {
              SM SM1
            }

            SM SM1 {
              State S1
              State S2
              S1 -> S2 : T1
            }
            """);

        yield return new CompleteTestCase(
            Name: "State machine with two states and a transition",
            """
            SM SM1 {
              State S1
              State S2
              S1 -> S2
            }
            """);
        
        yield return new IncompleteTestCase(
            Name: "State machine with a transition",
            originalText: """
            SM SM1 {
              S1 -> S2
            }
            """,
            expectedFinalText: """
            SM SM1 {
              State S1
              State S2
              S1 -> S2
            }
            """);
        
        yield return new IncompleteTestCase(
            Name: "State machine with a transition and only one explicit state",
            originalText: """
            SM SM1 {
              State S2
              S1 -> S2
            }
            """,
            expectedFinalText: """
            SM SM1 {
              State S1
              State S2
              S1 -> S2
            }
            """);

        yield return new CompleteTestCase(
            Name: "State machine with two states and a named transition",
            """
            SM SM1 {
              State S1
              State S2
              S1 -> S2 : T1
            }
            """);

        yield return new IncompleteTestCase(
            Name: "State machine with a named transition",
            originalText: """
            SM SM1 {
              S1 -> S2 : T1
            }
            """,
            expectedFinalText: """
            SM SM1 {
              State S1
              State S2
              S1 -> S2 : T2
            }
            """);

        yield return new CompleteTestCase(
            Name: "State machine with two states, one brokenly named, and a transition",
            """
            SM SM1 {
              State "S 1"
              State S2
              "S 1" -> S2 : T1
            }
            """);

        yield return new IncompleteTestCase(
            Name: "State machine with a transition that has a brokenly named state",
            originalText: """
            SM SM1 {
              "S 1" -> S2 : T1
            }
            """,
            expectedFinalText: """
            SM SM1 {
              State "S 1"
              State S2
              "S 1" -> S2 : T1
            }
            """);

        yield return new CompleteTestCase(
            Name: "State machine with two states and a brokenly named transition",
            """
            SM SM1 {
              State S1
              State S2
              S1 -> S2 : "T 1"
            }
            """);

        yield return new IncompleteTestCase(
            Name: "State machine with a brokenly named transition",
            originalText: """
            SM SM1 {
              S1 -> S2 : "T 1"
            }
            """,
            expectedFinalText: """
            SM SM1 {
              State S1
              State S2
              S1 -> S2 : "T 1"
            }
            """);
        
        yield return new CompleteTestCase(
            Name: "Two related entities",
            text: """
            Entity E1 {
            }

            Entity E2 {
            }

            E1 -> E2
            """);

        yield return new CompleteTestCase(
            Name: "Two related entities via named relation",
            text: """
            Entity E1 {
            }

            Entity E2 {
            }

            E1 -> E2 : R1
            """);

    }
}