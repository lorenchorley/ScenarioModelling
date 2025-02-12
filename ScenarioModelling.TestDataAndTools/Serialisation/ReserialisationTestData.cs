namespace ScenarioModelling.TestDataAndTools.Serialisation;

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
            StateMachine SM1 {
            }
            """);

        yield return new IncompleteTestCase(
            Name: "State machine without name",
            originalText:
            """
            StateMachine {
            }
            """,
            expectedFinalText:
            """
            StateMachine StateMachine1 {
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
              StateMachine SM1
            }

            StateMachine SM1 {
            }
            """);

        yield return new CompleteTestCase(
            Name: "Entity with state but no other direct link to state machine",
            text: """
            Entity E1 {
              State S1
            }

            StateMachine SM1 {
              State S1
            }
            """);

        //yield return new IncompleteTestCase(
        //    Name: "Entity with state but no other direct link to state machine",
        //    originalText: """
        //    Entity E1 {
        //      State S1
        //    }

        //    StateMachine SM1 {
        //      State S1
        //    }
        //    """,
        //    expectedFinalText: """
        //    Entity E1 {
        //      EntityType EntityType1
        //      State S1
        //    }

        //    EntityType EntityType1 {
        //      StateMachine SM1
        //    }

        //    StateMachine SM1 {
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
              StateMachine SM1
            }

            StateMachine SM1 {
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
              StateMachine SM1
            }

            StateMachine SM1 {
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
              StateMachine SM1
            }

            StateMachine SM1 {
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
              StateMachine SM1
            }

            StateMachine SM1 {
              State S1
              State S2
              S1 -> S2 : T1
            }
            """);

        yield return new CompleteTestCase(
            Name: "State machine with two states and a transition",
            """
            StateMachine SM1 {
              State S1
              State S2
              S1 -> S2
            }
            """);

        yield return new IncompleteTestCase(
            Name: "State machine with a transition",
            originalText: """
            StateMachine SM1 {
              S1 -> S2
            }
            """,
            expectedFinalText: """
            StateMachine SM1 {
              State S1
              State S2
              S1 -> S2
            }
            """);

        yield return new IncompleteTestCase(
            Name: "State machine with a transition and only one explicit state",
            originalText: """
            StateMachine SM1 {
              State S2
              S1 -> S2
            }
            """,
            expectedFinalText: """
            StateMachine SM1 {
              State S2
              State S1
              S1 -> S2
            }
            """);

        yield return new CompleteTestCase(
            Name: "State machine with two states and a named transition",
            """
            StateMachine SM1 {
              State S1
              State S2
              S1 -> S2 : T1
            }
            """);

        yield return new IncompleteTestCase(
            Name: "State machine with a named transition",
            originalText: """
            StateMachine SM1 {
              S1 -> S2 : T1
            }
            """,
            expectedFinalText: """
            StateMachine SM1 {
              State S1
              State S2
              S1 -> S2 : T1
            }
            """);

        yield return new CompleteTestCase(
            Name: "State machine with two states, one brokenly named, and a transition",
            """
            StateMachine SM1 {
              State "S 1"
              State S2
              "S 1" -> S2 : T1
            }
            """);

        yield return new IncompleteTestCase(
            Name: "State machine with a transition that has a brokenly named state",
            originalText: """
            StateMachine SM1 {
              "S 1" -> S2 : T1
            }
            """,
            expectedFinalText: """
            StateMachine SM1 {
              State "S 1"
              State S2
              "S 1" -> S2 : T1
            }
            """);

        yield return new CompleteTestCase(
            Name: "State machine with two states and a brokenly named transition",
            """
            StateMachine SM1 {
              State S1
              State S2
              S1 -> S2 : "T 1"
            }
            """);

        yield return new IncompleteTestCase(
            Name: "State machine with a brokenly named transition",
            originalText: """
            StateMachine SM1 {
              S1 -> S2 : "T 1"
            }
            """,
            expectedFinalText: """
            StateMachine SM1 {
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

        yield return new CompleteTestCase(
            Name: "One constraint",
            text: """
            Constraint <A == A> {
              Description "A must always be equal to A, a tautology"
            }
            """);

        yield return new CompleteTestCase(
            Name: "One constraint on entity state",
            text: """
            Entity E1 {
              State S1
            }
            
            StateMachine StateMachine1 {
              State S1
            }
            
            Constraint <E1.State == S1> {
              Description "E1's state must stay S1"
            }
            """);

        // TODO more constraints

    }
}