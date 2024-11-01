using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Relations;

public record Relation : IStateful, INameful
{
    public string Name { get; set; } = "";
    public IReference? LeftEntity { get; set; }
    public IReference? RightEntity { get; set; }
    public State? State { get; set; }

    public IStatefulObjectReference GenerateReference()
    {
        return new RelationReference() { RelationName = Name };
    }
}
