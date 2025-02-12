using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling.CoreObjects.SystemObjects.Properties;

public class RelationListProperty(MetaState system) : ReferencableSetProperty<Relation, RelationReference>(system)
{
}
