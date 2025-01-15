using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[ObjectLike<IObjectSerialiser, StateMachine>]
public class StateMachineSerialiser(string IndentSegment, StateSerialiser StateSerialiser, TransitionSerialiser TransitionSerialiser) : IObjectSerialiser<StateMachine>
{
    public void WriteObject(StringBuilder sb, System system, StateMachine obj, string currentIndent)
    {
        if (!obj.ShouldReserialise)
            return;

        string subIndent = currentIndent + IndentSegment;

        sb.AppendLine($"{currentIndent}SM {obj.Name.AddQuotes()} {{");

        foreach (var state in obj.States)
        {
            StateSerialiser.WriteObject(sb, system, state, subIndent);
        }

        foreach (var transition in obj.Transitions)
        {
            TransitionSerialiser.WriteObject(sb, system, transition, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

