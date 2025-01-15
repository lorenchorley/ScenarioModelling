using LanguageExt;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.ContextConstruction;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.NodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction;

public class ScenarioTransformer(System System, Instanciator Instanciator) : DefinitionToObjectDeserialiser<MetaStory, MetaStory>
{
    public Dictionary<string, IDefinitionToNodeDeserialiser> NodeProfilesByName { get; set; }
    public Dictionary<Func<Definition, bool>, IDefinitionToNodeDeserialiser> NodeProfilesByPredicate { get; set; }

    protected override Option<MetaStory> Transform(Definition def, TransformationType type)
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

        MetaStory value = Instanciator.NewScenario(definition: def);

        var tryTransform = TryTransformDefinitionToNode(value);
        value.Graph.PrimarySubGraph.NodeSequence.AddRange(named.Definitions.ChooseAndAssertAllSelected(d => tryTransform(d, value.Graph.PrimarySubGraph), "Unknown node types not taken into account : {0}"));

        return value;
    }

    private Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> TryTransformDefinitionToNode(MetaStory scenario)
        => (def, currentSubgraph) =>
        {
            var tryTransform = TryTransformDefinitionToNode(scenario);

            foreach (var profilePred in NodeProfilesByPredicate)
            {
                if (profilePred.Key(def))
                {
                    var node = profilePred.Value.Transform(def, scenario, currentSubgraph, tryTransform);

                    if (def is NamedDefinition named)
                        node.Name = named.Name.Value;

                    return Option<IScenarioNode>.Some(node);
                }
            }

            if (def is ExpressionDefinition expDef)
            {
                if (NodeProfilesByName.TryGetValue(expDef.Name.Value.ToUpperInvariant(), out IDefinitionToNodeDeserialiser? profile) && profile != null)
                {
                    var node = profile.Transform(def, scenario, currentSubgraph, tryTransform);

                    if (def is NamedDefinition named)
                        node.Name = named.Name.Value;

                    return Option<IScenarioNode>.Some(node);
                }
            }

            if (def is UnnamedDefinition unnamed)
            {
                if (NodeProfilesByName.TryGetValue(unnamed.Type.Value.ToUpperInvariant(), out IDefinitionToNodeDeserialiser? profile) && profile != null)
                {
                    var node = profile.Transform(def, scenario, currentSubgraph, tryTransform);

                    if (def is NamedDefinition named)
                        node.Name = named.Name.Value;

                    return Option<IScenarioNode>.Some(node);
                }
            }

            return null;
        };

    public override void BeforeIndividualInitialisation()
    {

    }

    public override void Initialise(MetaStory obj)
    {
    }
}

