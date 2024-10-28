using ScenarioModel.Execution.Events;

namespace ScenarioModel.Objects.Scenario;

public record JumpNode : IScenarioNode<JumpEvent>
{
    public string Name { get; set; } = "Jump";
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
