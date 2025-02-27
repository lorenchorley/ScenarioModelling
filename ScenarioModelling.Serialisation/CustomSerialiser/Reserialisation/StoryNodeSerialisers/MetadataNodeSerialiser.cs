using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, MetadataNode>]
public class MetadataNodeSerialiser : INodeSerialiser<MetadataNode>
{
    public void WriteNode(StringBuilder sb, MetaStory metaStory, MetadataNode node, string currentIndent)
    {
        var name = string.IsNullOrEmpty(node.Name) ? "" : node.Name.AddQuotes() + " ";
        sb.AppendLine($"{currentIndent}Metadata {name}{{");

        string subIndent = currentIndent + CustomContextSerialiser.IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        sb.AppendLine($"{currentIndent}}}");
    }
}
