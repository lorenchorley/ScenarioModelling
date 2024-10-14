using ScenarioModel.Execution.Events;

namespace ScenarioModel.ScenarioObjects;

public class ChooseNode : IScenarioNode
{
    public string Name { get; set; } = "";
    public List<string> Choices { get; set; } = new();

    public IEnumerable<string> TargetNodeNames => Choices;

    public IScenarioEvent ProduceEvent(string choice)
    {
        return new ChoiceSelectedEvent { Choice = choice };
    }
}
