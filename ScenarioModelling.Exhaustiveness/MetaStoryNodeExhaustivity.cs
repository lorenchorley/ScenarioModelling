﻿using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Exhaustiveness.Common;

namespace ScenarioModelling.Exhaustiveness;

public static class MetaStoryNodeExhaustivity
{
    public static Type[] AllNodeTypes =>
    [
        typeof(AssertNode),
        typeof(CallMetaStoryNode),
        typeof(ChooseNode),
        typeof(DialogNode),
        typeof(IfNode),
        typeof(JumpNode),
        typeof(LoopNode),
        typeof(MetadataNode),
        typeof(TransitionNode),
        typeof(WhileNode)
    ];

    public static void DoForEachNodeType(
        Action assert,
        Action callMetaStory,
        Action chooseNode,
        Action dialogNode,
        Action ifNode,
        Action jumpNode,
        Action loopNode,
        Action metadataNode,
        Action transitionNode,
        Action whileNode)
    {
        assert();
        callMetaStory();
        chooseNode();
        dialogNode();
        ifNode();
        jumpNode();
        loopNode();
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
