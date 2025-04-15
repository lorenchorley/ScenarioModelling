using Newtonsoft.Json;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
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
public class MetaStory : IIdentifiable
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(MetaStory);

    public MetaState MetaState { get; private set; }

    public DirectedGraph<IStoryNode> Graph { get; private set; }

    public MetaStory(MetaState metaState, DirectedGraph<IStoryNode> graph)
    {
        MetaState = metaState;
        Graph = graph;
    }
}
