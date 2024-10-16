using LanguageExt;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.References;

public record RelationReference : IReference<SystemObjects.Relations.Relation>, IStatefulObjectReference
{
    public string? RelationName { get; set; } = null;
    public string? FirstRelatableName { get; set; } = "";
    public string? SecondRelatableName { get; set; } = "";

    public Option<SystemObjects.Relations.Relation> ResolveReference(System system)
    {
        return system.AllRelations
                     .Find(x => x.Name.IsEqv(RelationName));
    }

    Option<IStateful> IReference<IStateful>.ResolveReference(System system)
        => ResolveReference(system).Map(x => (IStateful)x);

    override public string ToString() => $"{RelationName}";
}
