using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.Objects.Visitors;
using ScenarioModel.References;
using System.Text.Json.Serialization;

namespace ScenarioModel.Objects.SystemObjects;

/// <summary>
/// Types exist only to allow grouping and reuse of entities (that would then have the same state type and aspects)
/// </summary>
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
