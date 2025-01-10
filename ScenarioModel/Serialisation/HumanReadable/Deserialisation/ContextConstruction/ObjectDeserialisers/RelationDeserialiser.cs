using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.References;
using ScenarioModel.References.GeneralisedReferences;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;
using Relation = ScenarioModel.Objects.SystemObjects.Relation;

namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers;

[ObjectLike<IDefinitionToObjectDeserialiser, Relation>]
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

