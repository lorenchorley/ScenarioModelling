using FluentAssertions;
using ScenarioModel.Tests.Valid;
using ScenarioModel.Validation;

namespace ScenarioModel.Tests
{
    [TestClass]
    public class ScenarioTests
    {
        [TestMethod]
        [TestCategory("Valid")]
        public void Scenario1_Validates_Runs()
        {
            // Arrange
            // =======
            var scenario = ValidScenario1.Generate();
            scenario.Initialise();
            var validationErrors = new ScenarioValidator().Validate(scenario);


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
}