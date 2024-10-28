using ScenarioModel.Execution.Events;
using ScenarioModel.Expressions.SemanticTree;

namespace ScenarioModel.Objects.Scenario;

public record IfNode : IScenarioNode<IfBlockEvent>
{
    public string Name { get; set; } = "If";
    public Expression Expression { get; set; } = null!;
    public List<IScenarioNode> Steps { get; set; } = new();

    public IfBlockEvent GenerateEvent()
    {
        return new IfBlockEvent() { ProducerNode = this };
    }
}