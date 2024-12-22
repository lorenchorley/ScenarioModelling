using ScenarioModel.CodeHooks.HookDefinitions.SystemObjects;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;

namespace ScenarioModel.CodeHooks.HookDefinitions;

public class SystemHookDefinition
{
    public Context Context { get; }
    private readonly Instanciator _instanciator;
    protected List<EntityHookDefinition> _entityDefintions = new();
    protected List<StateMachineHookDefinition> _stateMachineDefintions = new();
    protected List<ConstraintHookDefinition> _constraintDefintions = new();

    public SystemHookDefinition(Context context)
    {
        Context = context;
        _instanciator = new Instanciator(context.System);
    }

    public EntityHookDefinition DefineEntity(string name)
    {
        EntityHookDefinition nodeDef = new(Context.System, _instanciator, name);
        _entityDefintions.Add(nodeDef);
        return nodeDef;
    }

    public StateMachineHookDefinition DefineStateMachine(string name)
    {
        StateMachineHookDefinition nodeDef = new(Context.System, _instanciator, name);
        _stateMachineDefintions.Add(nodeDef);
        return nodeDef;
    }

    public ConstraintHookDefinition DefineConstraint(string name)
    {
        ConstraintHookDefinition nodeDef = new(Context.System, _instanciator, name);
        _constraintDefintions.Add(nodeDef);
        return nodeDef;
    }

    internal void Initialise()
    {
        foreach (var entityDefintions in _entityDefintions)
        {
            Entity newlyDefinedEntity = entityDefintions.Entity;
            Initialise(newlyDefinedEntity);
            // TODO Make sure there are no duplicates
        }

        foreach (var stateMachineDefintions in _stateMachineDefintions)
        {
            StateMachine newlyDefinedStateMachine = stateMachineDefintions.StateMachine;
            StateMachine? existingCorrespondingStateMachine = Context.System.StateMachines.FirstOrDefault(e => e.Name.IsEqv(newlyDefinedStateMachine.Name));

            //    if (existingCorrespondingStateMachine == null)
            //    {
            //        // No worries, we add it to complete the system
            //        Context.System.StateMachines.Add(newlyDefinedStateMachine);
            //    }
            //    else
            //    {
            //        // If it exists already, we have to verify that all properties are the same
            //        existingCorrespondingStateMachine.AssertDeepEqv(newlyDefinedStateMachine);
            //    }
        }

        //Context.System.Entities.ForEach(UpdateStateMachineOnEntity);
    }

    //private void UpdateStateMachineOnEntity(Entity entity)
    //{
    //    State? state = entity.State.ResolvedValue;

    //    if (state == null)
    //        return;

    //    state.
    //}

    private void Initialise(Entity newlyDefinedEntity)
    {
        // Check if it has an entity type
        if (newlyDefinedEntity.EntityType.ReferenceOnly != null &&
            !newlyDefinedEntity.EntityType.ReferenceOnly.IsResolvable())
        {
            // Create new type unique to this entity
            var reference = _instanciator.NewReference<EntityType, EntityTypeReference>(newlyDefinedEntity.Name);
            newlyDefinedEntity.EntityType.SetReference(reference);
        }
    }

}
