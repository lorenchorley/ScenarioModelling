using Newtonsoft.Json;

namespace ScenarioModelling.Tools.GenericInterfaces;

public interface IIdentifiable : INamed
{
    [JsonIgnore]
    Type Type { get; }

}
