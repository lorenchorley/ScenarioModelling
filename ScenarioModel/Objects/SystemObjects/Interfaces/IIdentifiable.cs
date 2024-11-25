using System.Text.Json.Serialization;

namespace ScenarioModel.Objects.SystemObjects.Interfaces;

public interface IIdentifiable
{
    string Name { get; set; }

    [JsonIgnore]
    Type Type { get; }

}
