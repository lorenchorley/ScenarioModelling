using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.ObjectDeserialisers.Interfaces;

[ObjectLike<IObjectSerialiser, EntityType>]
public class EntityTypeSerialiser(string IndentSegment) : IObjectSerialiser<EntityType>
{
    public void WriteObject(StringBuilder sb, System system, EntityType obj, string currentIndent)
    {
        if (!obj.ShouldReserialise)
            return;

        string subIndent = currentIndent + IndentSegment;

        sb.AppendLine($"{currentIndent}EntityType {ContextSerialiser.AddQuotes(obj.Name)} {{");

        if (obj.StateMachine.ResolvedValue != null)
            sb.AppendLine($"{subIndent}SM {ContextSerialiser.AddQuotes(obj.StateMachine.Name ?? "")}");

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

