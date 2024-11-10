using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects;

public record Transition : IIdentifiable
{
    private readonly System _system;

    public string Name { get; set; } = "";
    public Type Type => typeof(Transition);

    public StateProperty SourceState { get; private init; }
    public StateProperty DestinationState { get; private init; }

    public Transition(System system)
    {
        _system = system;

        // Add this to the system
        _system.Transitions.Add(this);

        SourceState = new(system);
        DestinationState = new(system);
    }

    public TransitionReference GenerateReference()
        => new TransitionReference(_system) { Name = Name };

    public bool IsDeepEqv(Transition other)
    {
        return Name.IsEqv(other.Name) &&
               SourceState.IsEqv(other.SourceState) &&
               DestinationState.IsEqv(other.DestinationState);
    }
}