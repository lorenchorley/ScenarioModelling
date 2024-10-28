using ScenarioModel.Serialisation.HumanReadable.SemanticTree;
using LanguageExt;
using ScenarioModel.Objects.Scenario;

namespace ScenarioModel.Serialisation.HumanReadable;

public interface ISemanticStepProfile
{
    string Name { get; }
    Func<Definition, bool>? Predicate { get; }
    IScenarioNode CreateAndConfigure(Definition def, Scenario scenario, Func<Definition, Option<IScenarioNode>> transformDefinition);
}

