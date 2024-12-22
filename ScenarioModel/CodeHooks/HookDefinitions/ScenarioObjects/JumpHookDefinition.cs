using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, JumpNode>]
public class JumpHookDefinition : INodeHookDefinition
{
    public JumpNode Node { get; private set; }

    public JumpHookDefinition(string target)
    {
        Node = new JumpNode()
        {
            Target = target
        };
    }

    public JumpHookDefinition SetId(string id)
    {
        Node.Name = id;

        return this;
    }

    public IScenarioNode GetNode()
    {
        return Node;
    }

    public void ValidateFinalState()
    {
    }
}
