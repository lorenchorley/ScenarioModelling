using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.Reserialisation;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, Constraint>]
public class ConstraintSerialiser(string IndentSegment) : IObjectSerialiser<Constraint>
{
    public void WriteObject(StringBuilder sb, System system, Constraint obj, string currentIndent)
    {
        ExpressionSerialiser visitor = new(system);
        var result = (string)obj.Condition.Accept(visitor);

        string subIndent = currentIndent + IndentSegment;
        MetaStoryNodeExhaustivity.DoForEachNodeProperty(obj, (attr, prop, value) => sb.AppendLine($"{subIndent}{prop} {value}"));

        sb.AppendLine($"{currentIndent}Constraint <{result}> {{"); // TODO Serialise the expression correctly

        sb.AppendLine($"{subIndent}Description {obj.Name.AddQuotes()}");

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

