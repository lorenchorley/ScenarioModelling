using ScenarioModelling.Execution;
using ScenarioModelling.Expressions.Evaluation;
using ScenarioModelling.Interpolation;

namespace ScenarioModelling.Objects.ScenarioNodes.DataClasses;

public record EventGenerationDependencies(StringInterpolator Interpolator, ExpressionEvalator Evaluator, IExecutor Executor, Context Context);
