using ScenarioModel.References;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Relations;

public class Relation : IStateful, INameful
{
    public string Name { get; set; } = "";
    public IReference? LeftEntity { get; set; }
    public IReference? RightEntity { get; set; }
    public State? State { get; set; }
}
