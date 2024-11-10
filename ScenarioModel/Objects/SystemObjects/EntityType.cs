using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects;

/// <summary>
/// Types exist only to allow grouping and reuse of entities (that would then have the same state type and aspects)
/// </summary>
public record EntityType : IIdentifiable
{
    private readonly System _system;

    public string Name { get; set; } = "";
    public Type Type => typeof(EntityType);

    public StateMachineProperty StateMachine { get; private init; }
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

}
