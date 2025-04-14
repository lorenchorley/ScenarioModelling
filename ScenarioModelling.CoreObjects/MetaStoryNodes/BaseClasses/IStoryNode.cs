using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;

[ProtoContract]
public interface IStoryNode : ICategoryClass, IDirectedGraphNode<IStoryNode>, IIdentifiable
{
    /// <summary>
    /// The line number in the source file where the node is defined, if the node came from a serialised source file
    /// </summary>
    int? Line { get; set; }

    /// <summary>
    /// Human readable text that describes the line number of the source file where the node is defined
    /// </summary>
    string? LineInformation { get; }

    /// <summary>
    /// Defines whether the node is implicit or not, that is whether the node's existance may be inferred from the existance of other nodes around it in execution
    /// </summary>
    bool Implicit { get; }

    /// <summary>
    /// Allows for automatically producing a OneOf object from the node
    /// </summary>
    /// <returns></returns>
    OneOfScenaroNode ToOneOf();

    /// <summary>
    /// Allows for the visitor pattern to be used on the node
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    object Accept(IMetaStoryVisitor visitor);
    
    /// <summary>
    /// Allows for the visitor pattern to be used on the node
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    Task<object> Accept(IMetaStoryAsyncVisitor visitor);

    /// <summary>
    /// Checks only name and type
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    ///bool IsEqv(IMetaStoryNode other);

    /// <summary>
    /// Checks all properties of the object without delving into subnodes
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    bool IsFullyEqv(IStoryNode other);

    /// <summary>
    /// Fully checks equivalence of the object and subnodes
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    ///bool IsDeepEqv(IMetaStoryNode other);

}
