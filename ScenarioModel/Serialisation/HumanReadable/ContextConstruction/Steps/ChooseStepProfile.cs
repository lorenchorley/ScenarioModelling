using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using LanguageExt;
using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.Steps;

public class ChooseStepProfile : ISemanticStepProfile
{
    public string Name => "Choose".ToUpperInvariant();

    public Func<Definition, bool>? Predicate => null;

    public IScenarioNode CreateAndConfigure(Definition def, Scenario scenario, Func<Definition, Option<IScenarioNode>> transformDefinition)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("Jump node must be unnamed definition");
        }

        ChooseNode node = new();

        foreach (var item in unnamed.Definitions)
        {
            if (item is NamedDefinition named)
            {
                node.Choices.Add((named.Type.Value, named.Name.Value));
            }
            else if (item is UnnamedDefinition unnamedsub)
            {
                node.Choices.Add((unnamedsub.Type.Value, unnamedsub.Type.Value));
            }
        }

        return node;
    }
}

