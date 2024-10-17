using ScenarioModel.Execution.Events;
using ScenarioModel.Expressions.SemanticTree;

namespace ScenarioModel.ScenarioObjects;

public record IfNode : IScenarioNode<IfBlockEvent>
{
    public string Name { get; set; } = "If";
    public Expression Expression { get; set; } = null!;
    public List<IScenarioNode> NodesInIfBlock { get; set; } = new();

    public IfBlockEvent GenerateEvent()
    {
        return new IfBlockEvent() { ProducerNode = this };
    }
}