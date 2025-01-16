using LanguageExt;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.ContextConstruction;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction;

public class MetaStoryTransformer(System System, Instanciator Instanciator) : DefinitionToObjectDeserialiser<MetaStory, MetaStory>
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

        if (!named.Type.Value.IsEqv("MetaStory"))
            return null;

        if (type == TransformationType.Property)
            throw new InvalidOperationException("MetaStorys should not be properties");

        MetaStory value = Instanciator.NewMetaStory(definition: def);

        var tryTransform = TryTransformDefinitionToNode(value);
        value.Graph.PrimarySubGraph.NodeSequence.AddRange(named.Definitions.ChooseAndAssertAllSelected(d => tryTransform(d, value.Graph.PrimarySubGraph), "Unknown node types not taken into account : {0}"));

        return value;
    }

    private Func<Definition, SemiLinearSubGraph<IStoryNode>, Option<IStoryNode>> TryTransformDefinitionToNode(MetaStory MetaStory)
        => (def, currentSubgraph) =>
        {
            var tryTransform = TryTransformDefinitionToNode(MetaStory);

            foreach (var profilePred in NodeProfilesByPredicate)
            {
                if (profilePred.Key(def))
                {
                    var node = profilePred.Value.Transform(def, MetaStory, currentSubgraph, tryTransform);

                    if (def is NamedDefinition named)
                        node.Name = named.Name.Value;

                    return Option<IStoryNode>.Some(node);
                }
            }

            if (def is ExpressionDefinition expDef)
            {
                if (NodeProfilesByName.TryGetValue(expDef.Name.Value.ToUpperInvariant(), out IDefinitionToNodeDeserialiser? profile) && profile != null)
                {
                    var node = profile.Transform(def, MetaStory, currentSubgraph, tryTransform);

                    if (def is NamedDefinition named)
                        node.Name = named.Name.Value;

                    return Option<IStoryNode>.Some(node);
                }
            }

            if (def is UnnamedDefinition unnamed)
            {
                if (NodeProfilesByName.TryGetValue(unnamed.Type.Value.ToUpperInvariant(), out IDefinitionToNodeDeserialiser? profile) && profile != null)
                {
                    var node = profile.Transform(def, MetaStory, currentSubgraph, tryTransform);

                    if (def is NamedDefinition named)
                        node.Name = named.Name.Value;

                    return Option<IStoryNode>.Some(node);
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

