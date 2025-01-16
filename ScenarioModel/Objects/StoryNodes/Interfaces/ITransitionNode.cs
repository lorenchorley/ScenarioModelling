using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.Objects.StoryNodes.Interfaces;

public interface ITransitionNode : IStoryNode
{
    IEnumerable<string> TargetNodeNames { get; }
}
