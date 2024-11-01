using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Interpolation;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.Objects.ScenarioObjects.DataClasses;

namespace ScenarioModel.Objects.ScenarioObjects;

public record JumpNode : ScenarioNode<JumpEvent>
{
    public string Target { get; set; } = "";

    public JumpNode()
    {
        Name = "Jump";
    }

    public override JumpEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        return new JumpEvent()
        {
            Target = Target,
            ProducerNode = this,
        };
    }

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IScenarioNode>>();
}
