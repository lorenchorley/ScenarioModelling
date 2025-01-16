using LanguageExt;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;

public interface IDefinitionToNodeDeserialiser
{
    string Name { get; }
    Func<Definition, bool>? Predicate { get; }
    IStoryNode Transform(Definition def, MetaStory MetaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IStoryNode>, Option<IStoryNode>> tryTransform);
}

