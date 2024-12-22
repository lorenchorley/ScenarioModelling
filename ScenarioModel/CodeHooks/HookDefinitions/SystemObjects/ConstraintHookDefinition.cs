using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;

namespace ScenarioModel.CodeHooks.HookDefinitions.SystemObjects;

public class ConstraintHookDefinition : IObjectHookDefinition
{
    private readonly System _system;
    private readonly Instanciator _instanciator;

    public HookExecutionMode HookExecutionMode { get; set; }
    public Constraint Constraint { get; private set; }

    public ConstraintHookDefinition(System system, Instanciator instanciator, string name)
    {
        _system = system;
        _instanciator = instanciator;

        // Either create a new one or find an existing one in the provided system
        Constraint = _instanciator.GetOrNew<Constraint, ConstraintReference>(name);
    }

    public ConstraintHookDefinition SetExpression(string expression)
    {
        // TODO Either set the state or verify that the states match

        // Parse the expression before adding it to the node
        ExpressionInterpreter interpreter = new();
        var result = interpreter.Parse(expression);

        if (result.HasErrors)
            throw new Exception($@"Unable to parse expression ""{expression}"" on if declaration : \n{result.Errors.CommaSeparatedList()}");

        Constraint.OriginalConditionText = expression;
        Constraint.Condition = result.ParsedObject ?? throw new Exception("Parsed object is null");

        return this;
    }

}
