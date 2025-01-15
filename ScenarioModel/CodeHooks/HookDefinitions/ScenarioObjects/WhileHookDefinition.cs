using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.Interpreter;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.ScenarioObjects;

public delegate bool WhileHook(bool result);

[NodeLike<INodeHookDefinition, WhileNode>]
public class WhileHookDefinition : INodeHookDefinition
{
    private int _whileLoopCount = 0;
    private DefinitionScope? _whileLoopScope;
    private readonly EnterScopeDelegate _enterScope;
    private readonly ReturnOneScopeLevelDelegate _returnOneScopeLevel;
    private readonly Action _verifyPreviousDefinition;
    private readonly Action _finaliseDefinition;

    [NodeLikeProperty]
    public List<bool> RecordedWhileLoopEvents { get; } = new();

    public bool Validated { get; private set; } = false;
    public WhileNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public WhileHookDefinition(DefinitionScope scope, string expression, EnterScopeDelegate enterScope, ReturnOneScopeLevelDelegate returnOneScopeLevel, Action verifyPreviousDefinition, Action finaliseDefinition)
    {
        Node = new WhileNode();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(expression);

        if (result.HasErrors)
            throw new Exception($@"Unable to parse expression ""{expression}"" on while declaration : \n{result.Errors.CommaSeparatedList()}");

        Node.OriginalConditionText = expression;
        Node.Condition = result.ParsedObject ?? throw new Exception("Parsed object is null");
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();
        _enterScope = enterScope;
        _returnOneScopeLevel = returnOneScopeLevel;
        _verifyPreviousDefinition = verifyPreviousDefinition;
        _finaliseDefinition = finaliseDefinition;
    }

    private bool WhileHook(bool result)
    {
        _whileLoopCount++;

        RecordedWhileLoopEvents.Add(result);

        bool firstRun = _whileLoopCount == 1;

        // Otherwise on the first run we need to enter the loop's scope
        if (firstRun)
        {
            // If the first result is false, we don't enter the loop at all
            if (!result)
                return result;

            // Continue the loop
            _whileLoopScope = new DefinitionScope(Node.SubGraph, _verifyPreviousDefinition);
            _enterScope(_whileLoopScope);
        }

        if (result)
        {
            // Reset the position in the while loop subgraph to the beginning
            ArgumentNullException.ThrowIfNull(_whileLoopScope);
            _whileLoopScope.ReturnToStartOfScope();
        }
        else
        {
            // End the loop and return to the parent scope
            _returnOneScopeLevel();
        }

        return result;
    }

    public WhileHookDefinition GetConditionHook(out WhileHook whileHook)
    {
        whileHook = WhileHook;
        return this;
    }

    public WhileHookDefinition SetAsImplicit()
    {
        Node.Implicit = true;
        return this;
    }

    public IScenarioNode GetNode()
    {
        return Node;
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
        if (preexistingNode is not WhileNode node)
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
