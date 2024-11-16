using LanguageExt;
using ScenarioModel.Collections;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeTransformers;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, Scenario>]
public class ScenarioTransformer(System System, Instanciator Instanciator) : DefinitionToObjectTransformer<Scenario, Scenario>
{
    public Dictionary<string, IDefinitionToNodeTransformer> NodeProfilesByName { get; init; }
    public Dictionary<Func<Definition, bool>, IDefinitionToNodeTransformer> NodeProfilesByPredicate { get; init; }

    protected override Option<Scenario> Transform(Definition def, TransformationType type)
    {
        if (def is not NamedDefinition named)
        {
            if (def is not UnnamedDefinition unnamed)
            {
                // Report error
            }

            return null;
        }

        if (!named.Type.Value.IsEqv("Scenario"))
            return null;

        if (type == TransformationType.Property)
            throw new InvalidOperationException("Scenarios should not be properties");

        Scenario value = Instanciator.New<Scenario>(definition: def);

        var tryTransform = TryTransformDefinitionToNode(value);
        value.Graph.PrimarySubGraph.NodeSequence.AddRange(named.Definitions.ChooseAndAssertAllSelected(d => tryTransform(d, value.Graph.PrimarySubGraph), "Unknown node types not taken into account : {0}"));

        return value;
    }

    private Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> TryTransformDefinitionToNode(Scenario scenario)
        => (Definition def, SemiLinearSubGraph<IScenarioNode> currentSubgraph) =>
        {
            var tryTransform = TryTransformDefinitionToNode(scenario);

            foreach (var profilePred in NodeProfilesByPredicate)
            {
                if (profilePred.Key(def))
                {
                    var node = profilePred.Value.CreateAndConfigure(def, scenario, currentSubgraph, tryTransform);

                    if (def is NamedDefinition named)
                        node.Name = named.Name.Value;

                    return Option<IScenarioNode>.Some(node);
                }
            }

            if (def is ExpressionDefinition expDef)
            {
                if (NodeProfilesByName.TryGetValue(expDef.Name.Value.ToUpperInvariant(), out IDefinitionToNodeTransformer? profile) && profile != null)
                {
                    var node = profile.CreateAndConfigure(def, scenario, currentSubgraph, tryTransform);

                    if (def is NamedDefinition named)
                        node.Name = named.Name.Value;

                    return Option<IScenarioNode>.Some(node);
                }
            }

            if (def is UnnamedDefinition unnamed)
            {
                if (NodeProfilesByName.TryGetValue(unnamed.Type.Value.ToUpperInvariant(), out IDefinitionToNodeTransformer? profile) && profile != null)
                {
                    var node = profile.CreateAndConfigure(def, scenario, currentSubgraph, tryTransform);

                    if (def is NamedDefinition named)
                        node.Name = named.Name.Value;

                    return Option<IScenarioNode>.Some(node);
                }
            }

            return null;
        };

    public override void BeforeIndividualValidation()
    {

    }

    public override void Validate(Scenario obj)
    {
    }
}

