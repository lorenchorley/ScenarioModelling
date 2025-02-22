using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, Aspect>]
public class AspectSerialiser : IObjectSerialiser<Aspect>
{
    public void WriteObject(StringBuilder sb, MetaState metaState, Aspect obj, string currentIndent)
    {
        sb.AppendLine($"{currentIndent}Aspect {obj.Name.AddQuotes()} {{");

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}