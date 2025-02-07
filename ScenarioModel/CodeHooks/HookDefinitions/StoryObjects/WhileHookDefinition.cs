using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.Execution.Events;
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
    private readonly IHookFunctions _hookFunctions;

    [StoryNodeLikeProperty]
    public List<bool> RecordedWhileLoopEvents { get; } = new();

    public bool Validated { get; private set; } = false;
    public WhileNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public WhileHookDefinition(DefinitionScope scope, string expression, IHookFunctions hookFunctions)
    {
        _hookFunctions = hookFunctions;
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(expression);

        if (result.HasErrors)
            throw new Exception($@"Unable to parse expression ""{expression}"" on while declaration : \n{result.Errors.CommaSeparatedList()}");

        Node = new WhileNode();
        Node.OriginalConditionText = expression;
        Node.Condition = result.ParsedObject ?? throw new Exception("Parsed object is null");
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
            _hookFunctions.FinaliseDefinition(this);

            // If the first result is false, we don't enter the loop at all
            if (!result)
                return RegisterEventFromConditionResult(result); // We do not register the event here because it is managed by the call to _finaliseDefinition which also adds the node to the graph

            // Continue the loop
            _whileLoopScope = new DefinitionScope(Node.SubGraph, _hookFunctions.VerifyPreviousDefinition);
            _hookFunctions.EnterScope(_whileLoopScope);
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
            _hookFunctions.ReturnOneScopeLevel();
        }

        RegisterEventFromConditionResult(result);

        return result;
    }

    private bool RegisterEventFromConditionResult(bool result)
    {
        _hookFunctions.RegisterEventForHook(this, e => ((WhileLoopConditionCheckEvent)e).LoopBlockRun = result);
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
