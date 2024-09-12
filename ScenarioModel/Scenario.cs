using ScenarioModel.Collections;
using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.States;

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
    public System System { get; init; } = new();
    public DirectedGraph<IScenarioAction> Steps { get; set; } = new();

    public void Initialise()
    {
        // Complete system with entityies, states etc from the steps before initialising the system
        foreach (var action in Steps)
        {
            CompleteSystemWithObjectsFrom(action);
        }

        System.Initialise();
    }

    private void CompleteSystemWithObjectsFrom(IScenarioAction action)
    {
        switch (action)
        {
            case ChooseAction chooseAction:
                break;
            case StateTransitionAction stateTransitionAction:

                // Check if the state already exists in the system
                if (!System.HasState(stateTransitionAction.StateName))
                {
                    var stateType = new StateType() { Name = stateTransitionAction.StateName + "_Type", States = new() { new State() { Name = stateTransitionAction.StateName } } };
                    System.StateTypes.Add(stateType);
                }

                break;
        }
    }

    public StoryRunResult StartAtStep(string stepName)
    {
        Story story = new() { Scenario = this };

        IScenarioAction? initialAction = Steps.FirstOrDefault(step => step.Name == stepName);

        return StoryRunResult.Successful(story);
    }
}
