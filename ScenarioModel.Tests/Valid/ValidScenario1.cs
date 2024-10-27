using ScenarioModel.References;
using ScenarioModel.ScenarioObjects;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.Tests.Valid;

public static class ValidScenario1
{
    public static Scenario Scenario
    {
        get => new()
        {
            Name = nameof(ValidScenario1),
            Steps = new()
            {
                new DialogNode() { Name = "D1", TextTemplate = "Hello" },
                new ChooseNode() { Name = "C1", Choices = [ ("ST1", "ST1"), ("D1", "D1") ] },
                new StateTransitionNode() { Name = "ST1", StatefulObject = new EntityReference() { EntityName = "E1" }, TransitionName = "S1" },
                new DialogNode() { Name = "D2", TextTemplate = "Bubye" },
            }
        };
    }

    public static System System
    {
        get => new()
        {
            Entities = new()
            {
                new() { Name = "E1", State = new() { Name = "S1" } },
                new() { Name = "E2" },
            },
            StateMachines = new()
            {
                new() { Name = "ST1", States = [ new() { Name = "S1", Transitions = [new Transition() { SourceState = "S1", DestinationState = "S2", Name = "T1" }] }, new() { Name = "S2" }] },
            }
        };
    }

    // Important to have one of each type of thing here to test de/reserialisation
    public static string SerialisedContext
    {
        get => """
            Entity E1 {
              EntityType ET1
              State S1
            }

            Entity E2 {
              EntityType ET2
            }

            EntityType ET1 {
              SM SM1
            }

            EntityType ET2 {
            }

            SM SM1 {
              State S1
              State S2
              S1 -> S2
            }

            Scenario ValidScenario1 {
              Dialog D1 {
                Text Hello
              }

              Choose C1 {
                D2
                C1
              }

              Dialog D2 {
                Text Bubye
              }

            }
            """;
    }
}