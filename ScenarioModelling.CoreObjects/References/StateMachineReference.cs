using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.References.Interfaces;

namespace ScenarioModelling.CoreObjects.References;

[MetaStateObjectLike<IReference, StateMachine>]
public record StateMachineReference : ReferenceBase<StateMachine>
{
    [JsonIgnore]
    public MetaState MetaState { get; }

    public StateMachineReference(MetaState system)
    {
        MetaState = system;
    }

    public override Option<StateMachine> ResolveReference()
        => MetaState.StateMachines.Find(s => s.IsEqv(this));

    override public string ToString() => $"{Name}";

}
