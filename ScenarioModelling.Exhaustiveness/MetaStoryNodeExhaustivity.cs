using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.Exhaustiveness.Common;

namespace ScenarioModelling.Exhaustiveness;

public static class MetaStoryNodeExhaustivity
{
    public static Type[] AllNodeTypes =>
    [
        typeof(ChooseNode),
        typeof(DialogNode),
        typeof(IfNode),
        typeof(JumpNode),
        typeof(MetadataNode),
        typeof(TransitionNode),
        typeof(WhileNode)
    ];

    public static void DoForEachNodeType(
        Action chooseNode,
        Action dialogNode,
        Action ifNode,
        Action jumpNode,
        Action metadataNode,
        Action transitionNode,
        Action whileNode)
    {
        chooseNode();
        dialogNode();
        ifNode();
        jumpNode();
        metadataNode();
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
        _typeExhaustivityFunctions.AssertThatInterfaceIsExhaustivelyImplemented<TInterfaceType>();
    }

}
