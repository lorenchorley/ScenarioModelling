using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.References;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, StateTransitionNode>]
public class StateTransitionHookDefinition(System System, string StatefulObjectName, string Transition) : INodeHookDefinition
{
    [NodeLikeProperty]
    public string? Id { get; private set; }

    public IScenarioNode GetNode()
    {
        return new StateTransitionNode()
        {
            // Not sure
            Name = Id ?? "",
            StatefulObject = new StatefulObjectReference(System) { Name = StatefulObjectName },
            TransitionName = Transition
        };
    }

    public StateTransitionHookDefinition SetId(string id)
    {
        Id = id;
        return this;
    }
}
