using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects.Properties;

namespace ScenarioModelling.CoreObjects.SystemObjects.Interfaces;

public interface IStateful : IIdentifiable
{
    StateProperty InitialState { get; }
    StateProperty State { get; }
    IStatefulObjectReference GenerateStatefulReference();
}
