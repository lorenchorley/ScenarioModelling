using FluentAssertions;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Interpolation;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;

namespace ScenarioModelling.Tools.Tests;

[TestClass]
public class StringInterpolationTests
{
    private string _metaStoryText = """
        Entity E1 {
            State S1
        }
        """;

    [DataTestMethod]
    [TestCategory("Interpolation")]
    [DataRow("No interpolation necessary", "No interpolation necessary")]
    [DataRow("The state of E1 is {E1.State}", "The state of E1 is S1")]
    public void InterpolationTests_Succeeds(string uninterpolated, string expectedInterpolated)
    {
        // Arrange 
        // =======
        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();

        Context context =
            scope.Context
                 .UseSerialiser<CustomContextSerialiser>()
                 .LoadContext(_metaStoryText)
                 .Initialise();

        StringInterpolator stringInterpolator = scope.GetService<StringInterpolator>();


        // Act
        // ===
        string interpolated = stringInterpolator.ReplaceInterpolations(uninterpolated);


        // Assert
        // ======
        interpolated.Should().Be(expectedInterpolated);

    }
}