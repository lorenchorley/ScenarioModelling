using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;
using ScenarioModelling.CoreObjects.Visitors;
using YamlDotNet.Serialization;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;

namespace ScenarioModelling.CoreObjects.MetaStateObjects;

[MetaStateObjectLike<IMetaStateObject, Transition>]
public record Transition : IMetaStateObject<TransitionReference>, IEqualityComparer<Transition>
{
    private MetaState _system = null!;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type => typeof(Transition);

    public string Name { get; set; } = "";

    public StateProperty SourceState { get; private set; }

    public StateProperty DestinationState { get; private set; }

    private Transition()
    {

    }

    public Transition(MetaState system)
    {
        _system = system;

        // Add this to the system
        //_system.Transitions.Add(this);

        SourceState = new(system);
        DestinationState = new(system);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _system = system;
    }

    public OneOfMetaStateObject ToOneOf()
        => new OneOfMetaStateObject(this);

    public TransitionReference GenerateReference()
        => new TransitionReference(_system)
        {
            Name = Name,
            SourceName = SourceState.Name,
            DestinationName = DestinationState.Name
        };

    public bool IsDeepEqv(Transition other)
    {
        return Name.IsEqv(other.Name) &&
               SourceState.IsEqv(other.SourceState) &&
               DestinationState.IsEqv(other.DestinationState);
    }

    public object Accept(IMetaStateVisitor visitor)
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

    public int GetHashCode(/*[DisallowNull]*/ Transition obj)
        => obj.Name.GetHashCode();
}