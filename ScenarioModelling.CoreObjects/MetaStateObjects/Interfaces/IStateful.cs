using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;

public interface IStateful : IIdentifiable
{
    StateProperty InitialState { get; }
    StateProperty State { get; }
    IStatefulObjectReference GenerateStatefulReference();
}
