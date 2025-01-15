using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using ScenarioModelling.References.GeneralisedReferences;

namespace ScenarioModelling.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, TransitionNode>]
public class TransitionHookDefinition : INodeHookDefinition
{
    private readonly Action _finaliseDefinition;

    public bool Validated { get; private set; } = false;
    public TransitionNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public TransitionHookDefinition(DefinitionScope scope, System System, string StatefulObjectName, string Transition, Action finaliseDefinition)
    {
        Node = new TransitionNode()
        {
            // Not sure
            StatefulObject = new StatefulObjectReference(System) { Name = StatefulObjectName },
            TransitionName = Transition
        };
        Scope = scope;
        _finaliseDefinition = finaliseDefinition;
        ScopeSnapshot = Scope.TakeSnapshot();
    }

    public IScenarioNode GetNode()
    {
        return Node;
    }

    public TransitionHookDefinition SetId(string id)
    {
        Node.Name = id;
        return this;
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
        if (preexistingNode is not TransitionNode node)
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
