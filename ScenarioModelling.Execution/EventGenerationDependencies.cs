using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Expressions.Evaluation;
using ScenarioModelling.CoreObjects.Interpolation;

namespace ScenarioModelling.Execution;

public class EventGenerationDependencies
{
    public StringInterpolator Interpolator { get; set; }
    public ExpressionEvalator Evaluator { get; set; }
    public IExecutor Executor { get; set; }
    public Context Context { get; set; }

    public EventGenerationDependencies(StringInterpolator interpolator, ExpressionEvalator evaluator, IExecutor executor, Context context)
    {
        Interpolator = interpolator;
        Evaluator = evaluator;
        Executor = executor;
        Context = context;
    }
}
