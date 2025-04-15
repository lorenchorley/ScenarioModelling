using OneOf;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;

public class OneOfMetaStoryNode : OneOfBase<AssertNode, CallMetaStoryNode, ChooseNode, DialogNode, IfNode, JumpNode, LoopNode, MetadataNode, TransitionNode, WhileNode>
{
    [DebuggerNonUserCode]
    public OneOfMetaStoryNode(OneOf<AssertNode, CallMetaStoryNode, ChooseNode, DialogNode, IfNode, JumpNode, LoopNode, MetadataNode, TransitionNode, WhileNode> input) : base(input)
    {

    }
}
