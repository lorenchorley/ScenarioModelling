using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;

public interface IMetaStateObject : ICategoryClass, ICoreObject
{
    object Accept(IMetaStateVisitor visitor);
    //string? LineInformation { get; }

    /// <summary>
    /// Allows for automatically producing a OneOf object from the node
    /// </summary>
    /// <returns></returns>
    OneOfMetaStateObject ToOneOf();

}

public interface IMetaStateObject<TRef> : IMetaStateObject, IReferencable<TRef>
    where TRef : IReference
{
    void InitialiseAfterDeserialisation(MetaState system);
    TRef GenerateReference();
}
