using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions;

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
