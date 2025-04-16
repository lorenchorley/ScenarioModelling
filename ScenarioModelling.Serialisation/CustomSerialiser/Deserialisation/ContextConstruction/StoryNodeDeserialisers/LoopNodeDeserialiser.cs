using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Serialisation.Expressions.Interpreter;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.Tools.Exceptions;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, LoopNode>]
public class LoopNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "Loop".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public IStoryNode Transform(Definition def, MetaStory metaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, IStoryNode? existingCorrespondingNode, TryTransformDefinitionToNodeDelegate tryTransform)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("Loop node must be expression definition");
        }

        if (existingCorrespondingNode != null && existingCorrespondingNode is not LoopNode)
        {
            throw new Exception(@$"While trying to transform the definition ""{def}"" into a node of type {nameof(LoopNode)}, the type did not match with an existing node of type {existingCorrespondingNode.GetType().Name} in the subgraph");
        }

        LoopNode node = new();
        node.Line = def.Line;

        def.HasBeenTransformed = true;

        var subgraphToUse =
            existingCorrespondingNode != null
            ? ((LoopNode)existingCorrespondingNode).SubGraph // If there's an existing node, we take it's subgraph so as to be able to compare the transformed nodes with the existing ones the whole way down
            : node.SubGraph; // If there is no existing node, we use the new one

        subgraphToUse.TransformAndMergeDefinitionsIntoSubgraph(unnamed, tryTransform);

        return node;
    }
}

