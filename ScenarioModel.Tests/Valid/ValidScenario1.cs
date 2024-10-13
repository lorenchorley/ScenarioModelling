using ScenarioModel.References;
using ScenarioModel.SystemObjects.Entities;

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
                new ChooseNode() { Name = "C1", Choices = [ "ST1", "D1" ] },
                new StateTransitionNode() { Name = "ST1", StatefulObject = new EntityReference() { EntityName = "E1" }, StateName = "S1" },
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
                new() { Name = "ST1", States = [ new() { Name = "S1", Transitions = ["S2"] }, new() { Name = "S2" }] },
            }
        };
    }

    // Important to have one of each type of thing here to test de/reserialisation
    public static string SerialisedContext
    {
        get => """
            Entity "E1" 
            {
                State S1
            }

            Entity E2

            SM "SM1" {
                S1 -> S2
            }

            Scenario ValidScenario1 {
                Dialog "D1" {
                    Text "Hello"
                }
                Choose "C1" {
                    D2
                    C1
                }
                Dialog "D2" {
                    Text "Bubye"
                }
            }
            """;
    }
}