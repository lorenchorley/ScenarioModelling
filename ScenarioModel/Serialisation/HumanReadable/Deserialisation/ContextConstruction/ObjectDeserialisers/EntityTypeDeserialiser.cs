using LanguageExt;
using ScenarioModelling.ContextConstruction;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers;

[ObjectLike<IDefinitionToObjectDeserialiser, EntityType>]
public class EntityTypeDeserialiser(System System, Instanciator Instanciator, StateMachineDeserialiser StateMachineTransformer) : DefinitionToObjectDeserialiser<EntityType, EntityTypeReference>
{
    protected override Option<EntityTypeReference> Transform(Definition def, TransformationType type)
    {
        if (def is not UnnamedDefinition unnamed)
            return null;

        if (!unnamed.Type.Value.IsEqv("EntityType"))
            return null;

        // If this is meant to be the value of a property in another object, we need to return a reference
        // Otherwise we make a full object that is stored in the system
        if (type == TransformationType.Property)
            return Instanciator.NewReference<EntityType, EntityTypeReference>(definition: def);

        // Need to know if we're on a definition that is a property of an object, or a definition that is an object itself
        EntityType value = Instanciator.New<EntityType>(definition: def);

        value.StateMachine.SetReference(unnamed.Definitions.Choose(StateMachineTransformer.TransformAsProperty).FirstOrDefault());

        return value.GenerateReference();
    }

    public override void BeforeIndividualInitialisation()
    {

    }

    public override void Initialise(EntityType obj)
    {
    }
}

