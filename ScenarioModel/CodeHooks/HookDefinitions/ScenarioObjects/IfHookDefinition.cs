using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

public delegate bool IfHook(bool result);

[NodeLike<INodeHookDefinition, IfNode>]
public class IfHookDefinition(string Condition) : INodeHookDefinition
{
    [NodeLikeProperty]
    public List<bool> RecordedIfEvents { get; } = new();

    private bool IfHook(bool result)
    {
        RecordedIfEvents.Add(result);
        return result;
    }

    public IfHookDefinition GetConditionHook(out IfHook ifHook)
    {
        ifHook = IfHook;
        return this;
    }

    public IScenarioNode GetNode()
    {
        IfNode node = new();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(Condition);

        if (result.HasErrors)
        {
            throw new Exception($@"Unable to parse expression ""{Condition}"" on if declaration : \n{result.Errors.CommaSeparatedList()}");
        }

        node.Condition = result.ParsedObject;

        return node;
    }
}
