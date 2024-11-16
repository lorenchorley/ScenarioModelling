using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects.Properties;

namespace ScenarioModel.Objects.SystemObjects;

public record AspectType : ISystemObject, IOptionalSerialisability
{
    private readonly System _system;

    public string Name { get; set; } = "";
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

    //public AspectTypeReference GenerateReference()
    //    => new AspectTypeReference(System) { Name = Name };

}
