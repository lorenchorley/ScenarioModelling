using FluentAssertions;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Interpolation;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;

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
        ScenarioModellingContainer container = new();

        Context context =
            container.Context
                     .UseSerialiser<ContextSerialiser>()
                     .LoadContext(_metaStoryText)
                     .Initialise();

        StringInterpolator stringInterpolator = container.GetService<StringInterpolator>();


        // Act
        // ===
        string interpolated = stringInterpolator.ReplaceInterpolations(uninterpolated);


        // Assert
        // ======
        interpolated.Should().Be(expectedInterpolated);

    }
}