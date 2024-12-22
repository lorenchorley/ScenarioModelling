using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.Reserialisation;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[ObjectLike<IObjectSerialiser, Constraint>]
public class ConstraintSerialiser(string IndentSegment) : IObjectSerialiser<Constraint>
{
    public void WriteObject(StringBuilder sb, System system, Constraint obj, string currentIndent)
    {
        ExpressionSerialiser visitor = new(system);
        var result = (string)obj.Condition.Accept(visitor);

        string subIndent = currentIndent + IndentSegment;
        ScenarioNodeExhaustivity.DoForEachNodeProperty(obj, (prop, value) => sb.AppendLine($"{subIndent}{prop} {value}"));

        sb.AppendLine($"{currentIndent}Constraint <{result}> {{"); // TODO Serialise the expression correctly

        sb.AppendLine($"{subIndent}Description {obj.Name.AddQuotes()}");

        sb.AppendLine($"{currentIndent}}}");
    }
}

