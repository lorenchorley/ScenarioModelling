using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.CodeHooks.HookDefinitions.SystemObjects;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.References;
using ScenarioModel.References.Interfaces;

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
        foreach (var entityDefintion in _entityDefintions)
        {
            ValidateDefinition(entityDefintion);
            Entity newlyDefinedEntity = entityDefintion.Entity;
            CreateNewIfNotSet<EntityType, EntityTypeReference, EntityTypeProperty>(newlyDefinedEntity.EntityType, newlyDefinedEntity.Name);
            // TODO Make sure there are no duplicates
        }

        foreach (var stateMachineDefintions in _stateMachineDefintions)
        {
            ValidateDefinition(stateMachineDefintions);
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

        foreach (var constraintDefintion in _constraintDefintions)
        {
            ValidateDefinition(constraintDefintion);
        }

        //Context.System.Entities.ForEach(UpdateStateMachineOnEntity);
    }

    private void CreateNewIfNotSet<TObjType, TObjTypeRef, TProp>(TProp objType, string name)
        where TObjType : class, ISystemObject<TObjTypeRef>
        where TObjTypeRef : class, IReference<TObjType>
        where TProp : OptionalReferencableProperty<TObjType, TObjTypeRef>
    {
        // Check if it has an entity type
        if (objType.ReferenceOnly != null &&
            !objType.ReferenceOnly.IsResolvable())
        {
            // Create new type unique to this entity
            var reference = _instanciator.NewReference<TObjType, TObjTypeRef>(name);
            objType.SetReference(reference);
        }
    }

    //private void Initialise(Entity newlyDefinedEntity)
    //{
    //    // Check if it has an entity type
    //    if (newlyDefinedEntity.EntityType.ReferenceOnly != null &&
    //        !newlyDefinedEntity.EntityType.ReferenceOnly.IsResolvable())
    //    {
    //        // Create new type unique to this entity
    //        var reference = _instanciator.NewReference<EntityType, EntityTypeReference>(newlyDefinedEntity.Name);
    //        newlyDefinedEntity.EntityType.SetReference(reference);
    //    }
    //}

    private void ValidateDefinition(IHookDefinition definition)
    {
        if (definition.Validated)
            throw new Exception("Definition already validated");

        definition.Validate();
    }
}
