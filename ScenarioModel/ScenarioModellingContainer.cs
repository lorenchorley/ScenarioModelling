using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.ContextValidation;
using ScenarioModelling.CoreObjects.Expressions.Evaluation;
using ScenarioModelling.CoreObjects.Expressions.Traversal;
using ScenarioModelling.CoreObjects.Interpolation;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;

namespace ScenarioModelling;

public class ScenarioModellingContainer : IDisposable
{
    private readonly IHost _app;

    public MetaState MetaState { get; private set; } = null!;
    public Context Context { get; private set; } = null!;

    public ScenarioModellingContainer()
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        ConfigureHost(builder.Configuration);
        ConfigureServices(builder.Services);
        ConfigureMainServices(builder.Services);

        _app = builder.Build();

        InitialiseServices(_app.Services);

        _app.Start();

        SetExposedProperties(_app.Services);

        //SystemObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IObjectValidator>();

    }

    private void SetExposedProperties(IServiceProvider services)
    {
        MetaState = GetService<MetaState>();
        Context = GetService<Context>();
    }

    private void ConfigureMainServices(IServiceCollection services)
    {
        
        CodeHooks.Extensions.ServiceExtensions.ConfigureServices(services);
        Serialisation.Extensions.ServiceExtensions.ConfigureServices(services);
        Execution.Extensions.ServiceExtensions.ConfigureServices(services);
        CoreObjects.Extensions.ServiceExtensions.ConfigureServices(services);
        Exhaustiveness.Extensions.ServiceExtensions.ConfigureServices(services);
    }

    protected virtual void ConfigureHost(ConfigurationManager configuration)
    {
        // Overridable behaviour
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Overridable behaviour
    }

    protected virtual void InitialiseServices(IServiceProvider services)
    {
        // Overridable behaviour
    }

    internal T GetService<T>() where T : notnull
    {
        return _app.Services.GetRequiredService<T>();
    }

    public void Dispose()
    {
        _app.Dispose();
    }
}
