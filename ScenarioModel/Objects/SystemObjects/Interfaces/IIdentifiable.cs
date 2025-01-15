using Newtonsoft.Json;

namespace ScenarioModelling.Objects.SystemObjects.Interfaces;

public interface IIdentifiable
{
    string Name { get; set; }

    [JsonIgnore]
    Type Type { get; }

}
