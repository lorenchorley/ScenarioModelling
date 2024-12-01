using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

public delegate bool WhileHook(bool result);

[NodeLike<INodeHookDefinition, WhileNode>]
public class WhileHookDefinition : INodeHookDefinition
{
    [NodeLikeProperty]
    public List<bool> RecordedWhileLoopEvents { get; } = new();

    private int _whileLoopCount = 0;
    private readonly DefinitionScope _currentScope;

    public WhileNode Node { get; private set; }

    public WhileHookDefinition(string Condition, DefinitionScope CurrentScope)
    {
        Node = new WhileNode();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(Condition);

        if (result.HasErrors)
            throw new Exception($@"Unable to parse expression ""{Condition}"" on while declaration : \n{result.Errors.CommaSeparatedList()}");

        Node.Condition = result.ParsedObject ?? throw new Exception("Parsed object is null");
        _currentScope = CurrentScope;
    }

    private bool WhileHook(bool result)
    {
        RecordedWhileLoopEvents.Add(result);

        if (_whileLoopCount == 0)
            _currentScope.AddNodeDefintion(this);
        else
            _currentScope.SetCurrentNodeDefintion(this);

        return result;
    }

    public WhileHookDefinition GetConditionHook(out WhileHook whileHook)
    {
        whileHook = WhileHook;
        return this;
    }

    public IScenarioNode GetNode()
    {
        return Node;
    }
}
