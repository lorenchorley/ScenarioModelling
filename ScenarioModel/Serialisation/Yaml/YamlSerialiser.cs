using LanguageExt;
using LanguageExt.Common;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.SystemObjects.Properties;
using ScenarioModelling.References;
using ScenarioModelling.References.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeTypeResolvers;

namespace ScenarioModelling.Serialisation;

public class YamlSerialiser : ISerialiser
{
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public YamlSerialiser()
    {
        _serializer = 
            new SerializerBuilder()
                .EnsureRoundtrip()
                .IgnoreFields()
                .WithTypeConverter(new YamlPropertyConverter<EntityTypeProperty, EntityType, EntityTypeReference>())
                .WithTypeConverter(new YamlPropertyConverter<StateProperty, State, StateReference>())
                .WithTypeConverter(new YamlPropertyConverter<StateMachineProperty, StateMachine, StateMachineReference>())
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitEmptyCollections | DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)
                .EnablePrivateConstructors()
                .Build();

        _deserializer =
            new DeserializerBuilder() 
                .WithCaseInsensitivePropertyMatching()
                .EnablePrivateConstructors()
                .Build();
    }

    public Result<Context> DeserialiseContext(string text)
    {
        // TODO errors
        return _deserializer.Deserialize<Context>(text);
    }

    public Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context)
    {
        var newContext = DeserialiseContext(text);

        return newContext.Match(Succ: c => context.Incorporate(c), Fail: e => new Result<Context>(e));
    }

    public Result<string> SerialiseContext(Context context)
    {
        // TODO errors
        return _serializer.Serialize(context);
    }
}


public class YamlPropertyConverter<TProp, TVal, TRef> : IYamlTypeConverter
    where TProp : OptionalReferencableProperty<TVal, TRef>
    where TVal : class, ISystemObject<TRef>
    where TRef : class, IReference<TVal>
{
    public bool Accepts(Type type)
    {
        return type == typeof(TProp);
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        return rootDeserializer.Invoke(type);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        TProp prop = (TProp)value!;

        TRef? reference = prop.Match<TRef?>(v => v.GenerateReference(), r => r, () => null);

        if (reference != null) 
        {
            serializer.Invoke(reference);
        }
        else
        {
            serializer.Invoke(null);
        }
    }
}