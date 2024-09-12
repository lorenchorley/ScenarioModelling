using ScenarioModel.References;
using ScenarioModel.SystemObjects.States;
using ScenarioModel.Validation;

namespace ScenarioModel.SystemObjects.Relations;

public class Relation : IStateful
{
    public string Name { get; set; } = "";
    //public RelationType RelationType { get; set; } = null!;
    public IReference? LeftEntity { get; set; }
    public IReference? RightEntity { get; set; }
    public State? State { get; set; }
}
