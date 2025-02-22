using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, State>]
public class StateSerialiser : IObjectSerialiser<State>
{
    public void WriteObject(StringBuilder sb, MetaState metaState, State obj, string currentIndent)
    {
        sb.AppendLine($"{currentIndent}State {obj.Name.AddQuotes()}");
    }
}

