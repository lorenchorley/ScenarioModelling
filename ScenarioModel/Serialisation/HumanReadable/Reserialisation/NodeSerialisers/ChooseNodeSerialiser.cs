using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.ObjectDeserialisers.Interfaces;

[NodeLike<INodeSerialiser, ChooseNode>]
public class ChooseNodeSerialiser(string IndentSegment) : INodeSerialiser<ChooseNode>
{
    public void WriteNode(StringBuilder sb, Scenario scenario, ChooseNode node, string currentIndent)
    {
        sb.AppendLine($"{currentIndent}Choose {node.Name} {{");

        string subIndent = currentIndent + IndentSegment;
        ScenarioNodeExhaustivity.DoForEachNodeProperty(node, (prop, value) => sb.AppendLine($"{subIndent}{prop} {value}"));

        foreach (var option in node.Choices)
        {
            if (string.IsNullOrEmpty(option.Text))
                sb.AppendLine($"{subIndent}{option.NodeName}");
            else
                sb.AppendLine($"{subIndent}{option.NodeName} {option.Text}");
        }

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

