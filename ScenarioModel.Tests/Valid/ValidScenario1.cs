using ScenarioModel.References;
using ScenarioModel.ScenarioObjects.Events;
using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.Tests.Valid;

public static class ValidScenario1
{
    public static Scenario Scenario
    {
        get => new()
        {
            Name = nameof(ValidScenario1),
            SystemName = nameof(ValidScenario1) + "_System",
            Steps = new()
            {
                new ChooseAction() { Name = "A1", Choices = [ "A2", "A1" ] },
            }
        };
    }

    public static System System
    {
        get => new()
        {
            Name = nameof(ValidScenario1) + "_System",
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
}