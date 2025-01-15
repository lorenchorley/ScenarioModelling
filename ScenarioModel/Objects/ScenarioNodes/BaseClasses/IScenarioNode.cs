using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Objects.ScenarioNodes.DataClasses;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.Visitors;

namespace ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

public interface IScenarioNode : IDirectedGraphNode<IScenarioNode>, IIdentifiable
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
    /// Generate a scenario event without specifying the node type. This is the best we can do on this interface because it's generic as well.
    /// </summary>
    /// <param name="dependencies"></param>
    /// <returns></returns>
    IScenarioEvent GenerateGenericTypeEvent(EventGenerationDependencies dependencies);

    /// <summary>
    /// Allows for automatically producing a OneOf object from the node
    /// </summary>
    /// <returns></returns>
    OneOfIScenaroNode ToOneOf();

    /// <summary>
    /// Allows for the visitor pattern to be used on the node
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    object Accept(IScenarioVisitor visitor);

    /// <summary>
    /// Checks only name and type
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    ///bool IsEqv(IScenarioNode other);

    /// <summary>
    /// Checks all properties of the object without delving into subnodes
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    bool IsFullyEqv(IScenarioNode other);

    /// <summary>
    /// Fully checks equivalence of the object and subnodes
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    ///bool IsDeepEqv(IScenarioNode other);

}
