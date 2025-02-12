using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;

namespace ScenarioModelling.CoreObjects.SystemObjects.Interfaces;

public interface ISystemObject : IIdentifiable
{
    object Accept(ISystemVisitor visitor);
    //string? LineInformation { get; }
}

public interface ISystemObject<TRef> : ISystemObject, IReferencable<TRef>
    where TRef : IReference
{
    void InitialiseAfterDeserialisation(MetaState system);
    TRef GenerateReference();
}
