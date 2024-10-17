using ScenarioModel.Execution.Events;

namespace ScenarioModel.ScenarioObjects;

public record DialogNode : IScenarioNode<DialogEvent>
{
    public string Name { get; set; } = "Dialog";
    public string TextTemplate { get; set; } = "";
    public string? Character { get; set; } = null;

    public DialogEvent GenerateEvent()
    {
        return new DialogEvent()
        {
            Character = Character,
            ProducerNode = this,
        };
    }
}
