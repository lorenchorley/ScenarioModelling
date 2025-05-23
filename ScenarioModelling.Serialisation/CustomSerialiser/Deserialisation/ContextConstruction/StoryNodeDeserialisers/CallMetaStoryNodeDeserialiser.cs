﻿using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, CallMetaStoryNode>]
public class CallMetaStoryNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "CallMetaStory".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public IStoryNode Transform(Definition def, MetaStory metaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, IStoryNode? existingCorrespondingNode, TryTransformDefinitionToNodeDelegate transformDefinition)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("CallMetaStory node must be unnamed definition");
        }

        if (existingCorrespondingNode != null && existingCorrespondingNode is not CallMetaStoryNode)
        {
            throw new Exception(@$"While trying to transform the definition ""{def}"" into a node of type {nameof(CallMetaStoryNode)}, the type did not match with an existing node of type {existingCorrespondingNode.GetType().Name} in the subgraph");
        }

        def.HasBeenTransformed = true;

        CallMetaStoryNode node = new();
        node.Line = def.Line;

        foreach (var item in unnamed.Definitions)
        {
            if (item is NamedDefinition named && named.Type.IsEqv("MetaStoryName"))
            {
                node.MetaStoryName = named.Name.Value;
                item.HasBeenTransformed = true;
            }
        }

        return node;
    }
}

