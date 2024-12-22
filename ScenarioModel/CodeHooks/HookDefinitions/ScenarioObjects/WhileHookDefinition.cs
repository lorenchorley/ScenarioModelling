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
    private readonly EnterScopeDelegate _enterScope;
    private readonly ReturnOneScopeLevelDelegate _returnOneScopeLevel;

    public WhileNode Node { get; private set; }

    public WhileHookDefinition(string expression, DefinitionScope CurrentScope, EnterScopeDelegate enterScope, ReturnOneScopeLevelDelegate returnOneScopeLevel)
    {
        Node = new WhileNode();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(expression);

        if (result.HasErrors)
            throw new Exception($@"Unable to parse expression ""{expression}"" on while declaration : \n{result.Errors.CommaSeparatedList()}");

        Node.OriginalConditionText = expression;
        Node.Condition = result.ParsedObject ?? throw new Exception("Parsed object is null");
        _currentScope = CurrentScope;
        _enterScope = enterScope;
        _returnOneScopeLevel = returnOneScopeLevel;
    }

    private bool WhileHook(bool result)
    {
        _whileLoopCount++;

        RecordedWhileLoopEvents.Add(result);

        bool firstRun = _whileLoopCount == 1;
        if (firstRun)
        {
            _currentScope.AddNodeDefintion(this);
        }

        if (result)
        {
            // Continue the loop
            _enterScope(new DefinitionScope()
            {
                SubGraph = Node.SubGraph
            });

            _currentScope.SetCurrentNodeDefintion(this);
        }
        else
        {
            // End the loop
            _returnOneScopeLevel();
        }

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

    public void ValidateFinalState()
    {
    }
}
