using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.Reserialisation;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, IfNode>]
public class IfNodeSerialiser(string IndentSegment, MetaStorySerialiser MetaStorySerialiser) : INodeSerialiser<IfNode>
{
    public void WriteNode(StringBuilder sb, MetaStory MetaStory, IfNode node, string currentIndent)
    {
        ExpressionSerialiser visitor = new(MetaStory.System);
        var result = (string)node.Condition.Accept(visitor);

        string subIndent = currentIndent + IndentSegment;

        sb.AppendLine($"{currentIndent}If <{result}> {{");

        node.SerialiseAnnotatedProperties(sb, subIndent);

        foreach (var subNode in node.SubGraph.NodeSequence)
        {
            MetaStorySerialiser.WriteMetaStoryNode(sb, MetaStory, subNode, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
    }
}

