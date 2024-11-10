using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel.Objects.SystemObjects.Interfaces;

public interface IStateful : IIdentifiable
{
    StateProperty State { get; }
    IStatefulObjectReference GenerateStatefulReference();
}
