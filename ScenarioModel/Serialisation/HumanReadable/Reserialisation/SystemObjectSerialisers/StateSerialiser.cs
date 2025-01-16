using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, State>]
public class StateSerialiser(string IndentSegment) : IObjectSerialiser<State>
{
    public void WriteObject(StringBuilder sb, System system, State obj, string currentIndent)
    {
        sb.AppendLine($"{currentIndent}State {obj.Name.AddQuotes()}");
    }
}

