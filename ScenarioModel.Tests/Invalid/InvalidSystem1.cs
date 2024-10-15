using ScenarioModel.References;
using ScenarioModel.SystemObjects.Expressions;
using ScenarioModel.SystemObjects.Relations;

namespace ScenarioModel.Tests.Valid;

public static class InvalidSystem1
{
    public static System System
    {
        get => new()
        {
            Entities = [
                new() { Name = "E1", Relations = [ new Relation() { LeftEntity = new EntityReference() { EntityName = "E1" }, RightEntity = new EntityReference() { EntityName = "E2" } } ] },
                new() { Name = "E2" }
            ],
            Constraints =
            [
                new HasRelationExpression() { Ref = new RelationReference() { RelationName = "R1" }, RelatableObject = new EntityReference() { EntityName = "E1" } } // R1 does not exist on an entity
            ]
        };
    }
}