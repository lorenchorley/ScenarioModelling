using ScenarioModelling.CoreObjects.Expressions.SemanticTree;

namespace ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;

public interface IStoryNodeWithExpression : IStoryNode
{
    string? LineInformation { get; }
    Expression Condition { get; }
}
