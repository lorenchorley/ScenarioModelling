﻿using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CoreObjects;

// Assertions in C# code that works directly on the object model defined by the system and the MetaStory?
// Time series graphs and sequence diagrams derived from the MetaStory ?
// Component diagrams derived from the system?
// Code skeltons generated from the system?
// State exhaustiveness and reachability analysis?
// What makes relations special?

/// <summary>
/// A MetaStory is all the possibilities of a story, yet to be played out.
/// </summary>
[ProtoContract]
public class MetaStory : IIdentifiable
{
    [ProtoMember(1)]
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(MetaStory);

    public MetaState MetaState { get; private set; }

    [ProtoMember(2)]
    public DirectedGraph<IStoryNode> Graph { get; private set; }

    public MetaStory(MetaState metaState, DirectedGraph<IStoryNode> graph)
    {
        MetaState = metaState;
        Graph = graph;
    }
}
