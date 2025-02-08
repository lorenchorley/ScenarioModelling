using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.SystemObjects.Properties;
using ScenarioModelling.Objects.Visitors;
using ScenarioModelling.References;

namespace ScenarioModelling.Objects.SystemObjects;

/// <summary>
/// Types exist only to allow grouping and reuse of entities (that would then have the same state type and aspects)
/// </summary>
[ProtoContract]
[SystemObjectLike<ISystemObject, EntityType>]
public record EntityType : ISystemObject<EntityTypeReference>, IOptionalSerialisability
{
    private System _system = null!;

    [JsonIgnore]
    public Type Type => typeof(EntityType);

    [ProtoMember(1)]
    public string Name { get; set; } = "";

    [ProtoMember(2)]
    public StateMachineProperty StateMachine { get; private set; }

    public bool ExistanceOriginallyInferred { get; set; } = false;
    public bool ShouldReserialise
    {
        get
        {
            if (ExistanceOriginallyInferred)
                return false;

            //if (StateMachine.IsSet)
            //    return false;

            return true;
        }
    }

    // AspectType ?

    private EntityType()
    {
        
    }

    public EntityType(System system)
    {
        _system = system;

        // Add this to the system
        system.EntityTypes.Add(this);

        StateMachine = new(system);
    }

    public void InitialiseAfterDeserialisation(System system)
    {
        _system = system;
    }

    public EntityTypeReference GenerateReference()
        => new EntityTypeReference(_system) { Name = Name };

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitEntityType(this);
}
