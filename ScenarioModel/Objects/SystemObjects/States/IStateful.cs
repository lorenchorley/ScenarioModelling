using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.States;

public interface IStateful
{
    string Name { get; }
    NullableStateProperty State { get; }
    IStatefulObjectReference GenerateReference();
}
