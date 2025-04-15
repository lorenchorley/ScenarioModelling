using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;

namespace ScenarioModelling.Execution.Events.Interfaces;

public interface IMetaStoryEvent
{

}

public interface IMetaStoryEvent<T> : IMetaStoryEvent
    where T : IStoryNode
{
    T ProducerNode { get; }
}
