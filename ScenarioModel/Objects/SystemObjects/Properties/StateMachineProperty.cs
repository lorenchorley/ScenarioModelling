using ScenarioModelling.References;

namespace ScenarioModelling.Objects.SystemObjects.Properties;

public class StateMachineProperty(System System) : OptionalReferencableProperty<StateMachine, StateMachineReference>(System)
{
    public override string? Name
    {
        get
        {
            return _valueOrReference?.Match(
                stateMachine => stateMachine.Name,
                reference => reference.Name
            );
        }
    }
}
