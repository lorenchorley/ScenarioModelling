using Microsoft.Extensions.DependencyInjection;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Expressions.Initialisation;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Serialisation.Expressions;
using ScenarioModelling.Serialisation.Expressions.Interpreter;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.SystemObjectDeserialisers;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;
using ScenarioModelling.Serialisation.ProtoBuf;
using ScenarioModelling.Serialisation.Yaml;

namespace ScenarioModelling.Serialisation.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IContextSerialiser, ContextSerialiser>();
        services.AddSingleton<ContextSerialiser>();
        services.AddSingleton<IContextSerialiser, YamlSerialiser>();
        services.AddSingleton<YamlSerialiser>();
        services.AddSingleton<IContextSerialiser, ProtoBufSerialiser>();
        services.AddSingleton<ProtoBufSerialiser>();
        services.AddSingleton<IContextSerialiser, ProtoBufSerialiser_Uncompressed>();
        services.AddSingleton<ProtoBufSerialiser_Uncompressed>();

        services.AddSingleton<Instanciator>();
        services.AddSingleton<ContextDeserialiser>();
        services.AddSingleton<MetaStorySerialiser>();
        services.AddSingleton<MetaStoryTransformer>();
        services.AddSingleton<MetaStateSerialiser>();
        services.AddSingleton<ExpressionInterpreter>();
        services.AddSingleton<ExpressionInitialiser>();
        services.AddSingleton<ExpressionSerialiser>();

        services.AddSingleton<ChooseNodeSerialiser>();
        services.AddSingleton<DialogNodeSerialiser>();
        services.AddSingleton<IfNodeSerialiser>();
        services.AddSingleton<JumpNodeSerialiser>();
        services.AddSingleton<MetadataNodeSerialiser>();
        services.AddSingleton<TransitionNodeSerialiser>();
        services.AddSingleton<WhileNodeSerialiser>();

        services.AddSingleton<AspectSerialiser>();
        services.AddSingleton<ConstraintSerialiser>();
        services.AddSingleton<EntitySerialiser>();
        services.AddSingleton<EntityTypeSerialiser>();
        services.AddSingleton<RelationSerialiser>();
        services.AddSingleton<StateMachineSerialiser>();
        services.AddSingleton<StateSerialiser>();
        services.AddSingleton<TransitionSerialiser>();

        services.AddSingleton<RelationDeserialiser>();
        services.AddSingleton<ConstraintDeserialiser>();
        services.AddSingleton<StateDeserialiser>();
        services.AddSingleton<TransitionDeserialiser>();
        services.AddSingleton<AspectDeserialiser>();
        services.AddSingleton<StateMachineDeserialiser>();
        services.AddSingleton<EntityTypeDeserialiser>();
        services.AddSingleton<EntityDeserialiser>();

        services.AddSingleton<ChooseNodeDeserialiser>();
        services.AddSingleton<DialogNodeDeserialiser>();
        services.AddSingleton<JumpNodeDeserialiser>();
        services.AddSingleton<MetadataNodeDeserialiser>();
        services.AddSingleton<TransitionNodeDeserialiser>();
        services.AddSingleton<IfNodeDeserialiser>();
        services.AddSingleton<WhileNodeDeserialiser>();
    }
}
