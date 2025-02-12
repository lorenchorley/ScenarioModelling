using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling.CoreObjects.SystemObjects.Properties;

public class StateListProperty(MetaState System) : ReferencableSetProperty<State, StateReference>(System)
{
}
