using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.Interpreter;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;


[StoryNodeLike<INodeHookDefinition, WhileNode>]
public class WhileHookDefinition : IConditionRegistrationNodeHookDefinition<WhileHookDefinition, BifurcatingHook>
{
    private int _whileLoopCount = 0;
    private DefinitionScope? _whileLoopScope;
    private readonly EnterScopeDelegate _enterScope;
    private readonly ReturnOneScopeLevelDelegate _returnOneScopeLevel;
    private readonly Action _verifyPreviousDefinition;
    private readonly FinaliseDefinitionDelegate _finaliseDefinition;

    [StoryNodeLikeProperty]
    public List<bool> RecordedWhileLoopEvents { get; } = new();

    public bool Validated { get; private set; } = false;
    public WhileNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public WhileHookDefinition(DefinitionScope scope, string expression, EnterScopeDelegate enterScope, ReturnOneScopeLevelDelegate returnOneScopeLevel, Action verifyPreviousDefinition, FinaliseDefinitionDelegate finaliseDefinition)
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
        bool firstRun = _whileLoopCount == 1;
        
        RecordedWhileLoopEvents.Add(result);

        // Otherwise on the first run we need to enter the loop's scope
        if (firstRun)
        {
            // This is where it should be, that is when it's used ; once the condition is evoked and not when the hook declaration is made.
            // But only on the first usage for a loop
            _finaliseDefinition(this);

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
            ArgumentNullExceptionStandard.ThrowIfNull(_whileLoopScope);
            _whileLoopScope.ReturnToStartOfScope();
        }
        else
        {
            // End the loop and return to the parent scope
            _returnOneScopeLevel();
        }

        return result;
    }

    private bool _gotConditionHook = false;

    public WhileHookDefinition GetConditionHook(out BifurcatingHook whileHook)
    {
        _gotConditionHook = true;
        whileHook = WhileHook;
        return this;
    }

    public WhileHookDefinition SetAsImplicit()
    {
        Node.Implicit = true;
        return this;
    }

    public IStoryNode GetNode()
    {
        return Node;
    }

    public void Validate()
    {
        if (!_gotConditionHook)
        {
            throw new Exception($"The hook declaration did not ask for a condition hook callback, call {nameof(GetConditionHook)}");
        }

        Validated = true;
    }

    public void Build()
    {
        Validate();
    }

    public void ReplaceNodeWithExisting(IStoryNode preexistingNode)
    {
        if (preexistingNode is not WhileNode node)
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
