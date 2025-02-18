using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation;

public class MetaStateSerialiser
{
    private readonly AspectSerialiser _aspectSerialiser;
    private readonly ConstraintSerialiser _constraintSerialiser;
    private readonly EntitySerialiser _entitySerialiser;
    private readonly EntityTypeSerialiser _entityTypeSerialiser;
    private readonly RelationSerialiser _relationSerialiser;
    private readonly StateMachineSerialiser _stateMachineSerialiser;
    private readonly StateSerialiser _stateSerialiser;
    private readonly TransitionSerialiser _transitionSerialiser;

    public MetaStateSerialiser(AspectSerialiser aspectSerialiser, ConstraintSerialiser constraintSerialiser, EntitySerialiser entitySerialiser, EntityTypeSerialiser entityTypeSerialiser, RelationSerialiser relationSerialiser, StateMachineSerialiser stateMachineSerialiser, StateSerialiser stateSerialiser, TransitionSerialiser transitionSerialiser)
    {
        _aspectSerialiser = aspectSerialiser;
        _constraintSerialiser = constraintSerialiser;
        _entitySerialiser = entitySerialiser;
        _entityTypeSerialiser = entityTypeSerialiser;
        _relationSerialiser = relationSerialiser;
        _stateMachineSerialiser = stateMachineSerialiser;
        _stateSerialiser = stateSerialiser;
        _transitionSerialiser = transitionSerialiser;
    }

    public void WriteSystem(StringBuilder sb, MetaState metaState, string currentIndent)
    {
        foreach (var entity in metaState.Entities)
        {
            _entitySerialiser.WriteObject(sb, metaState, entity, currentIndent);
        }

        foreach (var entityType in metaState.EntityTypes)
        {
            _entityTypeSerialiser.WriteObject(sb, metaState, entityType, currentIndent);
        }

        foreach (var stateMachine in metaState.StateMachines)
        {
            _stateMachineSerialiser.WriteObject(sb, metaState, stateMachine, currentIndent);
        }

        foreach (var relation in metaState.Relations)
        {
            _relationSerialiser.WriteObject(sb, metaState, relation, currentIndent);
        }

        foreach (var constraint in metaState.Constraints)
        {
            _constraintSerialiser.WriteObject(sb, metaState, constraint, currentIndent);
        }
    }
}

