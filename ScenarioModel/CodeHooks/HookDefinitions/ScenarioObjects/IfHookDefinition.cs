using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

public delegate bool IfHook(bool result);

[NodeLike<INodeHookDefinition, IfNode>]
public class IfHookDefinition : INodeHookDefinition
{
    [NodeLikeProperty]
    public List<bool> RecordedIfEvents { get; } = new();

    public IfNode Node { get; private set; }

    public IfHookDefinition(string Condition)
    {
        Node = new IfNode();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(Condition);

        if (result.HasErrors)
            throw new Exception($@"Unable to parse expression ""{Condition}"" on if declaration : \n{result.Errors.CommaSeparatedList()}");

        Node.Condition = result.ParsedObject ?? throw new Exception("Parsed object is null");
    }

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
        return Node;
    }
}
