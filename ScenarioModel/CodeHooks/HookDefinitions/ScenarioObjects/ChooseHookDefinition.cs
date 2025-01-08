using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

public delegate string ChooseHook(string result);

[NodeLike<INodeHookDefinition, ChooseNode>]
public class ChooseHookDefinition : INodeHookDefinition
{
    [NodeLikeProperty]
    public List<string> RecordedChooseEvents { get; } = new();

    public ChooseNode Node { get; private set; }

    public ChooseHookDefinition()
    {
        Node = new ChooseNode();
    }

    private string ChooseHook(string result)
    {
        RecordedChooseEvents.Add(result);
        return result;
    }

    public ChooseHookDefinition GetConditionHook(out ChooseHook chooseCondition)
    {
        chooseCondition = ChooseHook;
        return this;
    }

    public ChooseHookDefinition SetId(string id)
    {
        Node.Name = id;
        return this;
    }

    public ChooseHookDefinition WithJump(string nodeId, string choiceName)
    {
        Node.Choices.Add((nodeId, choiceName));
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
