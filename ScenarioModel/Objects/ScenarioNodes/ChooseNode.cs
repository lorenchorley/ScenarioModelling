using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Objects.Visitors;

namespace ScenarioModel.Objects.ScenarioNodes;

[NodeLike<IScenarioNode, ChooseNode>]
public record ChooseNode : ScenarioNode<ChoiceSelectedEvent>
{
    [NodeLikeProperty(serialise: false)]
    public ChoiceList Choices { get; set; } = new();

    public ChooseNode()
    {
        Name = "Choose";
    }

    public override ChoiceSelectedEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        return new ChoiceSelectedEvent() { ProducerNode = this };
    }

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IScenarioNode>>();

    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IScenarioVisitor visitor)
        => visitor.VisitChooseNode(this);
}