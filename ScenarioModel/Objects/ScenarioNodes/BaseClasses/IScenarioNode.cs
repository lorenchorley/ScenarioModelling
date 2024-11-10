using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Objects.SystemObjects.Interfaces;

namespace ScenarioModel.Objects.ScenarioNodes.BaseClasses;

public interface IScenarioNode : IDirectedGraphNode<IScenarioNode>, IIdentifiable
{
    int? Line { get; set; }
    IScenarioEvent GenerateUntypedEvent(EventGenerationDependencies dependencies);
    OneOfIScenaroNode ToOneOf();
}
