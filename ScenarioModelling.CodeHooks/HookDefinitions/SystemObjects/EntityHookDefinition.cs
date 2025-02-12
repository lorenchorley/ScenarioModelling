using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Serialisation.ContextConstruction;

namespace ScenarioModelling.CodeHooks.HookDefinitions.SystemObjects;

public class EntityHookDefinition : IObjectHookDefinition
{
    private readonly MetaState _metaState;
    private readonly Instanciator _instanciator;

    public HookExecutionMode HookExecutionMode { get; set; }
    public Entity Entity { get; private set; }
    public bool Validated { get; private set; } = false;

    public EntityHookDefinition(MetaState metaState, Instanciator instanciator, string name)
    {
        _metaState = metaState;
        _instanciator = instanciator;

        // Either create a new one or find an existing one in the provided system
        Entity = _instanciator.GetOrNew<Entity, EntityReference>(name);
    }

    public EntityHookDefinition SetState(string stateName)
    {
        // TODO Either set the state or verify that the states match
        StateReference reference = new StateReference(_metaState) { Name = stateName };
        Entity.State.SetReference(reference);
        Entity.InitialState.SetReference(reference);

        return this;
    }

    public EntityHookDefinition SetType(string entityTypeName)
    {
        // TODO Either set the type or verify that the types match
        Entity.EntityType.SetReference(new EntityTypeReference(_metaState) { Name = entityTypeName });

        return this;
    }

    public EntityHookDefinition SetCharacterStyle(string style)
    {
        Entity.CharacterStyle = style;
        return this;
    }

    public void Validate()
    {
        Validated = true;
    }
}
