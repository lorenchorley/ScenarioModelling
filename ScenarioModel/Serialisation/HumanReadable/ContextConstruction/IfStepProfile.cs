using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Serialisation.HumanReadable;

public class IfStepProfile : ISemanticStepProfile
{
    public string Name => "If";

    public Func<Definition, bool>? Predicate => null;

    public IScenarioNode CreateAndConfigure(Definition def, Scenario scenario)
    {
        if (def is not ExpressionDefinition expDef)
        {
            throw new Exception("If node must be expression definition");
        }

        ChooseNode node = new();

        foreach (var item in expDef.Definitions)
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

