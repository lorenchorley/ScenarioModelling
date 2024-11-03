using OneOf;
using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.Objects.SystemObjects.Relations;
using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Entities;

public record Entity(System System) : IStateful, IRelatable, INameful
{
    public string Name { get; set; } = "";
    public EntityType EntityType { get; set; } = null!;
    public List<Relation> Relations { get; set; } = new();
    public List<Aspect> Aspects { get; set; } = new();
    public string CharacterStyle { get; set; } = "";

    public NullableStateProperty State { get; } = new(System);

    public IStatefulObjectReference GenerateReference()
    {
        return new EntityReference() { EntityName = Name };
    }
}
