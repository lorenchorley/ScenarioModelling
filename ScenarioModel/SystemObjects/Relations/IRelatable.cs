using ScenarioModel.References;
using ScenarioModel.SystemObjects.States;
using ScenarioModel.Validation;

namespace ScenarioModel.SystemObjects.Relations;

public interface IRelatable
{
    List<Relation> Relations { get; }
}
