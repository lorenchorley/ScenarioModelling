using ScenarioModel.Execution.Events;

namespace ScenarioModel.ScenarioObjects;

public record JumpNode : IScenarioNode<JumpEvent>
{
    public string Name { get; set; } = "";
    public string Target { get; set; } = "";

    public JumpEvent GenerateEvent()
    {
        return new JumpEvent()
        {
            Target = Target,
            ProducerNode = this,
        };
    }
}
