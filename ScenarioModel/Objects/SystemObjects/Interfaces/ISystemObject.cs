using ScenarioModel.Objects.Visitors;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel.Objects.SystemObjects.Interfaces;

public interface ISystemObject : IIdentifiable
{
    object Accept(ISystemVisitor visitor);
}

public interface ISystemObject<TRef> : ISystemObject, IReferencable<TRef>
    where TRef : IReference
{
    TRef GenerateReference();
}
