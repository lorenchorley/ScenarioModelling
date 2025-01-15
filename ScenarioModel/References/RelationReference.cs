using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.SemanticTree;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.References.Interfaces;
using Relation = ScenarioModelling.Objects.SystemObjects.Relation;

namespace ScenarioModelling.References;

[ObjectLike<IReference, Relation>]
public record RelationReference : IReference<Relation>, IStatefulObjectReference
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
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

        Relation? relation = _system.Relations
                              .Find(x => IsEqvToRelation(x));
        return relation;
    }

    private bool IsEqvToRelation(Relation relation)
    {
        if (!relation.IsEqv(this)) // Type and name
            return false;

        ArgumentNullExceptionStandard.ThrowIfNull(relation.LeftEntity);
        ArgumentNullExceptionStandard.ThrowIfNull(Left);

        if (Left.ValueList.Count != 1)
            throw new Exception("Left value list count is not 1");

        if (!Left.ValueList[0].IsEqv(relation.LeftEntity.Name))
            return false;

        ArgumentNullExceptionStandard.ThrowIfNull(relation.RightEntity);
        ArgumentNullExceptionStandard.ThrowIfNull(Right);

        if (Right.ValueList.Count != 1)
            throw new Exception("Right value list count is not 1");

        if (!Right.ValueList[0].IsEqv(relation.RightEntity.Name))
            return false;

        return true;
    }

    Option<IStateful> IReference<IStateful>.ResolveReference()
        => ResolveReference().Map(x => (IStateful)x);

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";


    public bool IsEqv(IStatefulObjectReference other)
    {
        if (other is not RelationReference otherReference)
            return false;

        if (!otherReference.Name.IsEqv(Name))
            return false;

        return true;
    }
}
