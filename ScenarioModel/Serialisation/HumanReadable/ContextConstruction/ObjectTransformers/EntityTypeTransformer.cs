using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, EntityType>]
public class EntityTypeTransformer(System System, Instanciator Instanciator, StateMachineTransformer StateMachineTransformer) : DefinitionToObjectTransformer<EntityType, EntityTypeReference>
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

    public override void Validate(EntityType obj)
    {
    }
}

