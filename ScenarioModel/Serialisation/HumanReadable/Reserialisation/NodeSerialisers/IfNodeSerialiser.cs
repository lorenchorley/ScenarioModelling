﻿using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.Reserialisation;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers;

[NodeLike<INodeSerialiser, IfNode>]
public class IfNodeSerialiser(string IndentSegment, ScenarioSerialiser ScenarioSerialiser) : INodeSerialiser<IfNode>
{
    public void WriteNode(StringBuilder sb, Scenario scenario, IfNode node, string currentIndent)
    {
        ExpressionSerialiser visitor = new(scenario.System);
        var result = (string)node.Condition.Accept(visitor);

        string subIndent = currentIndent + IndentSegment;
        ScenarioNodeExhaustivity.DoForEachNodeProperty(node, (prop, value) => sb.AppendLine($"{subIndent}{prop} {value}"));

        sb.AppendLine($"{currentIndent}If <{result}> {{"); // TODO Serialise the expression correctly

        foreach (var subNode in node.SubGraph.NodeSequence)
        {
            ScenarioSerialiser.WriteScenarioNode(sb, scenario, subNode, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
        //sb.AppendLine($"");
    }
}

