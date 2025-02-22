using Microsoft.Extensions.DependencyInjection.Extensions;
using ScenarioModelling.CoreObjects.ContextValidation;
using ScenarioModelling.CoreObjects.ContextValidation.SystemValidation;
using ScenarioModelling.CoreObjects.Expressions.Evaluation;
using ScenarioModelling.CoreObjects.Expressions.Traversal;
using ScenarioModelling.CoreObjects.Interpolation;
using ScenarioModelling.CoreObjects.Visitors;

namespace ScenarioModelling.CoreObjects.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        // Where is MetaState added to the services? It must be as a singleton
        // Where is MetaStory added ? I don't know how it should be added and injected since there can be multiple
        //  Perhaps a metastory stack. The complete list can be accessed via the context, but the current one is the top of the stack and the sub stories are pushed under it
        services.AddScoped<Context>();
        services.AddScoped<MetaState>();
        services.AddScoped<MetaStoryStack>(); // Singleton so that the flow of execution through several meta stories can be shared, but also implies that a single container can only run one story at a time !

        services.TryAddScoped<AspectValidator>();
        services.TryAddScoped<ConstraintValidator>();
        services.TryAddScoped<EntityValidator>();
        services.TryAddScoped<EntityTypeValidator>();
        services.TryAddScoped<RelationValidator>();
        services.TryAddScoped<StateValidator>();
        services.TryAddScoped<StateMachineValidator>();
        services.TryAddScoped<TransitionValidator>();

        services.TryAddScoped<MetaStoryValidator>();
        services.TryAddScoped<ISystemVisitor, MetaStateValidator>();
        services.TryAddScoped<MetaStateValidator>();

        services.TryAddScoped<ContextValidator>();
        services.TryAddScoped<IExpressionVisitor, ExpressionEvalator>();
        services.TryAddScoped<ExpressionEvalator>();
        services.TryAddScoped<StringInterpolator>();
    }
}
