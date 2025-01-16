using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, EntityType>]
public class EntityTypeSerialiser(string IndentSegment) : IObjectSerialiser<EntityType>
{
    public void WriteObject(StringBuilder sb, System system, EntityType obj, string currentIndent)
    {
        if (!obj.ShouldReserialise)
            return;

        string subIndent = currentIndent + IndentSegment;

        sb.AppendLine($"{currentIndent}EntityType {obj.Name.AddQuotes()} {{");

        if (obj.StateMachine.ResolvedValue != null)
            sb.AppendLine($"{subIndent}SM {obj.StateMachine.Name?.AddQuotes() ?? ""}");

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

