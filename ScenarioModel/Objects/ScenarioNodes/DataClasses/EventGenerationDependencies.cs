using ScenarioModel.Execution;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Interpolation;

namespace ScenarioModel.Objects.ScenarioNodes.DataClasses;

public record EventGenerationDependencies(StringInterpolator Interpolator, ExpressionEvalator Evaluator, IExecutor Executor, Context Context);
