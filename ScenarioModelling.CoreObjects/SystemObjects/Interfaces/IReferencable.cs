using ScenarioModelling.CoreObjects.References.Interfaces;

namespace ScenarioModelling.CoreObjects.SystemObjects.Interfaces;

public interface IReferencable<TRef>
    where TRef : IReference
{
    TRef GenerateReference();
}
