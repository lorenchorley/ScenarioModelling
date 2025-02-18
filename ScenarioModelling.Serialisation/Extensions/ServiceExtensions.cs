using Microsoft.Extensions.DependencyInjection;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Expressions.Initialisation;
using ScenarioModelling.Exhaustiveness;
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

        MetaStoryNodeExhaustivity.DoForEachNodeType(
            callMetaStory: () => services.AddSingleton<CallMetaStoryNodeSerialiser>(),
            chooseNode: () => services.AddSingleton<ChooseNodeSerialiser>(),
            dialogNode: () => services.AddSingleton<DialogNodeSerialiser>(),
            ifNode: () => services.AddSingleton<IfNodeSerialiser>(),
            jumpNode: () => services.AddSingleton<JumpNodeSerialiser>(),
            metadataNode: () => services.AddSingleton<MetadataNodeSerialiser>(),
            transitionNode: () => services.AddSingleton<TransitionNodeSerialiser>(),
            whileNode: () => services.AddSingleton<WhileNodeSerialiser>()
        );

        SystemObjectExhaustivity.DoForEachObjectType(
            entity: () => services.AddSingleton<AspectSerialiser>(),
            entityType: () => services.AddSingleton<ConstraintSerialiser>(),
            aspect: () => services.AddSingleton<EntitySerialiser>(),
            relation: () => services.AddSingleton<EntityTypeSerialiser>(),
            state: () => services.AddSingleton<RelationSerialiser>(),
            stateMachine: () => services.AddSingleton<StateSerialiser>(),
            transition: () => services.AddSingleton<StateMachineSerialiser>(),
            constraint: () => services.AddSingleton<TransitionSerialiser>()
        );

        MetaStoryNodeExhaustivity.DoForEachNodeType(
            callMetaStory: () => services.AddSingleton<CallMetaStoryNodeDeserialiser>(),
            chooseNode: () => services.AddSingleton<ChooseNodeDeserialiser>(),
            dialogNode: () => services.AddSingleton<DialogNodeDeserialiser>(),
            ifNode: () => services.AddSingleton<IfNodeDeserialiser>(),
            jumpNode: () => services.AddSingleton<JumpNodeDeserialiser>(),
            metadataNode: () => services.AddSingleton<MetadataNodeDeserialiser>(),
            transitionNode: () => services.AddSingleton<TransitionNodeDeserialiser>(),
            whileNode: () => services.AddSingleton<WhileNodeDeserialiser>()
        );

        SystemObjectExhaustivity.DoForEachObjectType(
            entity: () => services.AddSingleton<AspectDeserialiser>(),
            entityType: () => services.AddSingleton<ConstraintDeserialiser>(),
            aspect: () => services.AddSingleton<EntityDeserialiser>(),
            relation: () => services.AddSingleton<EntityTypeDeserialiser>(),
            state: () => services.AddSingleton<RelationDeserialiser>(),
            stateMachine: () => services.AddSingleton<StateDeserialiser>(),
            transition: () => services.AddSingleton<StateMachineDeserialiser>(),
            constraint: () => services.AddSingleton<TransitionDeserialiser>()
        );

    }
}
