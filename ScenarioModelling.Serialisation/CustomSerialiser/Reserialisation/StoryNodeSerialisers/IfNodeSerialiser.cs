using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Serialisation.Expressions;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, IfNode>]
public class IfNodeSerialiser : INodeSerialiser<IfNode>
{
    public MetaStorySerialiser? MetaStorySerialiser { get; set; }

    public void WriteNode(StringBuilder sb, MetaStory MetaStory, IfNode node, string currentIndent)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(MetaStorySerialiser);

        ExpressionSerialiser visitor = new(MetaStory.MetaState);
        var result = (string)node.Condition.Accept(visitor);

        string subIndent = currentIndent + CustomContextSerialiser.IndentSegment;

        sb.AppendLine($"{currentIndent}If <{result}> {{");

        node.SerialiseAnnotatedProperties(sb, subIndent);

        foreach (var subNode in node.SubGraph.UnorderedEnumerable)
        {
            // TODO write the nodes in a way that reflects the type of the sub graph
            MetaStorySerialiser!.WriteMetaStoryNode(sb, MetaStory, subNode, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
    }
}

