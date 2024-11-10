using ScenarioModel.Objects.SystemObjects.Properties;

namespace ScenarioModel.Objects.SystemObjects.Interfaces;

public interface IRelatable : IIdentifiable
{
    RelationListProperty Relations { get; }
    //IRelatableObjectReference GenerateReference();
}
