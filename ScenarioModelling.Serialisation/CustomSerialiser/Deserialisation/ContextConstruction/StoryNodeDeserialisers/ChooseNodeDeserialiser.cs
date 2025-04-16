using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, ChooseNode>]
public class ChooseNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "Choose".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public IStoryNode Transform(Definition def, MetaStory metaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, IStoryNode? existingCorrespondingNode, TryTransformDefinitionToNodeDelegate transformDefinition)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("Jump node must be unnamed definition");
        }

        if (existingCorrespondingNode != null && existingCorrespondingNode is not ChooseNode)
        {
            throw new Exception(@$"While trying to transform the definition ""{def}"" into a node of type {nameof(ChooseNode)}, the type did not match with an existing node of type {existingCorrespondingNode.GetType().Name} in the subgraph");
        }

        def.HasBeenTransformed = true;

        ChooseNode node = new();
        node.Line = def.Line;

        foreach (var item in unnamed.Definitions)
        {
            if (item is NamedDefinition named)
            {
                node.Choices.Add((named.Type.Value, named.Name.Value));
                item.HasBeenTransformed = true;
                continue;
            }

            if (item is UnnamedDefinition unnamedsub)
            {
                node.Choices.Add((unnamedsub.Type.Value, ""));
                item.HasBeenTransformed = true;
                continue;
            }
        }

        return node;
    }
}

