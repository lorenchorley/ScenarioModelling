using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.Expressions.SemanticTree;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;
using Relation = ScenarioModelling.CoreObjects.SystemObjects.Relation;

namespace ScenarioModelling.CoreObjects.References;

[SystemObjectLike<IReference, Relation>]
public record RelationReference : IReference<Relation>, IStatefulObjectReference
{

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Relation);

    public CompositeValue? Left { get; set; }
    public CompositeValue? Right { get; set; }

    [JsonIgnore]
    public MetaState System { get; }

    public RelationReference(MetaState system)
    {
        System = system;
    }

    public Option<Relation> ResolveReference()
    {
        if (Left == null || Right == null)
        {
            throw new NotImplementedException();
        }

        Relation? relation = System.Relations
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
