using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;
using ScenarioModelling.CoreObjects.Visitors;
using YamlDotNet.Serialization;

namespace ScenarioModelling.CoreObjects.MetaStateObjects;

//[ObjectLike<ISystemObject, AspectType>]
public record AspectType : ISystemObject<AspectTypeReference>, IOptionalSerialisability
{
    private MetaState _system = null!;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type => typeof(AspectType);

    [ProtoMember(1)]
    public string Name { get; set; } = "";

    [ProtoMember(2)]
    public StateMachineProperty StateMachine { get; private set; }

    public bool ExistanceOriginallyInferred { get; set; } = false;
    public bool ShouldReserialise
    {
        get
        {
            return !ExistanceOriginallyInferred;
        }
    }

    private AspectType()
    {

    }

    public AspectType(MetaState system)
    {
        _system = system;

        // Add this to the system
        //system.AspectTypes.Add(this);

        StateMachine = new(system);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _system = system;
    }

    public AspectTypeReference GenerateReference()
        => new AspectTypeReference(_system) { Name = Name };

    public object Accept(IMetaStateVisitor visitor)
        => visitor.VisitAspectType(this);

}
