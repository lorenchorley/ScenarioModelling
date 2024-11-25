using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.ObjectDeserialisers.Interfaces;

[NodeLike<INodeSerialiser, JumpNode>]
public class JumpNodeSerialiser(string IndentSegment) : INodeSerialiser<JumpNode>
{
    public void WriteNode(StringBuilder sb, Scenario scenario, JumpNode node, string currentIndent)
    {
        sb.AppendLine($"{currentIndent}Jump {node.Name} {{");

        string subIndent = currentIndent + IndentSegment;
        ScenarioNodeExhaustivity.DoForEachNodeProperty(node, (prop, value) => sb.AppendLine($"{subIndent}{prop} {value}"));

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

