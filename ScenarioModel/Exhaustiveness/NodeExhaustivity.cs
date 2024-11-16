using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Exhaustiveness.Common;
using ScenarioModel.Objects.ScenarioNodes;

namespace ScenarioModel.Exhaustiveness;

public static class NodeExhaustivity
{
    public static Type[] AllNodeTypes =>
    [
        typeof(ChooseNode),
        typeof(DialogNode),
        typeof(IfNode),
        typeof(JumpNode),
        typeof(StateTransitionNode),
        typeof(WhileNode)
    ];

    public static void DoForEachNodeType(
        Action chooseNode,
        Action dialogNode,
        Action ifNode,
        Action jumpNode,
        Action stateTransitionNode,
        Action whileNode)
    {
        chooseNode();
        dialogNode();
        ifNode();
        jumpNode();
        stateTransitionNode();
        whileNode();
    }

    private static readonly PropertyExhaustivityFunctions<NodeLikePropertyAttribute> _propertyExhaustivityFunctions = new();
    private static readonly TypeExhaustivityFunctions _typeExhaustivityFunctions = new(AllNodeTypes, typeof(NodeLikeAttribute<,>));

    public static void DoForEachNodeProperty<TNode>(TNode node, Action<string, object?> callback)
    {
        _propertyExhaustivityFunctions.DoForEachProperty<TNode>(node, callback);
    }

    public static void AssertInterfaceExhaustivelyImplemented<TInterfaceType>()
    {
        _typeExhaustivityFunctions.AssertTypeExhaustivelyImplemented<TInterfaceType>();
    }

}
