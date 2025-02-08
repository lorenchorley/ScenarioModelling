using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.SystemObjects.Properties;
using ScenarioModelling.Objects.Visitors;
using ScenarioModelling.References;
using YamlDotNet.Serialization;

namespace ScenarioModelling.Objects.SystemObjects;

//[ObjectLike<ISystemObject, AspectType>]
public record AspectType : ISystemObject<AspectTypeReference>, IOptionalSerialisability
{
    private System _system = null!;

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

    public AspectType(System system)
    {
        _system = system;

        // Add this to the system
        //system.AspectTypes.Add(this);

        StateMachine = new(system);
    }

    public void InitialiseAfterDeserialisation(System system)
    {
        _system = system;
    }

    public AspectTypeReference GenerateReference()
        => new AspectTypeReference(_system) { Name = Name };

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitAspectType(this);

}
