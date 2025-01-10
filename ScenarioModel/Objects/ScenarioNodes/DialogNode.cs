using ScenarioModel.Collections.Graph;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Objects.Visitors;
using System.Diagnostics;

namespace ScenarioModel.Objects.ScenarioNodes;

[NodeLike<IScenarioNode, DialogNode>]
public record DialogNode : ScenarioNode<DialogEvent>
{
    [NodeLikeProperty(serialisedName: "Text")]
    public string TextTemplate { get; set; } = "";

    [NodeLikeProperty(doNotSerialiseIfNullOrEmpty: true)]
    public string? Character { get; set; } = null;

    public DialogNode()
    {
    }

    public override DialogEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        DialogEvent e = new DialogEvent()
        {
            Character = Character,
            ProducerNode = this,
        };

        string text = TextTemplate;
        text = dependencies.Interpolator.ReplaceInterpolations(text);
        e.Text = text;

        return e;
    }

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IScenarioNode>>();

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IScenarioVisitor visitor)
        => visitor.VisitDialogNode(this);

    public override bool IsFullyEqv(IScenarioNode other)
    {
        if (other is not DialogNode otherNode)
            return false;

        if (!otherNode.Name.IsEqv(Name))
            return false;
        
        if (!otherNode.TextTemplate.IsEqv(TextTemplate))
            return false;

        if (!otherNode.Character.IsEqvCountingNulls(Character))
            return false;

        return true;
    }
}
