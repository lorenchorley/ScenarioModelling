using LanguageExt;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, DialogNode>]
public class DialogNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "Dialog".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public IStoryNode Transform(Definition def, MetaStory MetaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IStoryNode>, Option<IStoryNode>> transformDefinition)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("Jump node must be unnamed definition");
        }

        DialogNode node = new();
        node.Line = def.Line;

        foreach (var item in unnamed.Definitions)
        {
            if (item is NamedDefinition named)
            {
                if (named.Type.Value.IsEqv("Text"))
                {
                    node.TextTemplate = named.Name.Value;
                    continue;
                }

                if (named.Type.Value.IsEqv("Character") || named.Type.Value.IsEqv("Char"))
                {
                    node.Character = named.Name.Value;
                    continue;
                }
            }
        }

        return node;
    }
}

