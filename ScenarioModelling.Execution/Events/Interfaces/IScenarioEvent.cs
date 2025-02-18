using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;

namespace ScenarioModelling.Execution.Events.Interfaces;

public interface IMetaStoryEvent
{

}

public interface IMetaStoryEvent<T> : IMetaStoryEvent
    where T : IStoryNode
{
    T ProducerNode { get; }
}
