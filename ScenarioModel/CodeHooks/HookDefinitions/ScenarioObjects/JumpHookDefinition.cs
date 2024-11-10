using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, JumpNode>]
public class JumpHookDefinition(string Name) : INodeHookDefinition
{
    public IScenarioNode GetNode()
    {
        return new JumpNode()
        {
            Name = Name
        };
    }
}
