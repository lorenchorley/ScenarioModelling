using ScenarioModel.Exhaustiveness;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions;

public delegate bool WhileHook(bool result);

[NodeLike<INodeHookDefinition, WhileNode>]
public class WhileHookDefinition(string Condition) : INodeHookDefinition
{
    [NodeLikeProperty]
    public List<bool> RecordedWhileLoopEvents { get; } = new();

    private bool WhileHook(bool result)
    {
        RecordedWhileLoopEvents.Add(result);
        return result;
    }

    public WhileHookDefinition GetConditionHook(out WhileHook whileHook)
    {
        whileHook = WhileHook;
        return this;
    }

    public IScenarioNode GetNode()
    {
        WhileNode node = new();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(Condition);

        if (result.HasErrors)
        {
            throw new Exception($@"Unable to parse expression ""{Condition}"" on while declaration : \n{result.Errors.CommaSeparatedList()}");
        }

        node.Condition = result.ParsedObject;

        return node;
    }
}
