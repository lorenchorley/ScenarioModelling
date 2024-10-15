using FluentAssertions;
using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.Tests.Expressions;

[TestClass]
public class ExpressionGrammarTests
{
    private readonly System _system = new System()
    {
        Entities = new List<Entity>()
        {
            new Entity()
            {
                Name = "E1",
                State = new State()
                {
                    Name = "S1"
                }
            }
        },
    };

    [DataTestMethod]
    [TestCategory("Interpolation")]
    public void ExpressionGrammarTests_Succeeds(string uninterpolated, string expectedInterpolated)
    {
        // Arrange 
        // =======


        // Act
        // ===


        // Assert
        // ======

    }
}