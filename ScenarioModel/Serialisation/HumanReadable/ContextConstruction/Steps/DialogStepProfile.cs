using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using LanguageExt;
using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.Steps;

public class DialogStepProfile : ISemanticStepProfile
{
    public string Name => "Dialog".ToUpperInvariant();

    public Func<Definition, bool>? Predicate => null;

    public IScenarioNode CreateAndConfigure(Definition def, Scenario scenario, Func<Definition, Option<IScenarioNode>> transformDefinition)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("Jump node must be unnamed definition");
        }

        DialogNode node = new();

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

