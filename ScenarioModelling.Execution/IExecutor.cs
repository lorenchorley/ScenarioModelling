using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events.Interfaces;

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
