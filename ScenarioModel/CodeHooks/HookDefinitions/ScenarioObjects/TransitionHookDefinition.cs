using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.References.GeneralisedReferences;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, TransitionNode>]
public class TransitionHookDefinition : INodeHookDefinition
{
    public TransitionNode Node { get; private set; }

    public TransitionHookDefinition(System System, string StatefulObjectName, string Transition)
    {
        Node = new TransitionNode()
        {
            // Not sure
            StatefulObject = new StatefulObjectReference(System) { Name = StatefulObjectName },
            TransitionName = Transition
        };
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

    public void ValidateFinalState()
    {
    }
}
