using ProtoBuf;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.SemanticTree;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using ScenarioModelling.Objects.StoryNodes.Interfaces;
using ScenarioModelling.Objects.Visitors;
using System.Diagnostics;

namespace ScenarioModelling.Objects.StoryNodes;

[ProtoContract]
[StoryNodeLike<IStoryNode, IfNode>]
public record IfNode : StoryNode<IfBlockEvent>, IStoryNodeWithExpression, IFlowNode
{
    [ProtoMember(1)]
    [StoryNodeLikeProperty(serialise: false)]
    public Expression Condition { get; set; } = null!;

    [ProtoMember(2)]
    [StoryNodeLikeProperty(serialise: false)]
    public string OriginalConditionText { get; set; } = "";

    [ProtoMember(3)]
    [StoryNodeLikeProperty(serialise: false)]
    public SemiLinearSubGraph<IStoryNode> SubGraph { get; set; } = new();

    public IfNode()
    {
    }

    public override IfBlockEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        IfBlockEvent e = new IfBlockEvent() { ProducerNode = this };

        var result = Condition.Accept(dependencies.Evaluator);

        if (result is not bool shouldExecuteBlock)
        {
            throw new Exception($"If condition {Condition} did not evaluate to a boolean, this is a failure of the expression validation mecanism to not correctly determine the return type.");
        }

        e.Expression = OriginalConditionText;
        e.IfBlockRun = shouldExecuteBlock;

        return e;
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
    {
        yield return SubGraph;
    }

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitIfNode(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        if (other is not IfNode otherNode)
            return false;

        if (!other.Name.IsEqv(Name))
            return false;

        if (!otherNode.OriginalConditionText.IsEqv(OriginalConditionText))
            return false;

        // Should not take into account subgraph ! That would be for IsDeepEqv

        return true;
    }
}