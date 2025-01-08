using ScenarioModel.Collections.Graph;
using ScenarioModel.Execution.Events.Interfaces;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.Visitors;

namespace ScenarioModel.Objects.ScenarioNodes.BaseClasses;

public interface IScenarioNode : IDirectedGraphNode<IScenarioNode>, IIdentifiable
{
    int? Line { get; set; }
    IScenarioEvent GenerateUntypedEvent(EventGenerationDependencies dependencies);
    OneOfIScenaroNode ToOneOf();
    object Accept(IScenarioVisitor visitor);
    string? LineInformation { get; }
    bool IsFullyEqv(IScenarioNode other);
}
