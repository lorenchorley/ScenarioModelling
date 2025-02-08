﻿using ProtoBuf;
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
[StoryNodeLike<IStoryNode, WhileNode>]
public record WhileNode : StoryNode<WhileLoopConditionCheckEvent>, IStoryNodeWithExpression, IFlowNode
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

    public WhileNode()
    {
    }

    public override WhileLoopConditionCheckEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        WhileLoopConditionCheckEvent e = new()
        {
            ProducerNode = this,
        };

        var result = Condition.Accept(dependencies.Evaluator);

        if (result is not bool shouldExecuteBlock)
        {
            throw new Exception($"While loop condition {Condition} did not evaluate to a boolean, this is a failure of the expression validation mecanism to not correctly determine the return type.");
        }

        e.LoopBlockRun = shouldExecuteBlock;

        return e;
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
    {
        yield return SubGraph;
    }

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitWhileNode(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        if (other is not WhileNode otherNode)
            return false;

        if (!other.Name.IsEqv(Name))
            return false;

        if (!otherNode.OriginalConditionText.IsEqv(OriginalConditionText))
            return false;

        // Should not take into account subgraph ! That would be for IsDeepEqv

        return true;
    }
}
