using Newtonsoft.Json;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.References;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.Execution.Events;

[StoryNodeLike<IStoryNode, IfNode>]
public record StateChangeEvent : IMetaStoryEvent<TransitionNode>
{
    public IReference StatefulObject { get; set; } = null!;
    public StateReference InitialState { get; set; } = null!;
    public StateReference FinalState { get; set; } = null!;
    public string TransitionName { get; set; } = null!;

    [JsonIgnore]
    public TransitionNode ProducerNode { get; set; } = null!;

    public override string ToString()
    {
        string objText = JsonConvert.SerializeObject(this);
        return $"{GetType().Name} {objText}";
    }
}
