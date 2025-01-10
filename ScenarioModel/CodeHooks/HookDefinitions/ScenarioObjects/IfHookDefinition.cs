using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

public delegate bool IfConditionHook(bool result);
public delegate void IfBlockEndHook();
public delegate IDisposable IfBlockUsingHook();

[NodeLike<INodeHookDefinition, IfNode>]
public class IfHookDefinition : INodeHookDefinition
{
    private readonly EnterScopeDelegate _enterScope;
    private readonly ReturnOneScopeLevelDelegate _returnOneScopeLevel;

    [NodeLikeProperty]
    public List<bool> RecordedIfEvents { get; } = new();

    public bool Validated { get; private set; } = false;
    public IfNode Node { get; private set; }
    public DefinitionScope CurrentScope { get; }

    public IfHookDefinition(DefinitionScope currentScope, string expression, EnterScopeDelegate enterScope, ReturnOneScopeLevelDelegate returnOneScopeLevel)
    {
        Node = new IfNode();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(expression);

        if (result.HasErrors)
            throw new Exception($@"Unable to parse expression ""{expression}"" on if declaration : \n{result.Errors.CommaSeparatedList()}");

        Node.OriginalConditionText = expression;
        Node.Condition = result.ParsedObject ?? throw new Exception("Parsed object is null");
        CurrentScope = currentScope;
        _enterScope = enterScope;
        _returnOneScopeLevel = returnOneScopeLevel;
    }

    private bool IfConditionHook(bool result)
    {
        if (result)
        {
            _enterScope(new DefinitionScope()
            {
                SubGraph = Node.SubGraph
            });
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

    public IScenarioNode GetNode()
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
}
