using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, Aspect>]
public class AspectSerialiser(string IndentSegment) : IObjectSerialiser<Aspect>
{
    public void WriteObject(StringBuilder sb, System system, Aspect obj, string currentIndent)
    {
        sb.AppendLine($"{currentIndent}Aspect {obj.Name.AddQuotes()} {{");

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}