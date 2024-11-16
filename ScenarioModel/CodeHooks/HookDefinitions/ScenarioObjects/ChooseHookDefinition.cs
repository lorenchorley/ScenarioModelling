using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

public delegate bool ChooseHook(bool result);

[NodeLike<INodeHookDefinition, ChooseNode>]
public class ChooseHookDefinition : INodeHookDefinition
{
    [NodeLikeProperty]
    public string? Id { get; private set; }

    [NodeLikeProperty]
    public ChoiceList Choices { get; } = new();

    [NodeLikeProperty]
    public List<bool> RecordedChooseEvents { get; } = new();

    private bool ChooseHook(bool result)
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
        Id = id;
        return this;
    }

    public ChooseHookDefinition WithJump(string nodeId, string choiceName)
    {
        Choices.Add((nodeId, choiceName));
        return this;
    }

    public IScenarioNode GetNode()
    {
        return new ChooseNode()
        {
            Choices = Choices,
            Name = Id ?? "" // Not sure
        };
    }
}
