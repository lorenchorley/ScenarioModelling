using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, TransitionNode>]
public class TransitionNodeSerialiser : INodeSerialiser<TransitionNode>
{
    public void WriteNode(StringBuilder sb, MetaStory MetaStory, TransitionNode node, string currentIndent)
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

        string subIndent = currentIndent + ContextSerialiser.IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        sb.AppendLine($"{subIndent}{obj.Name} : {node.TransitionName}");

        sb.AppendLine($"{currentIndent}}}");
    }
}

