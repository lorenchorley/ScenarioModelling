using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, JumpNode>]
public class JumpHookDefinition : INodeHookDefinition
{
    private readonly Action _finaliseDefinition;

    public bool Validated { get; private set; } = false;
    public JumpNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public JumpHookDefinition(DefinitionScope scope, string target, Action finaliseDefinition)
    {
        Node = new JumpNode()
        {
            Target = target
        };
        Scope = scope;
        _finaliseDefinition = finaliseDefinition;
        ScopeSnapshot = Scope.TakeSnapshot();
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

    public void Build()
    {
        Validate();
        _finaliseDefinition();
    }


    public void ReplaceNodeWithExisting(IScenarioNode preexistingNode)
    {
        if (preexistingNode is not JumpNode node)
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
