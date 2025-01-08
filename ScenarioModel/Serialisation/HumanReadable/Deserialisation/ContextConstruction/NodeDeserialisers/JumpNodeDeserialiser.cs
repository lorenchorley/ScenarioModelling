using LanguageExt;
using ScenarioModel.Collections.Graph;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.NodeDeserialisers.Intefaces;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.NodeDeserialisers;

[NodeLike<IDefinitionToNodeDeserialiser, JumpNode>]
public class JumpNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [NodeLikeProperty]
    public string Name => "Jump".ToUpperInvariant();

    [NodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public IScenarioNode Transform(Definition def, Scenario scenario, SemiLinearSubGraph<IScenarioNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> transformDefinition)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("Jump node must be unnamed definition");
        }

        JumpNode node = new();
        node.Line = def.Line;

        foreach (var item in unnamed.Definitions)
        {
            if (item is NamedDefinition namedDefinition && namedDefinition.Type.Value == "Target") // We accept either an explicitly typed "Target" definition
            {
                node.Target = namedDefinition.Name.Value;
                break;
            }
            else if (item is UnnamedDefinition unnamedDefinition) // Or an unnamed definition as the target node name
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

