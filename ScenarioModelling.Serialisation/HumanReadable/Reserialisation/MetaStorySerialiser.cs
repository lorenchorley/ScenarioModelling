using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers;
using ScenarioModelling.Tools.Collections.Graph;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation;

public class MetaStorySerialiser
{
    private readonly CallMetaStoryNodeSerialiser _callMetaStoryNodeSerialiser;
    private readonly ChooseNodeSerialiser _chooseNodeSerialiser;
    private readonly DialogNodeSerialiser _dialogNodeSerialiser;
    private readonly IfNodeSerialiser _ifNodeSerialiser;
    private readonly JumpNodeSerialiser _jumpNodeSerialiser;
    private readonly MetadataNodeSerialiser _metadataNodeSerialiser;
    private readonly TransitionNodeSerialiser _transitionNodeSerialiser;
    private readonly WhileNodeSerialiser _whileNodeSerialiser;

    public MetaStorySerialiser(CallMetaStoryNodeSerialiser callMetaStoryNodeSerialiser, ChooseNodeSerialiser chooseNodeSerialiser, DialogNodeSerialiser dialogNodeSerialiser, IfNodeSerialiser ifNodeSerialiser, JumpNodeSerialiser jumpNodeSerialiser, MetadataNodeSerialiser metadataNodeSerialiser, TransitionNodeSerialiser transitionNodeSerialiser, WhileNodeSerialiser whileNodeSerialiser)
    {
        _callMetaStoryNodeSerialiser = callMetaStoryNodeSerialiser;
        _chooseNodeSerialiser = chooseNodeSerialiser;
        _dialogNodeSerialiser = dialogNodeSerialiser;
        _ifNodeSerialiser = ifNodeSerialiser;
        _jumpNodeSerialiser = jumpNodeSerialiser;
        _metadataNodeSerialiser = metadataNodeSerialiser;
        _transitionNodeSerialiser = transitionNodeSerialiser;
        _whileNodeSerialiser = whileNodeSerialiser;

        _ifNodeSerialiser.MetaStorySerialiser = this;
        _whileNodeSerialiser.MetaStorySerialiser = this;
    }

    public void WriteMetaStory(StringBuilder sb, MetaStory MetaStory, string currentIndent)
    {
        sb.AppendLine($"{currentIndent}MetaStory {MetaStory.Name.AddQuotes()} {{");

        foreach (var node in MetaStory.Graph.PrimarySubGraph.NodeSequence)
        {
            WriteMetaStoryNode(sb, MetaStory, node, currentIndent + ContextSerialiser.IndentSegment);
        }

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }

    public void WriteMetaStoryNode(StringBuilder sb, MetaStory metaStory, IStoryNode node, string currentIndent)
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
            (CallMetaStoryNode callMetaStoryNode) => _callMetaStoryNodeSerialiser.WriteNode(sb, metaStory, callMetaStoryNode, currentIndent),
            (ChooseNode chooseNode) => _chooseNodeSerialiser.WriteNode(sb, metaStory, chooseNode, currentIndent),
            (DialogNode dialogNode) => _dialogNodeSerialiser.WriteNode(sb, metaStory, dialogNode, currentIndent),
            (IfNode ifNode) => _ifNodeSerialiser.WriteNode(sb, metaStory, ifNode, currentIndent),
            (JumpNode jumpNode) => _jumpNodeSerialiser.WriteNode(sb, metaStory, jumpNode, currentIndent),
            (MetadataNode metadata) => _metadataNodeSerialiser.WriteNode(sb, metaStory, metadata, currentIndent),
            (TransitionNode transitionNode) => _transitionNodeSerialiser.WriteNode(sb, metaStory, transitionNode, currentIndent),
            (WhileNode whileNode) => _whileNodeSerialiser.WriteNode(sb, metaStory, whileNode, currentIndent)
        );
    }

}

