using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, JumpNode>]
public class JumpHookDefinition : INodeHookDefinition
{
    public bool Validated { get; private set; } = false;
    public DefinitionScope CurrentScope { get; }
    public JumpNode Node { get; private set; }

    public JumpHookDefinition(DefinitionScope currentScope, string target)
    {
        Node = new JumpNode()
        {
            Target = target
        };
        CurrentScope = currentScope;
    }

    public JumpHookDefinition SetId(string id)
    {
        Node.Name = id;

        return this;
    }

    public JumpHookDefinition SetAsImplicit()
    {
        Node.Implicit = true;
        return this;
    }

    public IScenarioNode GetNode()
    {
        return Node;
    }

    public void Validate()
    {
        Validated = true;
    }
}
