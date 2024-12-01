using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[ObjectLike<IObjectSerialiser, State>]
public class StateSerialiser(string IndentSegment) : IObjectSerialiser<State>
{
    public void WriteObject(StringBuilder sb, System system, State obj, string currentIndent)
    {
        sb.AppendLine($"{currentIndent}State {obj.Name.AddQuotes()}");
    }
}

