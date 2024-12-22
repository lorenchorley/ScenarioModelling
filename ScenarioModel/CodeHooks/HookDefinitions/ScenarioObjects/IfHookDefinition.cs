using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

public delegate bool IfConditionHook(bool result);
public delegate void IfBlockEndHook();

[NodeLike<INodeHookDefinition, IfNode>]
public class IfHookDefinition : INodeHookDefinition
{
    private readonly EnterScopeDelegate _enterScope;
    private readonly ReturnOneScopeLevelDelegate _returnOneScopeLevel;

    [NodeLikeProperty]
    public List<bool> RecordedIfEvents { get; } = new();

    public IfNode Node { get; private set; }

    public IfHookDefinition(string expression, EnterScopeDelegate enterScope, ReturnOneScopeLevelDelegate returnOneScopeLevel)
    {
        Node = new IfNode();

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(expression);

        if (result.HasErrors)
            throw new Exception($@"Unable to parse expression ""{expression}"" on if declaration : \n{result.Errors.CommaSeparatedList()}");

        Node.OriginalConditionText = expression;
        Node.Condition = result.ParsedObject ?? throw new Exception("Parsed object is null");
        _enterScope = enterScope;
        _returnOneScopeLevel = returnOneScopeLevel;
    }

    private bool IfConditionHook(bool result)
    {
        _enterScope(new DefinitionScope()
        {
            SubGraph = Node.SubGraph
        });

        // TODO
        RecordedIfEvents.Add(result);
        return result;
    }

    private void IfBlockEndHook()
    {
        // TODO
        _returnOneScopeLevel();
    }

    public IfHookDefinition GetConditionHooks(out IfConditionHook ifconditionHook, out IfBlockEndHook ifBlockEndHook)
    {
        ifconditionHook = IfConditionHook;
        ifBlockEndHook = IfBlockEndHook;
        return this;
    }

    public IScenarioNode GetNode()
    {
        return Node;
    }

    public void ValidateFinalState()
    {
        // TODO
        // If condition == false, then no recorded events
        // If condition == true, then one recorded event
        // How to take into account multiple usages ?
    }
}
