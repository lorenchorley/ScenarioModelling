using LanguageExt.Common;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ScenarioModelling.Serialisation.Yaml;

public class YamlSerialiser : IContextSerialiser
{
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;
    private Dictionary<string, string> _configuration;

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

    public void SetConfigurationOptions(Dictionary<string, string> configuration)
    {
        _configuration = configuration;
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

        TRef? reference = prop.Match(v => v.GenerateReference(), r => r, () => null);

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