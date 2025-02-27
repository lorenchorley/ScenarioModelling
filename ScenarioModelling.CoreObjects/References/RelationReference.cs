using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.Expressions.SemanticTree;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using Relation = ScenarioModelling.CoreObjects.MetaStateObjects.Relation;

namespace ScenarioModelling.CoreObjects.References;

[MetaStateObjectLike<IReference, Relation>]
public record RelationReference : ReferenceBase<Relation>, IStatefulObjectReference
{
    public CompositeValue? Left { get; set; }
    public CompositeValue? Right { get; set; }

    [JsonIgnore]
    public MetaState MetaState { get; }

    public RelationReference(MetaState system)
    {
        MetaState = system;
    }

    public override Option<Relation> ResolveReference()
    {
        if (Left == null || Right == null)
        {
            throw new NotImplementedException();
        }

        Relation? relation = MetaState.Relations
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
