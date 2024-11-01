//using ScenarioModel.Expressions.SemanticTree;
//using ScenarioModel.Objects.System.Relations;
//using ScenarioModel.References;

//namespace ScenarioModel.Tests.Valid;

//public static class InvalidSystem1
//{
//    public static System System
//    {
//        get => new()
//        {
//            Entities = [
//                new() { Name = "E1", Relations = [ new Relation() { LeftEntity = new EntityReference() { EntityName = "E1" }, RightEntity = new EntityReference() { EntityName = "E2" } } ] },
//                new() { Name = "E2" }
//            ],
//            Constraints =
//            [
//                new HasRelationExpression() { Name = "R1", Left = new ValueComposite() { ValueList = ["E1"] }, Right = new ValueComposite() { ValueList = ["E2"] } } // R1 does not exist on an entity
//            ]
//        };
//    }
//}