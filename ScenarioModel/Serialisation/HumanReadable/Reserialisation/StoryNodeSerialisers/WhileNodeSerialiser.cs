using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.Reserialisation;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, WhileNode>]
public class WhileNodeSerialiser(string IndentSegment, MetaStorySerialiser MetaStorySerialiser) : INodeSerialiser<WhileNode>
{
    public void WriteNode(StringBuilder sb, MetaStory MetaStory, WhileNode node, string currentIndent)
    {
        ExpressionSerialiser visitor = new(MetaStory.System);
        var result = (string)node.Condition.Accept(visitor);

        sb.AppendLine($"{currentIndent}While <{result}> {{");

        string subIndent = currentIndent + IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        foreach (var subNode in node.SubGraph.NodeSequence)
        {
            MetaStorySerialiser.WriteMetaStoryNode(sb, MetaStory, subNode, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
    }
}

