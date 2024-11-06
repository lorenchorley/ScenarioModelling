using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.References;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, StateTransitionNode>]
public class StateTransitionHookDefinition(string StatefulObjectName, string Transition) : INodeHookDefinition
{
    [NodeLikeProperty]
    public string? Id { get; private set; }

    public IScenarioNode GetNode()
    {
        return new StateTransitionNode()
        {
            // Not sure
            Name = Id ?? "",
            StatefulObject = new GenericStatefulObjectReference(StatefulObjectName),
            TransitionName = Transition
        };
    }

    public StateTransitionHookDefinition SetId(string id)
    {
        Id = id;
        return this;
    }
}
