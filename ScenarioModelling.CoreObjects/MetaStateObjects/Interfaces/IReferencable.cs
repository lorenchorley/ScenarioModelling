using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.Interfaces;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;

public interface IReferencable<TRef>
    where TRef : IReference
{
    TRef GenerateReference();
}
