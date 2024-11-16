using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;

namespace ScenarioModel.Objects.ScenarioNodes;

[NodeLike<IScenarioNode, JumpNode>]
public record JumpNode : ScenarioNode<JumpEvent>
{
    [NodeLikeProperty]
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

    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);
}
