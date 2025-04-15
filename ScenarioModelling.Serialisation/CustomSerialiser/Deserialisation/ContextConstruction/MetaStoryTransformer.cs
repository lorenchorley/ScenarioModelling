using LanguageExt;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction;

public class MetaStoryTransformer(MetaState metaState, Instanciator Instanciator) : DefinitionToObjectDeserialiser<MetaStory, MetaStory>
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

        def.HasBeenTransformed = true;

        if (type == TransformationType.Property)
            throw new InternalLogicException("MetaStories should not be properties");

        SemiLinearSubGraph<IStoryNode> primarySubGraph = new SemiLinearSubGraph<IStoryNode>();
        MetaStory value = Instanciator.NewMetaStory(definition: def, primarySubGraph);

        var tryTransform = TryTransformDefinitionToNode(value);
        primarySubGraph.AddRangeToSequence(named.Definitions.ChooseAndAssertAllSelected(d => tryTransform(d, primarySubGraph), "Unknown node types not taken into account : {0}"));

        return value;
    }

    private Func<Definition, SemiLinearSubGraph<IStoryNode>, Option<IStoryNode>> TryTransformDefinitionToNode(MetaStory metaStory)
        => (def, currentSubgraph) =>
        {
            var tryTransform = TryTransformDefinitionToNode(metaStory);

            foreach (var profilePred in NodeProfilesByPredicate)
            {
                if (profilePred.Key(def))
                {
                    var node = profilePred.Value.Transform(def, metaStory, currentSubgraph, tryTransform);

                    if (def is NamedDefinition named)
                        node.Name = named.Name.Value;

                    return Option<IStoryNode>.Some(node);
                }
            }

            if (def is ExpressionDefinition expDef)
            {
                if (NodeProfilesByName.TryGetValue(expDef.Name.Value.ToUpperInvariant(), out IDefinitionToNodeDeserialiser? profile) && profile != null)
                {
                    var node = profile.Transform(def, metaStory, currentSubgraph, tryTransform);

                    if (def is NamedDefinition named)
                        node.Name = named.Name.Value;

                    return Option<IStoryNode>.Some(node);
                }
            }

            if (def is UnnamedDefinition unnamed)
            {
                if (NodeProfilesByName.TryGetValue(unnamed.Type.Value.ToUpperInvariant(), out IDefinitionToNodeDeserialiser? profile) && profile != null)
                {
                    var node = profile.Transform(def, metaStory, currentSubgraph, tryTransform);

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

