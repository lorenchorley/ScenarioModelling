using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.Reserialisation;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.NodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.NodeSerialisers;

[NodeLike<INodeSerialiser, WhileNode>]
public class WhileNodeSerialiser(string IndentSegment, ScenarioSerialiser ScenarioSerialiser) : INodeSerialiser<WhileNode>
{
    public void WriteNode(StringBuilder sb, MetaStory scenario, WhileNode node, string currentIndent)
    {
        ExpressionSerialiser visitor = new(scenario.System);
        var result = (string)node.Condition.Accept(visitor);

        sb.AppendLine($"{currentIndent}While <{result}> {{");

        string subIndent = currentIndent + IndentSegment;

        node.SerialiseAnnotatedProperties(sb, subIndent);

        foreach (var subNode in node.SubGraph.NodeSequence)
        {
            ScenarioSerialiser.WriteScenarioNode(sb, scenario, subNode, subIndent);
        }

        sb.AppendLine($"{currentIndent}}}");
    }
}

