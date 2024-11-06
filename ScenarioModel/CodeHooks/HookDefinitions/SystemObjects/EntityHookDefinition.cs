using ScenarioModel.Objects.SystemObjects.Entities;
using ScenarioModel.References;

namespace ScenarioModel.CodeHooks.HookDefinitions.SystemObjects;

public class EntityHookDefinition : IObjectHookDefinition
{
    public HookExecutionMode HookExecutionMode { get; set; }
    public Entity Entity { get; private set; }

    public EntityHookDefinition(System system, string name)
    {
        // Either create a new one or find an existing one in the provided system
        Entity = new EntityReference() { EntityName = name }
            .ResolveReference(system)
            .Match(
                Some: e => e,
                None: () => New(system, name));
    }

    private Entity New(System system, string name)
        => new Entity(system)
        {
            Name = name
        };

    public EntityHookDefinition SetState(string stateName)
    {
        // Either set the state or verify that the states match
        Entity.State.Set(new StateReference() { StateName = stateName });

        return this;
    }
}
