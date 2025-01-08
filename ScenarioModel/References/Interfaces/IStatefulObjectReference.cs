using ScenarioModel.Objects.SystemObjects.Interfaces;

namespace ScenarioModel.References.Interfaces;

public interface IStatefulObjectReference : IReference<IStateful>
{
    bool IsEqv(IStatefulObjectReference other);
}
