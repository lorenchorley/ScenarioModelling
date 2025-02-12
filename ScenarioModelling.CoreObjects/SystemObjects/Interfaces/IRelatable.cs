using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects.Properties;

namespace ScenarioModelling.CoreObjects.SystemObjects.Interfaces;

public interface IRelatable : ISystemObject<IRelatableObjectReference>, IReferencable<IRelatableObjectReference>
{
    RelationListProperty Relations { get; }
}
