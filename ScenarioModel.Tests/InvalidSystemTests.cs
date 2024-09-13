using FluentAssertions;
using ScenarioModel.Tests.Valid;
using ScenarioModel.Validation;

namespace ScenarioModel.Tests
{
    [TestClass]
    public class InvalidSystemTests
    {
        [TestMethod]
        [TestCategory("Invalid"), TestCategory("System")]
        public void Scenario1_DoesNotValidate()
        {
            // Arrange
            // =======
            var system = InvalidSystem1.Generate();


            // Act
            // ===
            var validationErrors = new SystemValidator().Validate(system);


            // Assert
            // ======

            validationErrors.Should().HaveCount(1);
        }
    }
}