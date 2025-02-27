using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Properties;

public class StateMachineProperty(MetaState System) : OptionalReferencableProperty<StateMachine, StateMachineReference>(System)
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
