using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.Analysis;

public class AnalysisExecutor : IExecutor
{
    public Story EndMetaStory()
    {
        throw new NotImplementedException();
    }

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

    public void StartMetaStory(string name)
    {
        throw new NotImplementedException();
    }

}
