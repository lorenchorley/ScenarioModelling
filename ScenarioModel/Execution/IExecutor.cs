using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.Execution;

public interface IExecutor
{
    IStoryEvent? GetLastEvent();
    bool IsLastEventOfType<T>() where T : IStoryEvent;
    bool IsLastEventOfType<T>(Func<T, bool> pred) where T : IStoryEvent;
    IStoryNode? NextNode();
    void RegisterEvent(IStoryEvent @event);
    void StartMetaStory(string name);
    Story EndMetaStory();
}
