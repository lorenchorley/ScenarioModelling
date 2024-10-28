using ScenarioModel.Serialisation.HumanReadable.SemanticTree;
using LanguageExt;
using ScenarioModel.Objects.Scenario;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.Steps;

public class JumpStepProfile : ISemanticStepProfile
{
    public string Name => "Jump".ToUpperInvariant();

    public Func<Definition, bool>? Predicate => null;

    public IScenarioNode CreateAndConfigure(Definition def, Scenario scenario, Func<Definition, Option<IScenarioNode>> transformDefinition)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("Jump node must be unnamed definition");
        }

        JumpNode node = new();

        foreach (var item in unnamed.Definitions)
        {
            if (item is UnnamedDefinition unnamedDefinition)
            {
                node.Target = unnamedDefinition.Type.Value;
                break;
            }
        }

        if (string.IsNullOrEmpty(node.Target))
        {
            throw new Exception("Target not set on jump node");
        }

        return node;
    }
}

