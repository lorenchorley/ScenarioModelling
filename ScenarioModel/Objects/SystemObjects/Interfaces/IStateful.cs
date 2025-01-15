using ScenarioModelling.Objects.SystemObjects.Properties;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.Objects.SystemObjects.Interfaces;

public interface IStateful : IIdentifiable
{
    StateProperty InitialState { get; }
    StateProperty State { get; }
    IStatefulObjectReference GenerateStatefulReference();
}
