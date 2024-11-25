using OneOf;

namespace ScenarioModel.Objects.ScenarioNodes.BaseClasses;

public class OneOfIScenaroNode : OneOfBase<ChooseNode, DialogNode, IfNode, JumpNode, TransitionNode, WhileNode>
{
    public OneOfIScenaroNode(OneOf<ChooseNode, DialogNode, IfNode, JumpNode, TransitionNode, WhileNode> input) : base(input)
    {
    }
}
