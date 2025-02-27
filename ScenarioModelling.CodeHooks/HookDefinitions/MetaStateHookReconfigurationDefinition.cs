using ScenarioModelling.CodeHooks.HookDefinitions.MetaStateObjects;

namespace ScenarioModelling.CodeHooks.HookDefinitions;

public class MetaStateHookReconfigurationDefinition
{
    private readonly MetaStateHookDefinition _metaStateHookDefinition;

    public MetaStateHookReconfigurationDefinition(MetaStateHookDefinition metaStateHookDefinition)
    {
        _metaStateHookDefinition = metaStateHookDefinition;
    }

    public EntityHookDefinition RedefineEntity(string name)
    {
        EntityHookDefinition nodeDef =
            _metaStateHookDefinition.EntityDefintions
                                 .Where(d => d.Entity.Name.IsEqv(name))
                                 .FirstOrDefault()
                                 ?? throw new ArgumentNullException($"No entity already defined that corresponds to the name {name}");

        return nodeDef; // New encapsulation necessairy ?
    }

    //public StateMachineHookDefinition DefineStateMachine(string name)
    //{
    //    StateMachineHookDefinition nodeDef = new(Context.System, _instanciator, name);
    //    _stateMachineDefintions.Add(nodeDef);
    //    return nodeDef;
    //}

    //public ConstraintHookDefinition DefineConstraint(string name)
    //{
    //    ConstraintHookDefinition nodeDef = new(Context.System, _instanciator, name);
    //    _constraintDefintions.Add(nodeDef);
    //    return nodeDef;
    //}

    //internal void Initialise()
    //{
    //    foreach (var entityDefintion in EntityDefintions)
    //    {
    //        ValidateDefinition(entityDefintion);
    //        Entity newlyDefinedEntity = entityDefintion.Entity;
    //        CreateNewIfNotSet<EntityType, EntityTypeReference, EntityTypeProperty>(newlyDefinedEntity.EntityType, newlyDefinedEntity.Name);
    //        // TODO Make sure there are no duplicates
    //    }

    //    foreach (var stateMachineDefintions in StateMachineDefintions)
    //    {
    //        ValidateDefinition(stateMachineDefintions);
    //        StateMachine newlyDefinedStateMachine = stateMachineDefintions.StateMachine;
    //        StateMachine? existingCorrespondingStateMachine = Context.System.StateMachines.FirstOrDefault(e => e.Name.IsEqv(newlyDefinedStateMachine.Name));

    //        //    if (existingCorrespondingStateMachine == null)
    //        //    {
    //        //        // No worries, we add it to complete the metaState
    //        //        Context.System.StateMachines.Add(newlyDefinedStateMachine);
    //        //    }
    //        //    else
    //        //    {
    //        //        // If it exists already, we have to verify that all properties are the same
    //        //        existingCorrespondingStateMachine.AssertDeepEqv(newlyDefinedStateMachine);
    //        //    }
    //    }

    //    foreach (var constraintDefintion in _constraintDefintions)
    //    {
    //        ValidateDefinition(constraintDefintion);
    //    }

    //    //Context.System.Entities.ForEach(UpdateStateMachineOnEntity);
    //}

    //private void CreateNewIfNotSet<TObjType, TObjTypeRef, TProp>(TProp objType, string name)
    //    where TObjType : class, ISystemObject<TObjTypeRef>
    //    where TObjTypeRef : class, IReference<TObjType>
    //    where TProp : OptionalReferencableProperty<TObjType, TObjTypeRef>
    //{
    //    // Check if it has an entity type
    //    if (objType.ReferenceOnly != null &&
    //        !objType.ReferenceOnly.IsResolvable())
    //    {
    //        // Create new type unique to this entity
    //        var reference = _instanciator.NewReference<TObjType, TObjTypeRef>(name);
    //        objType.SetReference(reference);
    //    }
    //}

    ////private void Initialise(Entity newlyDefinedEntity)
    ////{
    ////    // Check if it has an entity type
    ////    if (newlyDefinedEntity.EntityType.ReferenceOnly != null &&
    ////        !newlyDefinedEntity.EntityType.ReferenceOnly.IsResolvable())
    ////    {
    ////        // Create new type unique to this entity
    ////        var reference = _instanciator.NewReference<EntityType, EntityTypeReference>(newlyDefinedEntity.Name);
    ////        newlyDefinedEntity.EntityType.SetReference(reference);
    ////    }
    ////}

    //private void ValidateDefinition(IHookDefinition definition)
    //{
    //    if (definition.Validated)
    //        throw new Exception("Definition already validated");

    //    definition.Validate();
    //}
}
