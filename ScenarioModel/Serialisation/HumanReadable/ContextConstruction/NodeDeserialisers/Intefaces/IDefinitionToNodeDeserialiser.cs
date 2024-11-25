using LanguageExt;
using ScenarioModel.Collections;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeDeserialisers.Intefaces;

public interface IDefinitionToNodeDeserialiser
{
    string Name { get; }
    Func<Definition, bool>? Predicate { get; }
    IScenarioNode Transform(Definition def, Scenario scenario, SemiLinearSubGraph<IScenarioNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> tryTransform);
}

