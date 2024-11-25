using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.Objects.Visitors;
using ScenarioModel.References;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ScenarioModel.Objects.SystemObjects;

public record Transition : ISystemObject<TransitionReference>, IEqualityComparer<Transition>
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
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
        => new TransitionReference(_system) 
        { 
            Name = Name ,
            SourceName = SourceState.Name,
            DestinationName = DestinationState.Name
        };

    public bool IsDeepEqv(Transition other)
    {
        return Name.IsEqv(other.Name) &&
               SourceState.IsEqv(other.SourceState) &&
               DestinationState.IsEqv(other.DestinationState);
    }

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitTransition(this);

    public bool Equals(Transition? x, Transition? y)
    {
        if (x == null || y == null)
        {
            if (x != null || y != null)
                return false; // If only one is null
            else
                return true; // If both are null
        }

        return x.Name.IsEqv(y.Name) &&
               x.SourceState.Name.IsEqvCountingNulls(y.SourceState.Name) &&
               x.DestinationState.Name.IsEqvCountingNulls(y.DestinationState.Name);
    }

    public int GetHashCode([DisallowNull] Transition obj)
        => obj.Name.GetHashCode();
}