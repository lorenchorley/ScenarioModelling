using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using LanguageExt;
using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Serialisation.HumanReadable;

public interface ISemanticStepProfile
{
    string Name { get; }
    Func<Definition, bool>? Predicate { get; }
    IScenarioNode CreateAndConfigure(Definition def, Scenario scenario, Func<Definition, Option<IScenarioNode>> transformDefinition);
}

