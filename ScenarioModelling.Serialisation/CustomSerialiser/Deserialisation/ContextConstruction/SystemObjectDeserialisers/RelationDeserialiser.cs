using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.GeneralisedReferences;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using Relation = ScenarioModelling.CoreObjects.MetaStateObjects.Relation;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers;

[MetaStateObjectLike<IDefinitionToObjectDeserialiser, Relation>]
public class RelationDeserialiser(MetaState MetaState, Instanciator Instanciator) : DefinitionToObjectDeserialiser<Relation, RelationReference>
{
    protected override Option<RelationReference> Transform(Definition def, TransformationType type)
    {
        if (def is not UnnamedLinkDefinition unnamed)
            return null;

        if (type == TransformationType.Object)
        {
            // Root level relation
            // TODO
        }
        else
        {
            // Relation applied to object
            // TODO
        }


        def.HasBeenTransformed = true;

        Relation value = Instanciator.New<Relation>(definition: def);

        value.LeftEntity = new RelatableObjectReference(MetaState)
        {
            Name = unnamed.Source.Value
        };

        value.RightEntity = new RelatableObjectReference(MetaState)
        {
            Name = unnamed.Destination.Value
        };

        if (MetaState.Relations.Any(e => e.IsEqv(value) && value.LeftEntity!.IsEqv(e.LeftEntity) && value.RightEntity!.IsEqv(e.RightEntity)))
        {
            // If an object of the same type with the same name already exists,
            // we remove this one and but return the object as if it we've transformed so that it doesn't get signaled as not transformed
            return value.GenerateReference();
        }

        // TODO State and InitialState

        Instanciator.AssociateWithMetaState(value);
        return value.GenerateReference();
    }

    public override void BeforeIndividualInitialisation()
    {

    }

    public override void Initialise(Relation obj)
    {
    }
}

