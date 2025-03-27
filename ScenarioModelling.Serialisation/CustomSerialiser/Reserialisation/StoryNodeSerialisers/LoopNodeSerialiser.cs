using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, LoopNode>]
public class LoopNodeSerialiser : INodeSerialiser<LoopNode>
{
    public MetaStorySerialiser? MetaStorySerialiser { get; set; }

    public void WriteNode(StringBuilder sb, MetaStory MetaStory, LoopNode node, string currentIndent)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(MetaStorySerialiser);

        sb.AppendLine($"{currentIndent}Loop {{");

        string subIndent = currentIndent + CustomContextSerialiser.IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        foreach (var subNode in node.SubGraph.UnorderedEnumerable)
        {
            // TODO write the nodes in a way that reflects the type of the sub graph
            MetaStorySerialiser!.WriteMetaStoryNode(sb, MetaStory, subNode, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
    }
}

