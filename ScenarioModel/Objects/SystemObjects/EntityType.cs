using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.SystemObjects.Properties;
using ScenarioModelling.Objects.Visitors;
using ScenarioModelling.References;
using System.Text.Json.Serialization;

namespace ScenarioModelling.Objects.SystemObjects;

/// <summary>
/// Types exist only to allow grouping and reuse of entities (that would then have the same state type and aspects)
/// </summary>
[ObjectLike<ISystemObject, EntityType>]
public record EntityType : ISystemObject<EntityTypeReference>, IOptionalSerialisability
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(EntityType);

    public StateMachineProperty StateMachine { get; private init; }

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

    public EntityType(System system)
    {
        _system = system;

        // Add this to the system
        system.EntityTypes.Add(this);

        StateMachine = new(system);
    }

    public EntityTypeReference GenerateReference()
        => new EntityTypeReference(_system) { Name = Name };

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitEntityType(this);
}
