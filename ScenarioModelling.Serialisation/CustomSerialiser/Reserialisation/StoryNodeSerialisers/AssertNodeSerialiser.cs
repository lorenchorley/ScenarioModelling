using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers.Interfaces;
using ScenarioModelling.Serialisation.Expressions;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, AssertNode>]
public class AssertNodeSerialiser : INodeSerialiser<AssertNode>
{
    public void WriteNode(StringBuilder sb, MetaStory MetaStory, AssertNode node, string currentIndent)
    {
        ExpressionSerialiser visitor = new(MetaStory.MetaState);
        var result = (string)node.AssertionExpression.Accept(visitor);

        var name = string.IsNullOrEmpty(node.Name) ? "" : node.Name.AddQuotes() + " ";

        sb.AppendLine($"{currentIndent}Assert {name}<{result}>");
    }
}
