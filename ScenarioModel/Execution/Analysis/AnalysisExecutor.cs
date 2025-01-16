using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.Execution.Analysis;

public class AnalysisExecutor : IExecutor
{
    public IStoryEvent? GetLastEvent()
    {
        throw new NotImplementedException();
    }

    public bool IsLastEventOfType<T>() where T : IStoryEvent
    {
        throw new NotImplementedException();
    }

    public bool IsLastEventOfType<T>(Func<T, bool> pred) where T : IStoryEvent
    {
        throw new NotImplementedException();
    }

    public IStoryNode? NextNode()
    {
        throw new NotImplementedException();
    }

    public void RegisterEvent(IStoryEvent @event)
    {
        throw new NotImplementedException();
    }

    public Story StartMetaStory(string name)
    {
        throw new NotImplementedException();
    }
}
