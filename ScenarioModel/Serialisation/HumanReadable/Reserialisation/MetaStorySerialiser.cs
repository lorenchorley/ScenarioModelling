using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Objects.StoryNodes.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation;

public class MetaStorySerialiser
{
    private readonly string _indentSegment;
    private readonly ChooseNodeSerialiser _chooseNodeSerialiser;
    private readonly DialogNodeSerialiser _dialogNodeSerialiser;
    private readonly IfNodeSerialiser _ifNodeSerialiser;
    private readonly JumpNodeSerialiser _jumpNodeSerialiser;
    private readonly TransitionNodeSerialiser _transitionNodeSerialiser;
    private readonly WhileNodeSerialiser _whileNodeSerialiser;

    public MetaStorySerialiser(string indentSegment)
    {
        _indentSegment = indentSegment;

        // TODO Place somewhere centralised
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<INodeSerialiser>();

        _chooseNodeSerialiser = new(indentSegment);
        _dialogNodeSerialiser = new(indentSegment);
        _jumpNodeSerialiser = new(indentSegment);
        _transitionNodeSerialiser = new(indentSegment);
        _ifNodeSerialiser = new(indentSegment, this);
        _whileNodeSerialiser = new(indentSegment, this);
    }

    public void WriteMetaStory(StringBuilder sb, MetaStory MetaStory, string currentIndent)
    {
        sb.AppendLine($"{currentIndent}MetaStory {MetaStory.Name.AddQuotes()} {{");

        foreach (var node in MetaStory.Graph.PrimarySubGraph.NodeSequence)
        {
            WriteMetaStoryNode(sb, MetaStory, node, currentIndent + _indentSegment);
        }

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }

    public void WriteMetaStoryNode(StringBuilder sb, MetaStory MetaStory, IStoryNode node, string currentIndent)
    {
        // TODO Still used ?
        if (node is ITransitionNode transitionNode)
        {
            foreach (var target in transitionNode.TargetNodeNames)
            {
                sb.AppendLine($"{currentIndent}{node.Name} -> {target}");
            }
            return;
        }

        node.ToOneOf().Switch(
            (ChooseNode chooseNode) => _chooseNodeSerialiser.WriteNode(sb, MetaStory, chooseNode, currentIndent),
            (DialogNode dialogNode) => _dialogNodeSerialiser.WriteNode(sb, MetaStory, dialogNode, currentIndent),
            (IfNode ifNode) => _ifNodeSerialiser.WriteNode(sb, MetaStory, ifNode, currentIndent),
            (JumpNode jumpNode) => _jumpNodeSerialiser.WriteNode(sb, MetaStory, jumpNode, currentIndent),
            (TransitionNode transitionNode) => _transitionNodeSerialiser.WriteNode(sb, MetaStory, transitionNode, currentIndent),
            (WhileNode whileNode) => _whileNodeSerialiser.WriteNode(sb, MetaStory, whileNode, currentIndent)
        );
    }

}

