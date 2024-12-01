using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[ObjectLike<IObjectSerialiser, EntityType>]
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

