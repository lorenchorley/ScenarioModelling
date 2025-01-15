using ScenarioModelling.Expressions.SemanticTree;

namespace ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

public interface IScenarioNodeWithExpression : IScenarioNode
{
    string? LineInformation { get; }
    Expression Condition { get; }
}
