using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers;

[NodeLike<INodeSerialiser, TransitionNode>]
public class TransitionNodeSerialiser(string IndentSegment) : INodeSerialiser<TransitionNode>
{
    public void WriteNode(StringBuilder sb, MetaStory scenario, TransitionNode node, string currentIndent)
    {
        if (node.StatefulObject == null)
        {
            throw new Exception($"Stateful object not set on transition: {node}");
        }

        var stateful = node.StatefulObject.ResolveReference();

        var obj = stateful.Match(
            Some: s => s,
            None: () => throw new Exception($"Stateful object not found: {node.StatefulObject}"));

        var name = string.IsNullOrEmpty(node.Name) ? "" : node.Name.AddQuotes() + " ";
        sb.AppendLine($"{currentIndent}Transition {name}{{");

        string subIndent = currentIndent + IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        sb.AppendLine($"{subIndent}{obj.Name} : {node.TransitionName}");

        sb.AppendLine($"{currentIndent}}}");
    }
}

