using FluentAssertions;
using ScenarioModel.Serialisation.HumanReadable;
using ScenarioModel.Tests.Valid;

namespace ScenarioModel.Tests;

[TestClass]
public class ScenarioTests
{
    [TestMethod]
    [TestCategory("Valid"), TestCategory("Scenario")]
    public void Scenario1_Validates_Runs()
    {
        // Arrange
        // =======
        var context =
            Context.New()
                   .UseSerialiser<HumanReadablePromptSerialiserV1>()
                   .LoadSystem(ValidScenario1.System, out System system)
                   .LoadScenario(ValidScenario1.Scenario, out Scenario scenario)
                   .Initialise();

        context.ValidationErrors.Should().BeEmpty();


        // Act
        // ===
        StoryRunResult result = scenario.StartAtStep("S1");


        // Assert
        // ======

        // Check final state of system
        result.Should().BeOfType<Successful>();
        Successful successful = (Successful)result;

        successful.Story.Events.Should().HaveCount(1);
    }
}