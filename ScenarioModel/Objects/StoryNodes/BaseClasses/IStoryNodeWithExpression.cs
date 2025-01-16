using ScenarioModelling.Expressions.SemanticTree;

namespace ScenarioModelling.Objects.StoryNodes.BaseClasses;

public interface IStoryNodeWithExpression : IStoryNode
{
    string? LineInformation { get; }
    Expression Condition { get; }
}
