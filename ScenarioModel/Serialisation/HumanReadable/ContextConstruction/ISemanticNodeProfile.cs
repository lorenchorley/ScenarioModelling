using LanguageExt;
using ScenarioModel.Collections;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable;

public interface ISemanticNodeProfile
{
    string Name { get; }
    Func<Definition, bool>? Predicate { get; }
    IScenarioNode CreateAndConfigure(Definition def, Scenario scenario, SemiLinearSubGraph<IScenarioNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> tryTransform);
}

