using ScenarioModelling.CoreObjects.Expressions.SemanticTree;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;

public interface IStoryNodeWithExpression : IStoryNode
{
    string? LineInformation { get; }
    Expression AssertionExpression { get; }
}
