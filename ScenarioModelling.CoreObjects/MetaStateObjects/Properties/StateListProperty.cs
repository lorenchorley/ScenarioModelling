using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Properties;

public class StateListProperty(MetaState System) : ReferencableSetProperty<State, StateReference>(System)
{
}
