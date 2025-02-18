using OneOf;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;

public class OneOfIScenaroNode : OneOfBase<CallMetaStoryNode, ChooseNode, DialogNode, IfNode, JumpNode, MetadataNode, TransitionNode, WhileNode>
{
    [DebuggerNonUserCode]
    public OneOfIScenaroNode(OneOf<CallMetaStoryNode, ChooseNode, DialogNode, IfNode, JumpNode, MetadataNode, TransitionNode, WhileNode> input) : base(input)
    {
        
    }
}
