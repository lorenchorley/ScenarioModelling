using ScenarioModel.Collections;
using ScenarioModel.Execution;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using System.Text.Json.Serialization;

namespace ScenarioModel;

// Assertions in C# code that works directly on the object model defined by the system and the scenario?
// Time series graphs and sequence diagrams derived from the scenario ?
// Component diagrams derived from the system?
// Code skeltons generated from the system?
// State exhaustiveness and reachability analysis?

// What makes relations special?

/// <summary>
/// A scenario is all the possibilities of a story, yet to be played out.
/// </summary>
public class Scenario : IIdentifiable
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Scenario);

    public System System { get; set; } = new();
    public DirectedGraph<IScenarioNode> Graph { get; set; } = new();

    public StoryRunResult StartAtNode(string nodeName)
    {
        ScenarioRun story = new() { Scenario = this };

        IScenarioNode? initialAction = Graph.PrimarySubGraph.NodeSequence.FirstOrDefault(step => step.Name == nodeName);

        return StoryRunResult.Successful(story);
    }
}
