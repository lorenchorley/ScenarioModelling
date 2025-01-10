﻿using ScenarioModel.Collections.Graph;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Objects.ScenarioNodes.Interfaces;
using ScenarioModel.Objects.Visitors;
using System.Diagnostics;

namespace ScenarioModel.Objects.ScenarioNodes;

[NodeLike<IScenarioNode, WhileNode>]
public record WhileNode : ScenarioNode<WhileLoopConditionCheckEvent>, IScenarioNodeWithExpression, IFlowNode
{
    [NodeLikeProperty(serialise: false)]
    public Expression Condition { get; set; } = null!;

    [NodeLikeProperty(serialise: false)]
    public string OriginalConditionText { get; set; } = "";

    [NodeLikeProperty(serialise: false)]
    public SemiLinearSubGraph<IScenarioNode> SubGraph { get; set; } = new();

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

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
    {
        yield return SubGraph;
    }

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IScenarioVisitor visitor)
        => visitor.VisitWhileNode(this);

    public override bool IsFullyEqv(IScenarioNode other)
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
