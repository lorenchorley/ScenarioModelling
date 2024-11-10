using LanguageExt;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel.References;

public record StateReference(System System) : IReference<State>
{
    public string Name { get; set; } = "";
    public Type Type => typeof(State);

    public Option<State> ResolveReference()
        => System.States.Find(s => s.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => Name;

}
