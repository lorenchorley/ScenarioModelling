using ScenarioModelling.Objects.SystemObjects.Interfaces;

namespace ScenarioModelling.References.Interfaces;

public interface IStatefulObjectReference : IReference<IStateful>
{
    bool IsEqv(IStatefulObjectReference other);
}
