using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.Objects.Visitors;
using ScenarioModel.References;
using System.Text.Json.Serialization;

namespace ScenarioModel.Objects.SystemObjects;

//[ObjectLike<ISystemObject, AspectType>]
public record AspectType : ISystemObject<AspectTypeReference>, IOptionalSerialisability
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(AspectType);
    public StateMachineProperty StateMachine { get; private init; }

    public bool ExistanceOriginallyInferred { get; set; } = false;
    public bool ShouldReserialise
    {
        get
        {
            return !ExistanceOriginallyInferred;
        }
    }

    public AspectType(System system)
    {
        _system = system;

        // Add this to the system
        //system.AspectTypes.Add(this);

        StateMachine = new(system);
    }

    public AspectTypeReference GenerateReference()
        => new AspectTypeReference(_system) { Name = Name };

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitAspectType(this);

}
