﻿using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Exhaustiveness.Common;
using ScenarioModel.Objects.ScenarioNodes;

namespace ScenarioModel.Exhaustiveness;

public static class ScenarioNodeExhaustivity
{
    public static Type[] AllNodeTypes =>
    [
        typeof(ChooseNode),
        typeof(DialogNode),
        typeof(IfNode),
        typeof(JumpNode),
        typeof(TransitionNode),
        typeof(WhileNode)
    ];

    public static void DoForEachNodeType(
        Action chooseNode,
        Action dialogNode,
        Action ifNode,
        Action jumpNode,
        Action transitionNode,
        Action whileNode)
    {
        chooseNode();
        dialogNode();
        ifNode();
        jumpNode();
        transitionNode();
        whileNode();
    }

    private static readonly PropertyExhaustivityFunctions<NodeLikePropertyAttribute> _propertyExhaustivityFunctions = new();
    private static readonly TypeExhaustivityFunctions _typeExhaustivityFunctions = new(AllNodeTypes, typeof(NodeLikeAttribute<,>), "node");

    public static void DoForEachNodeProperty<TNode>(TNode node, Action<string, object?> callback)
    {
        _propertyExhaustivityFunctions.DoForEachProperty<TNode>(node, callback);
    }

    public static void AssertInterfaceExhaustivelyImplemented<TInterfaceType>()
    {
        _typeExhaustivityFunctions.AssertTypeExhaustivelyImplemented<TInterfaceType>();
    }

}
