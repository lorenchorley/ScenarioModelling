using ScenarioModel.Serialisation.HumanReadable.SemanticTree;
using LanguageExt;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModel.Collections;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

public class DialogNodeProfile : ISemanticNodeProfile
{
    public string Name => "Dialog".ToUpperInvariant();

    public Func<Definition, bool>? Predicate => null;

    public IScenarioNode CreateAndConfigure(Definition def, Scenario scenario, SemiLinearSubGraph<IScenarioNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> transformDefinition)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("Jump node must be unnamed definition");
        }

        DialogNode node = new();
        node.Line = def.Line;

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

