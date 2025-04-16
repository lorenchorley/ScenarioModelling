using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;
using ScenarioModelling.CoreObjects.Visitors;
using YamlDotNet.Serialization;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStateObjects;

[DebuggerDisplay(@"Aspect : {Name}")]
[MetaStateObjectLike<IMetaStateObject, Aspect>]
public record Aspect : IMetaStateObject<AspectReference>, IStateful, IRelatable
{
    private MetaState _metaState = null!;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type => typeof(Aspect);

    public string Name { get; set; } = "";

    public AspectType? AspectType { get; set; }

    public EntityProperty Entity { get; private set; }

    public RelationListProperty Relations { get; private set; }

    public StateProperty InitialState { get; private set; }

    public StateProperty State { get; private set; }

    private Aspect()
    {

    }

    public Aspect(MetaState metaState)
    {
        _metaState = metaState;

        // Add this to the system
        //metaState.Aspects.Add(this);

        Entity = new EntityProperty(metaState);
        InitialState = new StateProperty(metaState);
        State = new StateProperty(metaState);
        Relations = new RelationListProperty(metaState);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _metaState = system;
    }

    public OneOfMetaStateObject ToOneOf()
        => new OneOfMetaStateObject(this);

    public AspectReference GenerateReference()
        => new AspectReference(_metaState) { Name = Name };

    public IStatefulObjectReference GenerateStatefulReference()
        => new AspectReference(_metaState) { Name = Name };

    IRelatableObjectReference IMetaStateObject<IRelatableObjectReference>.GenerateReference()
        => GenerateReference();

    IRelatableObjectReference IReferencable<IRelatableObjectReference>.GenerateReference()
        => GenerateReference();

    public object Accept(IMetaStateVisitor visitor)
        => visitor.VisitAspect(this);

}
