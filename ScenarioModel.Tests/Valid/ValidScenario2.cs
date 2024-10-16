using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.References;
using ScenarioModel.ScenarioObjects;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.Tests.Valid;

public static class ValidScenario2
{
    public static Scenario Scenario
    {
        get => new()
        {
            Name = nameof(ValidScenario2),
            Steps = new()
            {
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
            StateMachines = new()
            {
                new() { Name = "ST1", States = [ new() { Name = "S1", Transitions = [new Transition() { SourceState = "S1", DestinationState = "S2", Name = "T1" }] }, new() { Name = "S2" }] },
            },
            Constraints =
            [
                new HasRelationExpression()
                {
                    Name = "R1",
                    Left = "E1",
                    Right = "E2"
                }
            ]
        };
    }
}