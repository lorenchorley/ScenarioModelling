using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel.Objects.SystemObjects.Interfaces;

public interface IRelatable : ISystemObject<IRelatableObjectReference>, IReferencable<IRelatableObjectReference>
{
    RelationListProperty Relations { get; }
}
