using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Serialisation.Expressions.Interpreter;

namespace ScenarioModelling.CodeHooks.HookDefinitions.SystemObjects;

public class ConstraintHookDefinition : IObjectHookDefinition
{
    private readonly MetaState _metaState;
    private readonly Instanciator _instanciator;

    public HookExecutionMode HookExecutionMode { get; set; }
    public Constraint Constraint { get; private set; }

    public bool Validated { get; private set; } = false;

    public ConstraintHookDefinition(MetaState metaState, Instanciator instanciator, string name)
    {
        _metaState = metaState;
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

    public void Validate()
    {
        Validated = true;
    }
}
