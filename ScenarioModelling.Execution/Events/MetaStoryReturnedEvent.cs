using Newtonsoft.Json;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.Events;

//[StoryNodeLike<IMetaStoryEvent, CallMetaStoryNode>] // How to include this in the exhaustivity check? At the moment it's not possible because there's a uniqueness check for each node type
public record MetaStoryReturnedEvent : IMetaStoryEvent
{
    public override string ToString()
    {
        string objText = JsonConvert.SerializeObject(this);
        return $"{GetType().Name} {objText}";
    }
}
