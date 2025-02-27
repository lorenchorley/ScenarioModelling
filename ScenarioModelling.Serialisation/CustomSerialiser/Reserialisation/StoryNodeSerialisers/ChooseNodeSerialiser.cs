using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, ChooseNode>]
public class ChooseNodeSerialiser : INodeSerialiser<ChooseNode>
{
    public void WriteNode(StringBuilder sb, MetaStory MetaStory, ChooseNode node, string currentIndent)
    {
        var name = string.IsNullOrEmpty(node.Name) ? "" : node.Name.AddQuotes() + " ";
        sb.AppendLine($"{currentIndent}Choose {name}{{");

        string subIndent = currentIndent + CustomContextSerialiser.IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        foreach (var option in node.Choices)
        {
            if (string.IsNullOrEmpty(option.Text))
                sb.AppendLine($"{subIndent}{option.NodeName}");
            else
                sb.AppendLine($"{subIndent}{option.NodeName} {option.Text.AddQuotes()}");
        }

        sb.AppendLine($"{currentIndent}}}");
    }
}
