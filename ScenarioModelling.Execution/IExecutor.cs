using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution;

public interface IExecutor
{
    IMetaStoryEvent? GetLastEvent();
    bool IsLastEventOfType<T>() where T : IMetaStoryEvent;
    bool IsLastEventOfType<T>(Func<T, bool> pred) where T : IMetaStoryEvent;
    IStoryNode? NextNode();
    void RegisterEvent(IMetaStoryEvent @event);
    void StartMetaStory(string name);
    Story EndMetaStory();
}
