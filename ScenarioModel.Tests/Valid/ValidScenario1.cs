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
                new ChooseNode() { Name = "A1", Choices = [ "A2", "A1" ] },
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
            StateTypes = new()
            {
                new() { Name = "ST1", States = [ new() { Name = "S1", Transitions = ["S2"] }, new() { Name = "S2" }] },
            }
        };
    }

    public static string SerialisedContext
    {
        get => """
            Entity "E1" 
            {
                State "S1"
            }

            Entity "E2" {
            }

            State "S1" {
                Type "ST1"
            }

            Scenario "ValidScenario1" {
                Dialog "D1" {
                    TextTemplate: "Hello"
                }
                Choose "A1" {
                    Choices: [ "A2", "A1" ]
                }
            }
            """;
    }
}