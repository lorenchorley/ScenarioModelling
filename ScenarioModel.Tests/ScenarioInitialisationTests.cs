using FluentAssertions;
using ScenarioModel.Tests.Valid;
using ScenarioModel.Validation;

namespace ScenarioModel.Tests
{
    [TestClass]
    public class ScenarioInitialisationTests
    {
        [TestMethod]
        [TestCategory("Valid")]
        public void Scenario1_Validates()
        {
            // Arrange
            // =======
            var scenario = ValidScenario1.Generate();


            // Act
            // ===
            scenario.Initialise();
            new ScenarioValidator().Validate(scenario);


            // Assert
            // ======
            scenario.System.States.Should().NotBeEmpty();

        }
    }
}