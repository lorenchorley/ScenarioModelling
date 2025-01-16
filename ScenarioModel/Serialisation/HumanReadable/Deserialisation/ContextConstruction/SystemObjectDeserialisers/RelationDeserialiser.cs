using LanguageExt;
using ScenarioModelling.ContextConstruction;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.References;
using ScenarioModelling.References.GeneralisedReferences;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;
using Relation = ScenarioModelling.Objects.SystemObjects.Relation;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.SystemObjectDeserialisers;

[SystemObjectLike<IDefinitionToObjectDeserialiser, Relation>]
public class RelationDeserialiser(System System, Instanciator Instanciator) : DefinitionToObjectDeserialiser<Relation, RelationReference>
{
    protected override Option<RelationReference> Transform(Definition def, TransformationType type)
    {
        if (def is not UnnamedLinkDefinition unnamed)
            return null;

        if (type == TransformationType.Object)
        {
            // Root level relation

        }
        else
        {
            // Relation applied to object

        }


        Relation value = Instanciator.New<Relation>(definition: def);

        value.LeftEntity = new RelatableObjectReference(System)
        {
            Name = unnamed.Source.Value
        };

        value.RightEntity = new RelatableObjectReference(System)
        {
            Name = unnamed.Destination.Value
        };

        // TODO State and InitialState

        return value.GenerateReference();
    }

    public override void BeforeIndividualInitialisation()
    {

    }

    public override void Initialise(Relation obj)
    {
    }
}

