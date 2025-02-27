using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers;

[MetaStateObjectLike<IObjectSerialiser, Aspect>]
public class AspectSerialiser(StateSerialiser StateSerialiser) : IObjectSerialiser<Aspect>
{
    public void WriteObject(StringBuilder sb, MetaState metaState, Aspect obj, string currentIndent)
    {
        string subIndent = currentIndent + CustomContextSerialiser.IndentSegment;

        sb.AppendLine($"{currentIndent}Aspect {obj.Name.AddQuotes()} {{");

        if (obj.State.ResolvedValue != null)
            StateSerialiser.WriteObject(sb, metaState, obj.State.ResolvedValue, subIndent);

        sb.AppendLine($"{currentIndent}}}");
    }
}