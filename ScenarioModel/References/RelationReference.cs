using LanguageExt;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.References;

public record RelationReference : IReference<SystemObjects.Relations.Relation>, IStatefulObjectReference
{
    public string RelationName { get; set; } = "";
    //public IRelatableObjectReference RelatableObject { get; set; } = null!;

    private Option<SystemObjects.Relations.Relation> _relation = Option<SystemObjects.Relations.Relation>.None;

    public Option<SystemObjects.Relations.Relation> ResolveReference(System system)
    {
        if (_relation.IsSome)
        {
            return _relation;
        }

        _relation = system.AllRelations
                          .Find(x => x.Name.IsEqv(RelationName))
                          .Match(Some: x => _relation = x, None: () => Option<SystemObjects.Relations.Relation>.None);

        return _relation;
    }

    Option<IStateful> IReference<IStateful>.ResolveReference(System system)
        => ResolveReference(system).Map(x => (IStateful)x);

    override public string ToString() => $"{RelationName}";
}
