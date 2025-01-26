using LanguageExt.Common;

namespace ScenarioModelling.Serialisation.Export.PlantUML.SequenceDiagram;

public class PlantUMLSequenceDiagramExporter : ISerialiser
{
    public Result<Context> DeserialiseContext(string text)
    {
        throw new NotImplementedException();
    }

    public Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context)
    {
        throw new NotImplementedException();
    }

    public Result<string> SerialiseContext(Context context)
    {
        throw new NotImplementedException();
    }
}