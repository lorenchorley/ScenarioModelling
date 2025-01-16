using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers;

[StoryNodeLike<INodeSerialiser, DialogNode>]
public class DialogNodeSerialiser(string IndentSegment) : INodeSerialiser<DialogNode>
{
    public void WriteNode(StringBuilder sb, MetaStory MetaStory, DialogNode node, string currentIndent)
    {
        var name = string.IsNullOrEmpty(node.Name) ? "" : node.Name.AddQuotes() + " ";
        sb.AppendLine($"{currentIndent}Dialog {name}{{");

        string subIndent = currentIndent + IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        sb.AppendLine($"{currentIndent}}}");
    }
}

