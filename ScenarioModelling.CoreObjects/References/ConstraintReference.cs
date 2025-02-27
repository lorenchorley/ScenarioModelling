using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.References.Interfaces;

namespace ScenarioModelling.CoreObjects.References;

[MetaStateObjectLike<IReference, Constraint>]
public record ConstraintReference : ReferenceBase<Constraint>
{
    [JsonIgnore]
    public MetaState MetaState { get; }

    public ConstraintReference(MetaState system)
    {
        MetaState = system;
    }

    public override Option<Constraint> ResolveReference()
        => MetaState.Constraints.Find(x => x.IsEqv(this));

    override public string ToString() => $"{Name}";

}
