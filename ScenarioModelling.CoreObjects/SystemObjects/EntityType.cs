using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects.Properties;
using ScenarioModelling.CoreObjects.Visitors;

namespace ScenarioModelling.CoreObjects.SystemObjects;

/// <summary>
/// Types exist only to allow grouping and reuse of entities (that would then have the same state type and aspects)
/// </summary>
[ProtoContract]
[SystemObjectLike<ISystemObject, EntityType>]
public record EntityType : ISystemObject<EntityTypeReference>, IOptionalSerialisability
{
    private MetaState _system = null!;

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

    public EntityType(MetaState system)
    {
        _system = system;

        // Add this to the system
        system.EntityTypes.Add(this);

        StateMachine = new(system);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _system = system;
    }

    public EntityTypeReference GenerateReference()
        => new EntityTypeReference(_system) { Name = Name };

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitEntityType(this);
}
