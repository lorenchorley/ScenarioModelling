﻿using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.Objects.Visitors;
using ScenarioModel.References;
using ScenarioModel.References.GeneralisedReferences;
using ScenarioModel.References.Interfaces;
using System.Text.Json.Serialization;

namespace ScenarioModel.Objects.SystemObjects;

public record Relation : ISystemObject<RelationReference>, IStateful
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
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

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitRelation(this);
}
