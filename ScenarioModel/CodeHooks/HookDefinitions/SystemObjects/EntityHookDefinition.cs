using ScenarioModel.ContextConstruction;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;

namespace ScenarioModel.CodeHooks.HookDefinitions.SystemObjects;

public class EntityHookDefinition : IObjectHookDefinition
{
    private readonly System _system;
    private readonly Instanciator _instanciator;

    public HookExecutionMode HookExecutionMode { get; set; }
    public Entity Entity { get; private set; }

    public EntityHookDefinition(System system, Instanciator instanciator, string name)
    {
        _system = system;
        _instanciator = instanciator;

        // Either create a new one or find an existing one in the provided system
        Entity = _instanciator.GetOrNew<Entity, EntityReference>(name);
        //Entity = new EntityReference(system) { Name = name }
        //    .ResolveReference()
        //    .Match(
        //        Some: e => e,
        //        None: () => _instanciator.New<Entity>(name: name));
    }

    public EntityHookDefinition SetState(string stateName)
    {
        // Either set the state or verify that the states match
        Entity.State.SetReference(new StateReference(_system) { Name = stateName });

        return this;
    }
}
