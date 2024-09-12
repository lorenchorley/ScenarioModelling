using ScenarioModel.References;
using ScenarioModel.ScenarioObjects.Events;
using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.Tests.Valid;

public static class ValidScenario1
{
    public static Scenario Generate()
        => new()
        {
            Name = nameof(ValidScenario1),
            System = ValidSystem1.Generate(),
            Steps = new()
            {
                new ChooseAction() { Name = "A1", Choices = [ "A2", "A1" ] },
            }
        };
}