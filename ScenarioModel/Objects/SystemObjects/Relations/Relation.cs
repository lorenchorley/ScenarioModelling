using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Relations;

public record Relation(System System) : IStateful, INameful
{
    public string Name { get; set; } = "";
    public IReference? LeftEntity { get; set; }
    public IReference? RightEntity { get; set; }
    public NullableStateProperty State { get; } = new(System);

    public IStatefulObjectReference GenerateReference()
    {
        return new RelationReference() { RelationName = Name };
    }
}
