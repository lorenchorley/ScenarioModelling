using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Expressions.Initialisation;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Serialisation.Expressions;
using ScenarioModelling.Serialisation.Expressions.Interpreter;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers;
using ScenarioModelling.Serialisation.ProtoBuf;
using ScenarioModelling.Serialisation.Yaml;

namespace ScenarioModelling.Serialisation.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IContextSerialiser, CustomContextSerialiser>();
        services.AddScoped<CustomContextSerialiser>();
        services.AddScoped<IContextSerialiser, YamlSerialiser>();
        services.AddScoped<YamlSerialiser>();
        services.AddScoped<IContextSerialiser, ProtoBufSerialiser>();
        services.AddScoped<ProtoBufSerialiser>();
        services.AddScoped<IContextSerialiser, ProtoBufSerialiser_Uncompressed>();
        services.AddScoped<ProtoBufSerialiser_Uncompressed>();

        services.AddScoped<Instanciator>();
        services.AddScoped<CustomContextDeserialiser>();
        services.AddScoped<MetaStorySerialiser>();
        services.AddScoped<MetaStoryTransformer>();
        services.AddScoped<MetaStateSerialiser>();
        services.AddScoped<ExpressionInterpreter>();
        services.AddScoped<ExpressionInitialiser>();
        services.AddScoped<ExpressionSerialiser>();

        MetaStoryNodeExhaustivity.DoForEachNodeType(
            assert: () => services.AddScoped<AssertNodeSerialiser>(),
            callMetaStory: () => services.AddScoped<CallMetaStoryNodeSerialiser>(),
            chooseNode: () => services.AddScoped<ChooseNodeSerialiser>(),
            dialogNode: () => services.AddScoped<DialogNodeSerialiser>(),
            ifNode: () => services.AddScoped<IfNodeSerialiser>(),
            jumpNode: () => services.AddScoped<JumpNodeSerialiser>(),
            loopNode: () => services.AddScoped<LoopNodeSerialiser>(),
            metadataNode: () => services.AddScoped<MetadataNodeSerialiser>(),
            transitionNode: () => services.AddScoped<TransitionNodeSerialiser>(),
            whileNode: () => services.AddScoped<WhileNodeSerialiser>()
        );

        MetaStateObjectExhaustivity.DoForEachObjectType(
            entity: () => services.AddScoped<AspectSerialiser>(),
            entityType: () => services.AddScoped<ConstraintSerialiser>(),
            aspect: () => services.AddScoped<EntitySerialiser>(),
            relation: () => services.AddScoped<EntityTypeSerialiser>(),
            state: () => services.AddScoped<RelationSerialiser>(),
            stateMachine: () => services.AddScoped<StateSerialiser>(),
            transition: () => services.AddScoped<StateMachineSerialiser>(),
            constraint: () => services.AddScoped<TransitionSerialiser>()
        );

        MetaStoryNodeExhaustivity.DoForEachNodeType(
            assert: () => services.AddScoped<AssertNodeDeserialiser>(),
            callMetaStory: () => services.AddScoped<CallMetaStoryNodeDeserialiser>(),
            chooseNode: () => services.AddScoped<ChooseNodeDeserialiser>(),
            dialogNode: () => services.AddScoped<DialogNodeDeserialiser>(),
            ifNode: () => services.AddScoped<IfNodeDeserialiser>(),
            jumpNode: () => services.AddScoped<JumpNodeDeserialiser>(),
            loopNode: () => services.AddScoped<LoopNodeDeserialiser>(),
            metadataNode: () => services.AddScoped<MetadataNodeDeserialiser>(),
            transitionNode: () => services.AddScoped<TransitionNodeDeserialiser>(),
            whileNode: () => services.AddScoped<WhileNodeDeserialiser>()
        );

        MetaStateObjectExhaustivity.DoForEachObjectType(
            entity: () => services.AddScoped<AspectDeserialiser>(),
            entityType: () => services.AddScoped<ConstraintDeserialiser>(),
            aspect: () => services.AddScoped<EntityDeserialiser>(),
            relation: () => services.AddScoped<EntityTypeDeserialiser>(),
            state: () => services.AddScoped<RelationDeserialiser>(),
            stateMachine: () => services.AddScoped<StateDeserialiser>(),
            transition: () => services.AddScoped<StateMachineDeserialiser>(),
            constraint: () => services.AddScoped<TransitionDeserialiser>()
        );

    }
}
