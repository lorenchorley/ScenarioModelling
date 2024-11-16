using LanguageExt;
using ScenarioModel.Collections;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeTransformers;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[NodeLike<IDefinitionToNodeTransformer, ChooseNode>]
public class ChooseNodeProfile : IDefinitionToNodeTransformer
{
    [NodeLikeProperty]
    public string Name => "Choose".ToUpperInvariant();

    [NodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public IScenarioNode CreateAndConfigure(Definition def, Scenario scenario, SemiLinearSubGraph<IScenarioNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> transformDefinition)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            throw new Exception("Jump node must be unnamed definition");
        }

        ChooseNode node = new();
        node.Line = def.Line;

        foreach (var item in unnamed.Definitions)
        {
            if (item is NamedDefinition named)
            {
                node.Choices.Add((named.Type.Value, named.Name.Value));
            }
            else if (item is UnnamedDefinition unnamedsub)
            {
                node.Choices.Add((unnamedsub.Type.Value, ""));
            }
        }

        return node;
    }
}

