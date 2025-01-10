﻿using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers;

[NodeLike<INodeSerialiser, ChooseNode>]
public class ChooseNodeSerialiser(string IndentSegment) : INodeSerialiser<ChooseNode>
{
    public void WriteNode(StringBuilder sb, Scenario scenario, ChooseNode node, string currentIndent)
    {
        var name = string.IsNullOrEmpty(node.Name) ? "" : node.Name.AddQuotes() + " ";
        sb.AppendLine($"{currentIndent}Choose {name}{{");

        string subIndent = currentIndent + IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        foreach (var option in node.Choices)
        {
            if (string.IsNullOrEmpty(option.Text))
                sb.AppendLine($"{subIndent}{option.NodeName}");
            else
                sb.AppendLine($"{subIndent}{option.NodeName} {option.Text.AddQuotes()}");
        }

        sb.AppendLine($"{currentIndent}}}");
    }
}

