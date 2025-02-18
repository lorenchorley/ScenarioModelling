using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, ChooseNode>]
public class ChooseNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "Choose".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public IStoryNode Transform(Definition def, MetaStory metaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IStoryNode>, Option<IStoryNode>> transformDefinition)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("Jump node must be unnamed definition");
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

