using LanguageExt;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References.Interfaces;

using Relation = ScenarioModel.Objects.SystemObjects.Relation;

namespace ScenarioModel.References;

public record RelationReference : IReference<Relation>, IStatefulObjectReference
{
    private readonly System _system;

    public string Name { get; set; } = "";
    public Type Type => typeof(Relation);

    public CompositeValue? FirstRelatableName { get; set; }
    public CompositeValue? SecondRelatableName { get; set; }

    public RelationReference(System system)
    {
        _system = system;
    }

    public Option<Relation> ResolveReference()
    {
        if (FirstRelatableName == null || SecondRelatableName == null)
        {
            throw new NotImplementedException();
        }

        return _system.Relations
                      .Find(x => x.IsEqv(this));
    }

    Option<IStateful> IReference<IStateful>.ResolveReference()
        => ResolveReference().Map(x => (IStateful)x);

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
