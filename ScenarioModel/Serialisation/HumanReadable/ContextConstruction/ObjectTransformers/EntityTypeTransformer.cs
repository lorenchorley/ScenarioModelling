using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, EntityType>]
public class EntityTypeTransformer(System System, Instanciator Instanciator, StateMachineTransformer StateMachineTransformer) : IDefinitionToObjectTransformer<EntityType, EntityTypeReference>
{
    public Option<EntityTypeReference> Transform(Definition def)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.IsEqv("EntityType"))
        {
            return null;
        }

        EntityType value = Instanciator.New<EntityType>(definition: def);

        value.StateMachine.SetReference(unnamed.Definitions.Choose(StateMachineTransformer.Transform).FirstOrDefault());

        return value.GenerateReference();
    }

    public void Validate(EntityType obj)
    {
    }
}

