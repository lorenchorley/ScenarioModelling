using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.ContextConstruction.ObjectDeserialisers.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers.Interfaces;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation;

public class ScenarioSerialiser
{
    private readonly string _indentSegment;
    private readonly ChooseNodeSerialiser _chooseNodeSerialiser;
    private readonly DialogNodeSerialiser _dialogNodeSerialiser;
    private readonly IfNodeSerialiser _ifNodeSerialiser;
    private readonly JumpNodeSerialiser _jumpNodeSerialiser;
    private readonly TransitionNodeSerialiser _transitionNodeSerialiser;
    private readonly WhileNodeSerialiser _whileNodeSerialiser;

    public ScenarioSerialiser(string indentSegment)
    {
        _indentSegment = indentSegment;

        // TODO Place somewhere centralised
        ScenarioNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<INodeSerialiser>();

        _chooseNodeSerialiser = new(indentSegment);
        _dialogNodeSerialiser = new(indentSegment);
        _jumpNodeSerialiser = new(indentSegment);
        _transitionNodeSerialiser = new(indentSegment);
        _ifNodeSerialiser = new(indentSegment, this);
        _whileNodeSerialiser = new(indentSegment, this);
    }

    public void WriteScenario(StringBuilder sb, Scenario scenario, string currentIndent)
    {
        sb.AppendLine($"{currentIndent}Scenario {scenario.Name} {{");

        foreach (var node in scenario.Graph.PrimarySubGraph.NodeSequence)
        {
            WriteScenarioNode(sb, scenario, node, currentIndent + _indentSegment);
        }

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }

    public void WriteScenarioNode(StringBuilder sb, Scenario scenario, IScenarioNode node, string currentIndent)
    {
        if (node is ITransitionNode transitionNode)
        {
            foreach (var target in transitionNode.TargetNodeNames)
            {
                sb.AppendLine($"{currentIndent}{node.Name} -> {target}"); // TODO Used ?
            }
            return;
        }

        node.ToOneOf().Switch(
            (ChooseNode chooseNode) => _chooseNodeSerialiser.WriteNode(sb, scenario, chooseNode, currentIndent),
            (DialogNode dialogNode) => _dialogNodeSerialiser.WriteNode(sb, scenario, dialogNode, currentIndent),
            (IfNode ifNode) => _ifNodeSerialiser.WriteNode(sb, scenario, ifNode, currentIndent),
            (JumpNode jumpNode) => _jumpNodeSerialiser.WriteNode(sb, scenario, jumpNode, currentIndent),
            (TransitionNode transitionNode) => _transitionNodeSerialiser.WriteNode(sb, scenario, transitionNode, currentIndent),
            (WhileNode whileNode) => _whileNodeSerialiser.WriteNode(sb, scenario, whileNode, currentIndent)
        );
    }

}

