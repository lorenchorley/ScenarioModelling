using ScenarioModel.Execution.Events;

namespace ScenarioModel.ScenarioObjects;

public class DialogNode : IScenarioNode
{
    public string Name { get; set; } = "";
    public string TextTemplate { get; set; } = "";

    public IScenarioEvent ProduceEvent(string finalText)
    {
        return new DialogEvent
        {
            ProducerNode = this,
            Text = finalText
        };
    }
}
