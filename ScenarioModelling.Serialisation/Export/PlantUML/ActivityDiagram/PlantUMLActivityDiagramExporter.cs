using LanguageExt.Common;
using ScenarioModelling.CoreObjects;

namespace ScenarioModelling.Serialisation.Export.PlantUML.ActivityDiagram;

public class PlantUMLActivityDiagramExporter : IContextSerialiser
{
    private Dictionary<string, string> _configuration;

    public void SetConfigurationOptions(Dictionary<string, string> configuration)
    {
        _configuration = configuration;
    }

    public Result<Context> DeserialiseContext(string text)
    {
        throw new NotImplementedException();
    }

    //public Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context)
    //{
    //    throw new NotImplementedException();
    //}

    public Result<string> SerialiseContext(Context context)
    {
        throw new NotImplementedException();
    }
}