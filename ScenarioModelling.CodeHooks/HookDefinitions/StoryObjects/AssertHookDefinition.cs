using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Serialisation.Expressions.Interpreter;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

[StoryNodeLike<INodeHookDefinition, AssertNode>]
public class AssertHookDefinition : IInSituNodeHookDefinition
{
    private readonly IMetaStoryHookFunctions _hookFunctions;

    [StoryNodeLikeProperty]
    public List<bool> RecordedAssertEvents { get; } = new();

    public bool Validated { get; private set; } = false;
    public AssertNode Node { get; private set; }
    public SubgraphScopedHookSynchroniser Scope { get; }
    public SubGraphScopeSnapshot ScopeSnapshot { get; }

    public AssertHookDefinition(SubgraphScopedHookSynchroniser scope, string expression, IMetaStoryHookFunctions hookFunctions)
    {
        _hookFunctions = hookFunctions;
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(expression);

        if (result.HasErrors)
            throw new ExpressionException($@"Unable to parse expression ""{expression}"" on assert declaration : \n{result.Errors.CommaSeparatedList()}");

        Node = new AssertNode();
        Node.OriginalExpressionText = expression;
        Node.AssertionExpression = result.ParsedObject ?? throw new InternalLogicException($@"The expression ""{expression}"" resulted in a null value after being parsed");
    }

    private bool AssertConditionHook(bool result)
    {
        // This is where it should be, that is when it's used ; once the condition is evoked and not when the hook declaration is made.
        _hookFunctions.FinaliseDefinition(this);
        _hookFunctions.RegisterEventForHook(this, e => ((AssertionEvent)e).AssertionSucceeded = result);

        RecordedAssertEvents.Add(result);

        return result;
    }

    public IStoryNode GetNode()
    {
        return Node;
    }

    public void Validate()
    {
        Validated = true;
    }

    public void BuildAndRegister()
    {
        Validate();
        _hookFunctions.FinaliseDefinition(this);
        _hookFunctions.RegisterEventForHook(this, _ => { });
    }

    public void ReplaceNodeWithExisting(IStoryNode preexistingNode)
    {
        if (preexistingNode is not AssertNode node)
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
