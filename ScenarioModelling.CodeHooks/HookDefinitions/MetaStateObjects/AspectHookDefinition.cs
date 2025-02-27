using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.Serialisation.ContextConstruction;

namespace ScenarioModelling.CodeHooks.HookDefinitions.MetaStateObjects;

public class AspectHookDefinition : IObjectHookDefinition
{
    private readonly MetaState _metaState;
    private readonly Instanciator _instanciator;

    public HookExecutionMode HookExecutionMode { get; set; }
    public Entity Entity { get; private set; }
    public Aspect Aspect { get; private set; }
    public bool Validated { get; private set; } = false;

    public AspectHookDefinition(MetaState metaState, Instanciator instanciator, Entity entity, string name)
    {
        _metaState = metaState;
        _instanciator = instanciator;
        Entity = entity;

        // Either create a new one or find an existing one in the provided system
        Aspect = _instanciator.GetOrNew<Aspect, AspectReference>(name);
        Aspect.Entity.SetReference(entity.GenerateReference());
        Entity.Aspects.TryAddReference(Aspect.GenerateReference());
    }

    public AspectHookDefinition SetState(string stateName)
    {
        // TODO Either set the state or verify that the states match
        StateReference reference = new StateReference(_metaState) { Name = stateName };
        Aspect.State.SetReference(reference);

        if (!Aspect.InitialState.IsSet)
            Aspect.InitialState.SetReference(reference);

        return this;
    }

    public void Validate()
    {
        Validated = true;
    }

}
