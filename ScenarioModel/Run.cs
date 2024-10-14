using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;

namespace ScenarioModel;

// Assertions in C# code that works directly on the object model defined by the system and the scenario?
// Time series graphs and sequence diagrams derived from the scenario ?
// Component diagrams derived from the system?
// Code skeltons generated from the system?
// State exhaustiveness and reachability analysis?

// What makes relations special?


/// <summary>
/// A story is an instance or a play through of a scenario.
/// </summary>
public class Run
{
    public Scenario Scenario { get; init; } = null!;
    public List<IScenarioEvent> Events { get; set; } = new();
    public StoryRunResult Result { get; set; } = StoryRunResult.NotStarted();
}

public interface StoryRunResult
{
    public static StoryRunResult ConstraintFailure(string value) => new ConstraintFailure(value);
    public static StoryRunResult Successful(Run story) => new Successful(story);
    public static StoryRunResult NotStarted() => new NotStarted();
}

public class ConstraintFailure(string value) : StoryRunResult
{
    public string Value { get; } = value;
}

public class Successful(Run story) : StoryRunResult
{
    public Run Story { get; } = story;
}

public class NotStarted : StoryRunResult
{
}