using ScenarioModelling.Objects.SystemObjects.Properties;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.Objects.SystemObjects.Interfaces;

public interface IRelatable : ISystemObject<IRelatableObjectReference>, IReferencable<IRelatableObjectReference>
{
    RelationListProperty Relations { get; }
}
