using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.References;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel.Objects.SystemObjects;

public record Relation : ISystemObject, IStateful
{
    private readonly System _system;

    public string Name { get; set; } = "";
    public Type Type => typeof(Relation);

    public RelatableObjectReference? LeftEntity { get; set; } // TODO Propertyise
    public RelatableObjectReference? RightEntity { get; set; } // TODO Propertyise
    public StateProperty State { get; }

    public Relation(System system)
    {
        _system = system;

        // Add this to the system
        system.Relations.Add(this);

        State = new(system);
    }

    public RelationReference GenerateReference()
        => new RelationReference(_system) { Name = Name };

    public IStatefulObjectReference GenerateStatefulReference()
        => GenerateReference();

}
