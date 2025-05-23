﻿using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.References.Interfaces;

namespace ScenarioModelling.CoreObjects.References;

// Transition reference cannot base themselves only on the name
// They are unique only on the triplet (name, source, dest)
// (related : TransitionEquivalanceComparer, )
[MetaStateObjectLike<IReference, Transition>]
public record TransitionReference : ReferencesBase<Transition>, IEqualityComparer<TransitionReference>
{
    public string? SourceName { get; set; } = "";
    public string? DestinationName { get; set; } = "";

    [JsonIgnore]
    public MetaState MetaState { get; }

    public TransitionReference(MetaState system)
    {
        MetaState = system;
    }

    public override IEnumerable<Transition> ResolveReferences()
        => MetaState.Transitions.Where(s => 
                s.IsEqv(this) &&
                s.SourceState.Name.IsEqvCountingNulls(SourceName) &&
                s.DestinationState.Name.IsEqvCountingNulls(DestinationName)
            );

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
