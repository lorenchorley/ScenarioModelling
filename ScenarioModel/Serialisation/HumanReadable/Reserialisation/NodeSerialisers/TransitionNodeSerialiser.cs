using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.ObjectDeserialisers.Interfaces;

[NodeLike<INodeSerialiser, TransitionNode>]
public class TransitionNodeSerialiser(string IndentSegment) : INodeSerialiser<TransitionNode>
{
    public void WriteNode(StringBuilder sb, Scenario scenario, TransitionNode node, string currentIndent)
    {
        if (node.StatefulObject == null)
        {
            throw new Exception($"Stateful object not set on transition: {node}");
        }

        var stateful = node.StatefulObject.ResolveReference();

        var obj = stateful.Match(
            Some: s => s,
            None: () => throw new Exception($"Stateful object not found: {node.StatefulObject}"));

        sb.AppendLine($"{currentIndent}Transition {{");

        string subIndent = currentIndent + IndentSegment;
        sb.AppendLine($"{subIndent}{obj.Name} : {node.TransitionName}");

        sb.AppendLine($"{currentIndent}}}");
    }
}

