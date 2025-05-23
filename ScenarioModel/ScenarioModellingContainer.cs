﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Reflection;

namespace ScenarioModelling;

public class ScenarioModellingContainer : IDisposable
{
    private readonly IHost _app;

    public ScenarioModellingContainer()
    {
        ExhaustivenessChecks();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        ConfigureHost(builder.Configuration);
        ConfigureLogging(builder.Logging);
        ConfigureServices(builder.Services);
        ConfigureMainServices(builder.Services);

        _app = builder.Build();

        InitialiseServices(_app.Services);

        _app.Start();
    }

    private static void ExhaustivenessChecks()
    {
        MetaStateObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IMetaStateObject>();
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

    protected virtual void ConfigureLogging(ILoggingBuilder logging)
    {
        // Overridable behaviour

        // This explicitly excludes the use of the EventLog provider which is windows specific, AddEventLog must not be re-added to this list
        logging.ClearProviders();
        //logging.AddConsole(); // This is not compatible with a wasm runtime because it directly manipulates threads
        logging.AddDebug();
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
