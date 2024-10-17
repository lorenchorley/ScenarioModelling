using ScenarioModel.Execution.Events;

namespace ScenarioModel.ScenarioObjects;

public record ChooseNode : IScenarioNode<ChoiceSelectedEvent>
{
    public string Name { get; set; } = "Choose";
    public ChoiceList Choices { get; set; } = new();

    public IEnumerable<string> TargetNodeNames => Choices.Select(c => c.NodeName);

    public ChoiceSelectedEvent GenerateEvent()
    {
        return new ChoiceSelectedEvent() { ProducerNode = this };
    }
}