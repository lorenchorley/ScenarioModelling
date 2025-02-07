using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, MetadataNode>]
public class MetadataNodeSerialiser(string IndentSegment) : INodeSerialiser<MetadataNode>
{
    public void WriteNode(StringBuilder sb, MetaStory metaStory, MetadataNode node, string currentIndent)
    {
        var name = string.IsNullOrEmpty(node.Name) ? "" : node.Name.AddQuotes() + " ";
        sb.AppendLine($"{currentIndent}Metadata {name}{{");

        string subIndent = currentIndent + IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        sb.AppendLine($"{currentIndent}}}");
    }
}
