using ScenarioModel.Collections;
using ScenarioModel.Execution;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.Objects.SystemObjects.States;

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
public class Scenario
{
    public string Name { get; set; } = "";
    public System System { get; set; } = new();
    public DirectedGraph<IScenarioNode> Graph { get; set; } = new();

    public void Initialise(Context context)
    {
        // Complete system with entities, states etc from the nodes before initialising the system
        foreach (var action in Graph.PrimarySubGraph.NodeSequence)
        {
            CompleteSystemWithObjectsFrom(action);
        }
    }

    private void CompleteSystemWithObjectsFrom(IScenarioNode action)
    {
        switch (action)
        {
            case ChooseNode chooseAction:
                break;
            case StateTransitionNode stateTransitionAction:

                // Check if the state already exists in the system
                if (!System.HasState(stateTransitionAction.TransitionName))
                {
                    var stateMachine = new StateMachine() { Name = stateTransitionAction.TransitionName + "_Type", States = new() { new State() { Name = stateTransitionAction.TransitionName } } };
                    System.StateMachines.Add(stateMachine);
                }

                break;
        }
    }

    public StoryRunResult StartAtNode(string nodeName)
    {
        ScenarioRun story = new() { Scenario = this };

        IScenarioNode? initialAction = Graph.PrimarySubGraph.NodeSequence.FirstOrDefault(step => step.Name == nodeName);

        return StoryRunResult.Successful(story);
    }
}
