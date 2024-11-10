using OneOf;

namespace ScenarioModel.Objects.ScenarioNodes.BaseClasses;

public class OneOfIScenaroNode : OneOfBase<ChooseNode, DialogNode, IfNode, JumpNode, StateTransitionNode, WhileNode>
{
    public OneOfIScenaroNode(OneOf<ChooseNode, DialogNode, IfNode, JumpNode, StateTransitionNode, WhileNode> input) : base(input)
    {
    }
}
