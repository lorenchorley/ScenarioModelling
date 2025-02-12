using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, EntityType>]
public class EntityTypeSerialiser : IObjectSerialiser<EntityType>
{
    public void WriteObject(StringBuilder sb, MetaState metaState, EntityType obj, string currentIndent)
    {
        if (!obj.ShouldReserialise)
            return;

        string subIndent = currentIndent + ContextSerialiser.IndentSegment;

        sb.AppendLine($"{currentIndent}EntityType {obj.Name.AddQuotes()} {{");

        if (obj.StateMachine.ResolvedValue != null)
            sb.AppendLine($"{subIndent}StateMachine {obj.StateMachine.Name?.AddQuotes() ?? ""}");

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

