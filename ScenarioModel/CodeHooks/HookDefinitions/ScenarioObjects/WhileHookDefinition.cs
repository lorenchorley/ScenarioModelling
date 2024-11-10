using ScenarioModel.Exhaustiveness;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

public delegate bool WhileHook(bool result);

[NodeLike<INodeHookDefinition, WhileNode>]
public class WhileHookDefinition(string Condition, DefinitionScope CurrentScope) : INodeHookDefinition
{
    [NodeLikeProperty]
    public List<bool> RecordedWhileLoopEvents { get; } = new();

    private int _whileLoopCount = 0;

    private bool WhileHook(bool result)
    {
        RecordedWhileLoopEvents.Add(result);

        if (_whileLoopCount == 0)
        {
            CurrentScope.AddNodeDefintion(this);
        }
        else
        {
            CurrentScope.SetCurrentNodeDefintion(this);
        }

        return result;
    }

    public WhileHookDefinition GetConditionHook(out WhileHook whileHook)
    {
        whileHook = WhileHook;
        return this;
    }

    private WhileNode? _node;

    public IScenarioNode GetNode()
    {
        if (_node is not null)
        {
            return _node;
        }

        _node = new();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(Condition);

        if (result.HasErrors)
        {
            throw new Exception($@"Unable to parse expression ""{Condition}"" on while declaration : \n{result.Errors.CommaSeparatedList()}");
        }

        _node.Condition = result.ParsedObject;

        return _node;
    }
}
