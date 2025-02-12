using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, StateMachine>]
public class StateMachineSerialiser(StateSerialiser StateSerialiser, TransitionSerialiser TransitionSerialiser) : IObjectSerialiser<StateMachine>
{
    public void WriteObject(StringBuilder sb, MetaState metaState, StateMachine obj, string currentIndent)
    {
        if (!obj.ShouldReserialise)
            return;

        string subIndent = currentIndent + ContextSerialiser.IndentSegment;

        sb.AppendLine($"{currentIndent}StateMachine {obj.Name.AddQuotes()} {{");

        foreach (var state in obj.States)
        {
            StateSerialiser.WriteObject(sb, metaState, state, subIndent);
        }

        foreach (var transition in obj.Transitions)
        {
            TransitionSerialiser.WriteObject(sb, metaState, transition, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}