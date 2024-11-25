using ScenarioModel.References.Interfaces;

namespace ScenarioModel.Objects.SystemObjects.Interfaces;

public interface IReferencable<TRef>
    where TRef : IReference
{
    TRef GenerateReference();
}
