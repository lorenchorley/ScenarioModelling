using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.Objects.ScenarioObjects.DataClasses;

namespace ScenarioModel.Objects.ScenarioObjects;

public record DialogNode : ScenarioNode<DialogEvent>
{
    public string TextTemplate { get; set; } = "";
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
}
