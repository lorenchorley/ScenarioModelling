using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[ObjectLike<IObjectSerialiser, Constraint>]
public class ConstraintSerialiser(string IndentSegment) : IObjectSerialiser<Constraint>
{
    public void WriteObject(StringBuilder sb, System system, Constraint obj, string currentIndent)
    {
    }
}

