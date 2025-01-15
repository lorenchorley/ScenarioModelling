using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation;

public class SystemSerialiser
{
    private readonly string _indentSegment;
    private readonly AspectSerialiser _aspectSerialiser;
    private readonly ConstraintSerialiser _constraintSerialiser;
    private readonly EntitySerialiser _entitySerialiser;
    private readonly EntityTypeSerialiser _entityTypeSerialiser;
    private readonly RelationSerialiser _relationSerialiser;
    private readonly StateMachineSerialiser _stateMachineSerialiser;
    private readonly StateSerialiser _stateSerialiser;
    private readonly TransitionSerialiser _transitionSerialiser;

    public SystemSerialiser(string indentSegment)
    {
        _indentSegment = indentSegment;

        // TODO Place somewhere centralised
        SystemObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IObjectSerialiser>();

        _aspectSerialiser = new(indentSegment);
        _constraintSerialiser = new(indentSegment);
        _entityTypeSerialiser = new(indentSegment);
        _relationSerialiser = new(indentSegment);
        _stateSerialiser = new(indentSegment);
        _transitionSerialiser = new(indentSegment);
        _entitySerialiser = new(indentSegment, _stateSerialiser, _aspectSerialiser);
        _stateMachineSerialiser = new(indentSegment, _stateSerialiser, _transitionSerialiser);
    }

    public void WriteSystem(StringBuilder sb, System system, string currentIndent)
    {
        foreach (var entity in system.Entities)
        {
            _entitySerialiser.WriteObject(sb, system, entity, currentIndent);
        }

        foreach (var entityType in system.EntityTypes)
        {
            _entityTypeSerialiser.WriteObject(sb, system, entityType, currentIndent);
        }

        foreach (var stateMachine in system.StateMachines)
        {
            _stateMachineSerialiser.WriteObject(sb, system, stateMachine, currentIndent);
        }

        foreach (var relation in system.Relations)
        {
            _relationSerialiser.WriteObject(sb, system, relation, currentIndent);
        }

        foreach (var constraint in system.Constraints)
        {
            _constraintSerialiser.WriteObject(sb, system, constraint, currentIndent);
        }
    }
}

