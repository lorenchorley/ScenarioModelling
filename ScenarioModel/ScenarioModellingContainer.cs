using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Mocks;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers.Interfaces;
using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling;

public class ScenarioModellingContainer : IDisposable
{
    private readonly IHost _app;

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
    }

    private static void ExhaustivenessChecks()
    {
        MetaStateObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<ISystemObject>();
        MetaStateObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IReference>();
        MetaStateObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IObjectValidator>();
        MetaStateObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IObjectSerialiser>();
        MetaStateObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IDefinitionToObjectDeserialiser>();

        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<IStoryNode>();
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<IMetaStoryEvent>();
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<INodeHookDefinition>();
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<INodeValidator>();
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<INodeSerialiser>();
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<IDefinitionToNodeDeserialiser>();
    }

    private void ConfigureMainServices(IServiceCollection services)
    {
        CodeHooks.Extensions.ServiceExtensions.ConfigureServices(services);
        Serialisation.Extensions.ServiceExtensions.ConfigureServices(services);
        Execution.Extensions.ServiceExtensions.ConfigureServices(services);
        CoreObjects.Extensions.ServiceExtensions.ConfigureServices(services);
        Exhaustiveness.Extensions.ServiceExtensions.ConfigureServices(services);
        Mocks.Extensions.ServiceExtensions.ConfigureServices(services);
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

    public void Dispose()
    {
        _app.Dispose();
    }

    public ScenarioModellingContainerScope StartScope()
    {
        return new ScenarioModellingContainerScope(_app.Services.CreateScope());
    }
}
