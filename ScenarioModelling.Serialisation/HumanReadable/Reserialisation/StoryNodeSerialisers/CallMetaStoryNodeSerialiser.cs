using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, CallMetaStoryNode>]
public class CallMetaStoryNodeSerialiser : INodeSerialiser<CallMetaStoryNode>
{
    public void WriteNode(StringBuilder sb, MetaStory MetaStory, CallMetaStoryNode node, string currentIndent)
    {
        var name = string.IsNullOrEmpty(node.Name) ? "" : node.Name.AddQuotes() + " ";
        sb.AppendLine($"{currentIndent}CallMetaStory {name}{{");

        string subIndent = currentIndent + ContextSerialiser.IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        sb.AppendLine($"{currentIndent}}}");
    }
}
