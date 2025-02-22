using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.GeneralisedReferences;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using Relation = ScenarioModelling.CoreObjects.SystemObjects.Relation;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers;

[SystemObjectLike<IDefinitionToObjectDeserialiser, Relation>]
public class RelationDeserialiser(MetaState MetaState, Instanciator Instanciator) : DefinitionToObjectDeserialiser<Relation, RelationReference>
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

        def.HasBeenTransformed = true;

        value.LeftEntity = new RelatableObjectReference(MetaState)
        {
            Name = unnamed.Source.Value
        };

        value.RightEntity = new RelatableObjectReference(MetaState)
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

