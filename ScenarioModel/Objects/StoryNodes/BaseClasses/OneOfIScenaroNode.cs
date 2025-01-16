using OneOf;
using System.Diagnostics;

namespace ScenarioModelling.Objects.StoryNodes.BaseClasses;

public class OneOfIScenaroNode : OneOfBase<ChooseNode, DialogNode, IfNode, JumpNode, TransitionNode, WhileNode>
{
    [DebuggerNonUserCode]
    public OneOfIScenaroNode(OneOf<ChooseNode, DialogNode, IfNode, JumpNode, TransitionNode, WhileNode> input) : base(input)
    {
    }
}
