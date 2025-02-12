using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;

namespace ScenarioModelling.Execution.Events.Interfaces;

public interface IStoryEvent
{

}

public interface IMetaStoryEvent<T> : IStoryEvent
    where T : IStoryNode
{
    T ProducerNode { get; }
}
