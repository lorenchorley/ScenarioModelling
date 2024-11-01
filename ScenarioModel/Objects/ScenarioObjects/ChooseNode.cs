using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.Objects.ScenarioObjects.DataClasses;

namespace ScenarioModel.Objects.ScenarioObjects;

public record ChooseNode : ScenarioNode<ChoiceSelectedEvent>
{
    public ChoiceList Choices { get; set; } = new();

    public ChooseNode()
    {
        Name = "Choose";
    }

    public override ChoiceSelectedEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        return new ChoiceSelectedEvent() { ProducerNode = this };
    }

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IScenarioNode>>();
}