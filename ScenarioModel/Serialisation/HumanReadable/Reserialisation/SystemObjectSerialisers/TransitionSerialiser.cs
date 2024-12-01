using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[ObjectLike<IObjectSerialiser, Transition>]
public class TransitionSerialiser(string IndentSegment) : IObjectSerialiser<Transition>
{
    public void WriteObject(StringBuilder sb, System system, Transition obj, string currentIndent)
    {
        if (string.IsNullOrEmpty(obj.Name))
            sb.AppendLine($@"{currentIndent}{obj.SourceState.ResolvedValue?.Name?.AddQuotes() ?? ""} -> {obj.DestinationState.ResolvedValue?.Name?.AddQuotes() ?? ""}");
        else
            sb.AppendLine($@"{currentIndent}{obj.SourceState.ResolvedValue?.Name?.AddQuotes() ?? ""} -> {obj.DestinationState.ResolvedValue?.Name?.AddQuotes() ?? ""} : {obj.Name.AddQuotes()}");
    }
}

