using FluentAssertions;
using ScenarioModelling.Interpolation;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;

namespace ScenarioModelling.Tests;

[TestClass]
public class StringInterpolationTests
{
    private string _scenarioText = """
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

        Context context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .LoadContext(_scenarioText)
                   .Initialise();

        StringInterpolator stringInterpolator = new StringInterpolator(context.System);


        // Act
        // ===
        string interpolated = stringInterpolator.ReplaceInterpolations(uninterpolated);


        // Assert
        // ======
        interpolated.Should().Be(expectedInterpolated);

    }
}