using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers;

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
            None: () => throw new Exception($"Stateful object not found: {node.StatefulObject.Name}"));

        var name = string.IsNullOrEmpty(node.Name) ? "" : node.Name.AddQuotes() + " ";
        sb.AppendLine($"{currentIndent}Transition {name}{{");

        string subIndent = currentIndent + CustomContextSerialiser.IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        if (obj is Aspect aspect)
        {
            sb.AppendLine($"{subIndent}{aspect.Entity.Name}.{aspect.Name} : {node.TransitionName.AddQuotes()}");
        }
        else
        {
            sb.AppendLine($"{subIndent}{obj.Name} : {node.TransitionName.AddQuotes()}");
        }

        sb.AppendLine($"{currentIndent}}}");
    }
}

