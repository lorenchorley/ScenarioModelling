using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.References;

// Transition reference cannot base themselves only on the name
// They are unique only on the triplet (name, source, dest)
// (related : TransitionEquivalanceComparer, )
[ObjectLike<IReference, Transition>]
public record TransitionReference : IReferences<Transition>, IEqualityComparer<TransitionReference>
{
    public string Name { get; set; } = "";
    public string? SourceName { get; set; } = "";
    public string? DestinationName { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Transition);

    public System System { get; }

    public TransitionReference(System system)
    {
        System = system;
    }

    public IEnumerable<Transition> ResolveReferences()
        => System.Transitions.Where(s => s.IsEqv(this)); // TODO Need to search with only the information that is available

    public bool IsResolvable() => ResolveReferences().Any();

    override public string ToString() => Name;

    public bool Equals(TransitionReference? x, TransitionReference? y)
    {
        if (x == null || y == null)
        {
            if (x != null || y != null)
                return false; // If only one is null
            else
                return true; // If both are null
        }

        return x.Name.IsEqv(y.Name) &&
               x.SourceName.IsEqvCountingNulls(y.SourceName) &&
               x.DestinationName.IsEqvCountingNulls(y.DestinationName);
    }

    public int GetHashCode(/*[DisallowNull]*/ TransitionReference obj)
        => obj.Name.GetHashCode();
}
