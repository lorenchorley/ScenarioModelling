using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Exhaustiveness.Common;
using ScenarioModelling.Objects.StoryNodes;

namespace ScenarioModelling.Exhaustiveness;

public static class MetaStoryNodeExhaustivity
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

    private static readonly PropertyExhaustivityFunctions<StoryNodeLikePropertyAttribute> _propertyExhaustivityFunctions = new();
    private static readonly TypeExhaustivityFunctions _typeExhaustivityFunctions = new(AllNodeTypes, typeof(StoryNodeLikeAttribute<,>), "node");

    public static void DoForEachNodeProperty<TNode>(TNode node, Action<StoryNodeLikePropertyAttribute, string, object?> callback)
    {
        _propertyExhaustivityFunctions.DoForEachProperty<TNode>(node, callback);
    }

    public static void AssertInterfaceExhaustivelyImplemented<TInterfaceType>()
    {
        _typeExhaustivityFunctions.AssertTypeExhaustivelyImplemented<TInterfaceType>();
    }

}
