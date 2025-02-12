using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Serialisation.Expressions.Interpreter;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

[StoryNodeLike<INodeHookDefinition, IfNode>]
public class IfHookDefinition : IConditionRegistrationNodeHookDefinition<IfHookDefinition, BifurcatingHook>
{
    private readonly IHookFunctions _hookFunctions;

    [StoryNodeLikeProperty]
    public List<bool> RecordedIfEvents { get; } = new();

    public bool Validated { get; private set; } = false;
    public IfNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public IfHookDefinition(DefinitionScope scope, string expression, IHookFunctions hookFunctions)
    {
        _hookFunctions = hookFunctions;
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(expression);

        if (result.HasErrors)
            throw new Exception($@"Unable to parse expression ""{expression}"" on if declaration : \n{result.Errors.CommaSeparatedList()}");

        Node = new IfNode();
        Node.OriginalConditionText = expression;
        Node.Condition = result.ParsedObject ?? throw new Exception("Parsed object is null");
    }

    private bool IfConditionHook(bool result)
    {
        // This is where it should be, that is when it's used ; once the condition is evoked and not when the hook declaration is made.
        _hookFunctions.FinaliseDefinition(this);
        _hookFunctions.RegisterEventForHook(this, e => ((IfBlockEvent)e).IfBlockRun = result);

        if (result)
        {
            _hookFunctions.EnterScope(new DefinitionScope(Node.SubGraph, _hookFunctions.VerifyPreviousDefinition));
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
            _hookFunctions.ReturnOneScopeLevel();
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

    private bool _gotConditionHook = false;
    public IfHookDefinition GetConditionHook(out BifurcatingHook ifconditionHook)
    {
        _gotConditionHook = true;
        ifconditionHook = IfConditionHook;
        return this;
    }

    private bool _gotEndHook = false;
    public IfHookDefinition GetEndBlockHook(out BlockEndHook ifBlockEndHook)
    {
        _gotEndHook = true;
        ifBlockEndHook = IfBlockEndHook;
        return this;
    }

    public IfHookDefinition GetScopeHook(out ScopeDefiningHook ifBlockUsingHook)
    {
        _gotEndHook = true;
        ifBlockUsingHook = IfBlockUsingHook;
        return this;
    }

    //public IfHookDefinition GetConditionHooks(out IfConditionHook ifconditionHook, out IfBlockEndHook ifBlockEndHook)
    //{
    //    ifconditionHook = IfConditionHook;
    //    ifBlockEndHook = IfBlockEndHook;
    //    return this;
    //}

    //public IfHookDefinition GetConditionUsingHook(out IfConditionHook ifconditionHook, out IfBlockUsingHook ifBlockUsingHook)
    //{
    //    ifconditionHook = IfConditionHook;
    //    ifBlockUsingHook = IfBlockUsingHook;
    //    return this;
    //}

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
        if (!_gotEndHook)
        {
            throw new Exception($"The hook declaration did not ask for a hook scoping callback, call either {nameof(GetEndBlockHook)} or {nameof(GetScopeHook)}");
        }

        if (!_gotConditionHook)
        {
            throw new Exception($"The hook declaration did not ask for a condition hook callback, call {nameof(GetConditionHook)}");
        }

        // TODO
        // If condition == false, then no recorded events
        // If condition == true, then one recorded event
        // How to take into account multiple usages ?
        Validated = true;
    }

    public void Build()
    {
        Validate();
    }

    public void ReplaceNodeWithExisting(IStoryNode preexistingNode)
    {
        if (preexistingNode is not IfNode node)
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
