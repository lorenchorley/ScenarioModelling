using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.Serialisation.Expressions;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, WhileNode>]
public class WhileNodeSerialiser : INodeSerialiser<WhileNode>
{
    public MetaStorySerialiser? MetaStorySerialiser { get; set; }

    public void WriteNode(StringBuilder sb, MetaStory MetaStory, WhileNode node, string currentIndent)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(MetaStorySerialiser);

        ExpressionSerialiser visitor = new(MetaStory.MetaState);
        var result = (string)node.Condition.Accept(visitor);

        sb.AppendLine($"{currentIndent}While <{result}> {{");

        string subIndent = currentIndent + CustomContextSerialiser.IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        foreach (var subNode in node.SubGraph.NodeSequence)
        {
            MetaStorySerialiser!.WriteMetaStoryNode(sb, MetaStory, subNode, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
    }
}

