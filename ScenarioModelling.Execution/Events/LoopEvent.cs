﻿using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.Events;

[StoryNodeLike<IMetaStoryEvent, LoopNode>]
public record LoopEvent : IMetaStoryEvent<LoopNode>
{
    public bool LoopRun { get; set; }

    [JsonIgnore]
    public LoopNode ProducerNode { get; set; } = null!;

    public override string ToString()
    {
        string objText = JsonConvert.SerializeObject(this);
        return $"{GetType().Name} {objText}";
    }
}
