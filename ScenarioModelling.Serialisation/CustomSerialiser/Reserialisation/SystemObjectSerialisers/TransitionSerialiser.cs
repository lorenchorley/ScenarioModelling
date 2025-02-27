using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers;

[MetaStateObjectLike<IObjectSerialiser, Transition>]
public class TransitionSerialiser : IObjectSerialiser<Transition>
{
    public void WriteObject(StringBuilder sb, MetaState metaState, Transition obj, string currentIndent)
    {
        string source = obj.SourceState.ResolvedValue?.Name?.AddQuotes() ?? "";
        string destination = obj.DestinationState.ResolvedValue?.Name?.AddQuotes() ?? "";
        if (string.IsNullOrEmpty(obj.Name))
        {
            sb.AppendLine($@"{currentIndent}{source} -> {destination}");
        }
        else
            sb.AppendLine($@"{currentIndent}{source} -> {destination} : {obj.Name.AddQuotes()}");
    }
}

