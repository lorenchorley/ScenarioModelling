using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;

public interface ISystemObject : IIdentifiable
{
    object Accept(IMetaStateVisitor visitor);
    //string? LineInformation { get; }
}

public interface ISystemObject<TRef> : ISystemObject, IReferencable<TRef>
    where TRef : IReference
{
    void InitialiseAfterDeserialisation(MetaState system);
    TRef GenerateReference();
}
