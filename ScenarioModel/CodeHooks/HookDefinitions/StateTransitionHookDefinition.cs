using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects;

namespace ScenarioModel.CodeHooks.HookDefinitions;

[NodeLike<INodeHookDefinition, StateTransitionNode>]
public class StateTransitionHookDefinition(string Name) : INodeHookDefinition
{
    [NodeLikeProperty]
    public string? Id { get; private set; }

    public StateTransitionHookDefinition SetId(string id)
    {
        Id = id;
        return this;
    }
}
