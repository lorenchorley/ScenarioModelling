using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, Entity>]
public class EntitySerialiser(StateSerialiser StateSerialiser, AspectSerialiser AspectSerialiser) : IObjectSerialiser<Entity>
{
    public void WriteObject(StringBuilder sb, MetaState metaState, Entity obj, string currentIndent)
    {
        string subIndent = currentIndent + ContextSerialiser.IndentSegment;

        sb.AppendLine($"{currentIndent}Entity {obj.Name.AddQuotes()} {{");

        if (obj.EntityType.ResolvedValue != null && obj.EntityType.ResolvedValue.ShouldReserialise)
            sb.AppendLine($"{subIndent}EntityType {obj.EntityType.Name}");

        if (obj.State.ResolvedValue != null)
            StateSerialiser.WriteObject(sb, metaState, obj.State.ResolvedValue, subIndent);

        if (!string.IsNullOrEmpty(obj.CharacterStyle))
            sb.AppendLine($@"{subIndent}CharacterStyle {obj.CharacterStyle.AddQuotes()}");

        foreach (var aspectType in obj.Aspects)
        {
            AspectSerialiser.WriteObject(sb, metaState, aspectType, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

