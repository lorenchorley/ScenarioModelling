using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.ObjectDeserialisers.Interfaces;

[ObjectLike<IObjectSerialiser, Entity>]
public class EntitySerialiser(string IndentSegment, StateSerialiser StateSerialiser, AspectSerialiser AspectSerialiser) : IObjectSerialiser<Entity>
{
    public void WriteObject(StringBuilder sb, System system, Entity obj, string currentIndent)
    {
        string subIndent = currentIndent + IndentSegment;

        sb.AppendLine($"{currentIndent}Entity {ContextSerialiser.AddQuotes(obj.Name)} {{");
        if (obj.EntityType.ResolvedValue != null && obj.EntityType.ResolvedValue.ShouldReserialise)
            sb.AppendLine($"{subIndent}EntityType {obj.EntityType.Name}");

        if (obj.State.ResolvedValue != null)
            StateSerialiser.WriteObject(sb, system, obj.State.ResolvedValue, subIndent);

        foreach (var aspectType in obj.Aspects)
        {
            AspectSerialiser.WriteObject(sb, system, aspectType, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

