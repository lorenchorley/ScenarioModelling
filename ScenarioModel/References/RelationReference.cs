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

    public CompositeValue? Left { get; set; }
    public CompositeValue? Right { get; set; }

    public RelationReference(System system)
    {
        _system = system;
    }

    public Option<Relation> ResolveReference()
    {
        if (Left == null || Right == null)
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
