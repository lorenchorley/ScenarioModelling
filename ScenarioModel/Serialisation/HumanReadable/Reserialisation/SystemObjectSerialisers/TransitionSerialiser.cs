using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.ObjectDeserialisers.Interfaces;

[ObjectLike<IObjectSerialiser, Transition>]
public class TransitionSerialiser(string IndentSegment) : IObjectSerialiser<Transition>
{
    public void WriteObject(StringBuilder sb, System system, Transition obj, string currentIndent)
    {
        if (string.IsNullOrEmpty(obj.Name))
            sb.AppendLine($@"{currentIndent}{ContextSerialiser.AddQuotes(obj.SourceState.ResolvedValue?.Name ?? "")} -> {ContextSerialiser.AddQuotes(obj.DestinationState.ResolvedValue?.Name ?? "")}");
        else
            sb.AppendLine($@"{currentIndent}{ContextSerialiser.AddQuotes(obj.SourceState.ResolvedValue?.Name ?? "")} -> {ContextSerialiser.AddQuotes(obj.DestinationState.ResolvedValue?.Name ?? "")} : {ContextSerialiser.AddQuotes(obj.Name)}");
    }
}

