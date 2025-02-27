using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.References.Interfaces;

namespace ScenarioModelling.CoreObjects.References;

[MetaStateObjectLike<IReference, State>]
public record StateReference : ReferenceBase<State>
{
    [JsonIgnore]
    public MetaState MetaState { get; }

    public StateReference(MetaState system)
    {
        MetaState = system;
    }

    public override LanguageExt.Option<State> ResolveReference()
        => MetaState.States.Find(s => s.IsEqv(this));

    override public string ToString() => Name;

}
