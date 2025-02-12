using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;

namespace ScenarioModelling.CoreObjects.References.Interfaces;

public interface IStatefulObjectReference : IReference<IStateful>
{
    bool IsEqv(IStatefulObjectReference other);
}
