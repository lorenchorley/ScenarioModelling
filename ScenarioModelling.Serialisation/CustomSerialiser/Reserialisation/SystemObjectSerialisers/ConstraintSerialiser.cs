using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Serialisation.Expressions;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, Constraint>]
public class ConstraintSerialiser : IObjectSerialiser<Constraint>
{
    public void WriteObject(StringBuilder sb, MetaState metaState, Constraint obj, string currentIndent)
    {
        ExpressionSerialiser visitor = new(metaState);
        var result = (string)obj.Condition.Accept(visitor);

        string subIndent = currentIndent + CustomContextSerialiser.IndentSegment;

        MetaStoryNodeExhaustivity.DoForEachNodeProperty(obj, (attr, prop, value) => sb.AppendLine($"{subIndent}{prop} {value}"));

        sb.AppendLine($"{currentIndent}Constraint <{result}> {{"); // TODO Serialise the expression correctly

        sb.AppendLine($"{subIndent}Description {obj.Name.AddQuotes()}");

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

