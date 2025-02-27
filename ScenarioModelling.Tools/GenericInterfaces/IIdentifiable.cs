using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScenarioModelling.Tools.GenericInterfaces;

public interface IIdentifiable : INamed
{
    [JsonIgnore]
    Type Type { get; }
}
