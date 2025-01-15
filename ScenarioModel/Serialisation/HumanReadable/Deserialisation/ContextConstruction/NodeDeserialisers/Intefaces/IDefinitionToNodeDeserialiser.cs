using LanguageExt;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.NodeDeserialisers.Intefaces;

public interface IDefinitionToNodeDeserialiser
{
    string Name { get; }
    Func<Definition, bool>? Predicate { get; }
    IScenarioNode Transform(Definition def, MetaStory scenario, SemiLinearSubGraph<IScenarioNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> tryTransform);
}

