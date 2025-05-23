﻿using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, MetadataNode>]
public class MetadataNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "Metadata".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public IStoryNode Transform(Definition def, MetaStory metaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, IStoryNode? existingCorrespondingNode, TryTransformDefinitionToNodeDelegate transformDefinition)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("Metadata node must be unnamed definition");
        }

        if (existingCorrespondingNode != null && existingCorrespondingNode is not MetadataNode)
        {
            throw new Exception(@$"While trying to transform the definition ""{def}"" into a node of type {nameof(MetadataNode)}, the type did not match with an existing node of type {existingCorrespondingNode.GetType().Name} in the subgraph");
        }

        throw new NotImplementedException();

        MetadataNode node = new();
        node.Line = def.Line;

        def.HasBeenTransformed = true;

        foreach (var item in unnamed.Definitions)
        {
            //if (item is NamedDefinition namedDefinition && namedDefinition.Type.Value == "Target") // We accept either an explicitly typed "Target" definition
            //{
            //    node.Target = namedDefinition.Name.Value;
            //    break;
            //}
            //else if (item is UnnamedDefinition unnamedDefinition) // Or an unnamed definition as the target node name
            //{
            //    node.Target = unnamedDefinition.Type.Value;
            //    break;
            //}
        }

        //if (string.IsNullOrEmpty(node.Target))
        //{
        //    throw new Exception("Target not set on metadata node");
        //}

        return node;
    }
}

