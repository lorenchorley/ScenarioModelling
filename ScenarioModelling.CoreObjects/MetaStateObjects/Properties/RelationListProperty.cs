using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Properties;

public class RelationListProperty(MetaState system) : ReferencableSetProperty<Relation, RelationReference>(system)
{
}
