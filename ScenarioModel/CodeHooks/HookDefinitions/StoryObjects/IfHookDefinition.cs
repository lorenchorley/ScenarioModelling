using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.Interpreter;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

public delegate bool IfConditionHook(bool result);
public delegate void IfBlockEndHook();
public delegate IDisposable IfBlockUsingHook();

[StoryNodeLike<INodeHookDefinition, IfNode>]
public class IfHookDefinition : INodeHookDefinition
{
    private readonly EnterScopeDelegate _enterScope;
    private readonly ReturnOneScopeLevelDelegate _returnOneScopeLevel;
    private readonly Action _verifyPreviousDefinition;
    private readonly Action _finaliseDefinition;

    [StoryNodeLikeProperty]
    public List<bool> RecordedIfEvents { get; } = new();

    public bool Validated { get; private set; } = false;
    public IfNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public IfHookDefinition(DefinitionScope scope, string expression, EnterScopeDelegate enterScope, ReturnOneScopeLevelDelegate returnOneScopeLevel, Action verifyPreviousDefinition, Action finaliseDefinition)
    {
        Node = new IfNode();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(expression);

        if (result.HasErrors)
            throw new Exception($@"Unable to parse expression ""{expression}"" on if declaration : \n{result.Errors.CommaSeparatedList()}");

        Node.OriginalConditionText = expression;
        Node.Condition = result.ParsedObject ?? throw new Exception("Parsed object is null");
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();
        _enterScope = enterScope;
        _returnOneScopeLevel = returnOneScopeLevel;
        _verifyPreviousDefinition = verifyPreviousDefinition;
        _finaliseDefinition = finaliseDefinition;
    }

    private bool IfConditionHook(bool result)
    {
        if (result)
        {
            _enterScope(new DefinitionScope(Node.SubGraph, _verifyPreviousDefinition));
        }

        RecordedIfEvents.Add(result);
        return result;
    }

    private void IfBlockEndHook()
    {
        if (RecordedIfEvents.Count == 0)
            throw new Exception("If block end hook called without any recorded events");

        // Only return the scope if the last recorded event was true
        // This gives flexibility in that this callback can be either inside the if scope at the end or after it outside
        if (RecordedIfEvents.Last() == true)
            _returnOneScopeLevel();
    }

    private class UsingHookScope(Action endScope) : IDisposable
    {
        public void Dispose()
        {
            endScope();
        }
    }

    private IDisposable IfBlockUsingHook()
    {
        return new UsingHookScope(IfBlockEndHook);
    }

    public IfHookDefinition GetConditionHooks(out IfConditionHook ifconditionHook, out IfBlockEndHook ifBlockEndHook)
    {
        ifconditionHook = IfConditionHook;
        ifBlockEndHook = IfBlockEndHook;
        return this;
    }

    public IfHookDefinition GetConditionUsingHook(out IfConditionHook ifconditionHook, out IfBlockUsingHook ifBlockUsingHook)
    {
        ifconditionHook = IfConditionHook;
        ifBlockUsingHook = IfBlockUsingHook;
        return this;
    }

    public IfHookDefinition SetAsImplicit()
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
        // TODO
        // If condition == false, then no recorded events
        // If condition == true, then one recorded event
        // How to take into account multiple usages ?
        Validated = true;
    }

    public void Build()
    {
        Validate();
        _finaliseDefinition();
    }

    public void ReplaceNodeWithExisting(IStoryNode preexistingNode)
    {
        if (preexistingNode is not IfNode node)
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
