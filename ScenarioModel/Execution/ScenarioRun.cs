using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.Scenario;

namespace ScenarioModel.Execution;

// Assertions in C# code that works directly on the object model defined by the system and the scenario?
// Time series graphs and sequence diagrams derived from the scenario ?
// Component diagrams derived from the system?
// Code skeltons generated from the system?
// State exhaustiveness and reachability analysis?

// What makes relations special?


/// <summary>
/// A story is an instance or a play through of a scenario.
/// </summary>
public class ScenarioRun
{
    public Scenario Scenario { get; init; } = null!;
    public List<IScenarioEvent> Events { get; set; } = new();
    public IScenarioNode? CurrentNode { get; set; }

    public void Init()
    {
    }

    public IScenarioNode? NextNode()
    {
        if (CurrentNode == null)
        {
            return CurrentNode = Scenario.Steps.First();
        }

        if (CurrentNode is ChooseNode chooseNode)
        {
            // The last event must be a choice event
            var lastEvent = Events.LastOrDefault();

            if (lastEvent is null)
                throw new Exception("No choice event found");

            if (lastEvent is not ChoiceSelectedEvent choiceEvent)
                throw new Exception("Last event is not a choice event");

            // Find the next node based on the choice
            CurrentNode = Scenario.Steps
                                  .Where(s => s.Name.IsEqv(choiceEvent.Choice))
                                  .FirstOrDefault();

            if (CurrentNode is null)
                throw new Exception($"Choice not found in graph : {choiceEvent.Choice}");

            return CurrentNode;
        }

        if (CurrentNode is JumpNode jumpNode)
        {
            // The last event must be a choice event
            var lastEvent = Events.LastOrDefault();

            if (lastEvent is null)
                throw new Exception("No jump event found");

            if (lastEvent is not JumpEvent jumpEvent)
                throw new Exception("Last event is not a jump event");

            // Find the next node based on the choice
            CurrentNode = Scenario.Steps
                                  .Where(s => s.Name.IsEqv(jumpEvent.Target))
                                  .FirstOrDefault();

            if (CurrentNode is null)
                throw new Exception($"Node corresponding to jump target not found in graph : {jumpEvent.Target}");

            return CurrentNode;
        }

        return CurrentNode = Scenario.Steps.GetNextInSequence(CurrentNode);
    }

    public void RegisterEvent(IScenarioEvent @event)
    {
        Events.Add(@event);
    }
}

public interface StoryRunResult
{
    public static StoryRunResult ConstraintFailure(string value) => new ConstraintFailure(value);
    public static StoryRunResult Successful(ScenarioRun story) => new Successful(story);
    public static StoryRunResult NotStarted() => new NotStarted();
}

public class ConstraintFailure(string value) : StoryRunResult
{
    public string Value { get; } = value;
}

public class Successful(ScenarioRun story) : StoryRunResult
{
    public ScenarioRun Story { get; } = story;
}

public class NotStarted : StoryRunResult
{
}