using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.States;

public interface IStateful
{
    string Name { get; }
    State? State { get; set; }
    IStatefulObjectReference GenerateReference();
}
