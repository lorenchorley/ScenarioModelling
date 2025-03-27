using OneOf;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;

public class OneOfScenaroNode : OneOfBase<AssertNode, CallMetaStoryNode, ChooseNode, DialogNode, IfNode, JumpNode, LoopNode, MetadataNode, TransitionNode, WhileNode>
{
    [DebuggerNonUserCode]
    public OneOfScenaroNode(OneOf<AssertNode, CallMetaStoryNode, ChooseNode, DialogNode, IfNode, JumpNode, LoopNode, MetadataNode, TransitionNode, WhileNode> input) : base(input)
    {

    }
}
