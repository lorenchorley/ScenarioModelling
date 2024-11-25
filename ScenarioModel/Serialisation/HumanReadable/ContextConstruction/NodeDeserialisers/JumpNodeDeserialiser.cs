using LanguageExt;
using ScenarioModel.Collections;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeDeserialisers.Intefaces;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeDeserialisers;

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

