using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.EventSourcing;

public class StoryEventSource
{
    public List<IMetaStoryEvent> Events { get; set; } = new();

    public void Add(IMetaStoryEvent @event)
    {
        Events.Add(@event);
    }

    internal IEnumerable<IMetaStoryEvent> GetEnumerable()
    {
        return Events;
    }
}
