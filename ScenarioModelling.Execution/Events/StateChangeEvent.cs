using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events.Interfaces;

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
