using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.Objects.SystemObjects.Interfaces;

public interface IReferencable<TRef>
    where TRef : IReference
{
    TRef GenerateReference();
}
