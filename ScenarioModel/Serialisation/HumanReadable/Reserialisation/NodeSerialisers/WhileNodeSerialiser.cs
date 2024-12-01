using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.Reserialisation;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers;

[NodeLike<INodeSerialiser, WhileNode>]
public class WhileNodeSerialiser(string IndentSegment, ScenarioSerialiser ScenarioSerialiser) : INodeSerialiser<WhileNode>
{
    public void WriteNode(StringBuilder sb, Scenario scenario, WhileNode node, string currentIndent)
    {
        ExpressionSerialiser visitor = new(scenario.System);
        var result = (string)node.Condition.Accept(visitor);

        sb.AppendLine($"{currentIndent}While <{result}> {{"); // TODO Serialise the expression correctly

        string subIndent = currentIndent + IndentSegment;
        ScenarioNodeExhaustivity.DoForEachNodeProperty(node, (prop, value) => sb.AppendLine($"{subIndent}{prop} {value}"));

        foreach (var subNode in node.SubGraph.NodeSequence)
        {
            ScenarioSerialiser.WriteScenarioNode(sb, scenario, subNode, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
        //sb.AppendLine($"");
    }
}

