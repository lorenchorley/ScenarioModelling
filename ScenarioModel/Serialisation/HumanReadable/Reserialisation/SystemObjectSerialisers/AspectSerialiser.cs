using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.ObjectDeserialisers.Interfaces;

[ObjectLike<IObjectSerialiser, Aspect>]
public class AspectSerialiser(string IndentSegment) : IObjectSerialiser<Aspect>
{
    public void WriteObject(StringBuilder sb, System system, Aspect obj, string currentIndent)
    {
        sb.AppendLine($"{currentIndent}Aspect {ContextSerialiser.AddQuotes(obj.Name)} {{");

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

