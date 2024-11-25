using LanguageExt;
using ScenarioModel.Collections;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeDeserialisers.Intefaces;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeDeserialisers;

[NodeLike<IDefinitionToNodeDeserialiser, DialogNode>]
public class DialogNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [NodeLikeProperty]
    public string Name => "Dialog".ToUpperInvariant();

    [NodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public IScenarioNode Transform(Definition def, Scenario scenario, SemiLinearSubGraph<IScenarioNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> transformDefinition)
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

