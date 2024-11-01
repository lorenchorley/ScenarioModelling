using FluentAssertions;
using ScenarioModel.Interpolation;
using ScenarioModel.Objects.SystemObjects.Entities;
using ScenarioModel.Objects.SystemObjects.States;

namespace ScenarioModel.Tests;

[TestClass]
public class StringInterpolationTests
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
    [DataRow("No interpolation necessary", "No interpolation necessary")]
    [DataRow("The state of E1 is {E1.State}", "The state of E1 is S1")]
    public void InterpolationTests_Succeeds(string uninterpolated, string expectedInterpolated)
    {
        // Arrange 
        // =======
        StringInterpolator stringInterpolator = new StringInterpolator(_system);


        // Act
        // ===
        string interpolated = stringInterpolator.ReplaceInterpolations(uninterpolated);


        // Assert
        // ======
        interpolated.Should().Be(expectedInterpolated);

    }
}