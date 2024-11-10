using LanguageExt;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel.References;

public record TransitionReference(System System) : IReference<Transition>
{
    public string Name { get; set; } = "";
    public Type Type => typeof(Transition);

    public Option<Transition> ResolveReference()
        => System.Transitions.Find(s => s.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => Name;
}
