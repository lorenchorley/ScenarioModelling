using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;

namespace ScenarioModelling;

public class ScenarioModellingContainer : IDisposable
{
    private readonly IHost _app;

    public MetaState MetaState { get; private set; } = null!;
    public Context Context { get; private set; } = null!;

    public ScenarioModellingContainer()
    {
        ExhaustivenessChecks();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        ConfigureHost(builder.Configuration);
        ConfigureServices(builder.Services);
        ConfigureMainServices(builder.Services);

        _app = builder.Build();

        InitialiseServices(_app.Services);

        _app.Start();

        SetExposedProperties(_app.Services);
    }

    private static void ExhaustivenessChecks()
    {
        SystemObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<ISystemObject>();
        SystemObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IReference>();
        SystemObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IObjectValidator>();
        SystemObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IObjectSerialiser>();
        SystemObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IDefinitionToObjectDeserialiser>();

        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<IStoryNode>();
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<IMetaStoryEvent>();
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<INodeHookDefinition>();
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<INodeValidator>();
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<INodeSerialiser>();
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<IDefinitionToNodeDeserialiser>();
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
