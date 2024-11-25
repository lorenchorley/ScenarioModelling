using ScenarioModel.Expressions.SemanticTree;

namespace ScenarioModel.Objects.ScenarioNodes.BaseClasses;

public interface IScenarioNodeWithExpression : IScenarioNode
{
    string? LineInformation { get; }
    Expression Condition { get; }
}
