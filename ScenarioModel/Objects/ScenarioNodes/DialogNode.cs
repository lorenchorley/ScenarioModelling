using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;

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
        Name = "Dialog";
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

    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);
}
