using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;

public interface IRelatable : IMetaStateObject<IRelatableObjectReference>, IReferencable<IRelatableObjectReference>
{
    RelationListProperty Relations { get; }
}
