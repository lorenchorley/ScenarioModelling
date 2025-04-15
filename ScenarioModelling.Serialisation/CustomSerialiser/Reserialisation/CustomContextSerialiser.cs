using LanguageExt.Common;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.Interpreter;
using ScenarioModelling.Tools.Extensions;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;

public class CustomContextSerialiser : IContextSerialiser
{
    public static Encoding CompressionEncoding = Encoding.UTF8;
    internal const string IndentSegment = "  ";

    private readonly IServiceProvider _serviceProvider;
    private readonly MetaStorySerialiser _metaStorySerialiser;
    private readonly MetaStateSerialiser _systemSerialiser;
    private Dictionary<string, string> _configuration = new();

    private bool UseCompression => _configuration.TryGetValue("Compress", out string useCompression) && useCompression.IsEqv("true");

    public CustomContextSerialiser(IServiceProvider serviceProvider, MetaStorySerialiser metaStorySerialiser, MetaStateSerialiser systemSerialiser)
    {
        _serviceProvider = serviceProvider;
        _metaStorySerialiser = metaStorySerialiser;
        _systemSerialiser = systemSerialiser;
    }

    public void SetConfigurationOptions(Dictionary<string, string> configuration)
    {
        _configuration = configuration;
    }

    //public Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context)
    //{
    //    var newContext = DeserialiseContext(text);

    //    return newContext.Match(Succ: c => c, Fail: e => new Result<Context>(e));
    //}

    public Result<Context> DeserialiseContext(string text)
    {
        text = UseCompression ? Decompress(text) : text;

        CustomSerialiserInterpreter interpreter = new();
        var result = interpreter.Parse(text);

        if (result.HasErrors)
        {
            return new Result<Context>(new Exception(string.Join("\n", result.Errors)));
        }

        ContextBuilderInputs inputs = new();
        inputs.TopLevelOfDefinitionTree.AddRange(result.ParsedObject!);

        CustomContextDeserialiser deserialiser = _serviceProvider.GetRequiredService<CustomContextDeserialiser>();

        return deserialiser.RefreshContextWithInputs(inputs);
    }

    public Result<string> SerialiseContext(Context context)
    {
        StringBuilder sb = new();
        //StrMkr sm = new();

        _systemSerialiser.WriteSystem(sb, context.MetaState, "");

        foreach (var metaStory in context.MetaStories)
        {
            _metaStorySerialiser.WriteMetaStory(sb, metaStory, "");
        }

        string serialisedContext = sb.ToString();

        return UseCompression ? Compress(serialisedContext) : serialisedContext;
    }

    private static string Decompress(string text)
        => CompressionEncoding.GetString(SerialisationFunctions.DecompressFromWeb(text).ToArray());

    private static string Compress(string serialisedContext)
        => SerialisationFunctions.CompressForWeb(new MemoryStream(CompressionEncoding.GetBytes(serialisedContext)));

}
