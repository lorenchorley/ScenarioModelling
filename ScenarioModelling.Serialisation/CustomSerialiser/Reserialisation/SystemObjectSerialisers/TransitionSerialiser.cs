using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, Transition>]
public class TransitionSerialiser : IObjectSerialiser<Transition>
{
    public void WriteObject(StringBuilder sb, MetaState metaState, Transition obj, string currentIndent)
    {
        if (string.IsNullOrEmpty(obj.Name))
            sb.AppendLine($@"{currentIndent}{obj.SourceState.ResolvedValue?.Name?.AddQuotes() ?? ""} -> {obj.DestinationState.ResolvedValue?.Name?.AddQuotes() ?? ""}");
        else
            sb.AppendLine($@"{currentIndent}{obj.SourceState.ResolvedValue?.Name?.AddQuotes() ?? ""} -> {obj.DestinationState.ResolvedValue?.Name?.AddQuotes() ?? ""} : {obj.Name.AddQuotes()}");
    }
}

