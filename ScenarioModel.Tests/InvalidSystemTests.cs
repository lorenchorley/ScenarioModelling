using FluentAssertions;
using ScenarioModel.Serialisation.HumanReadable;
using ScenarioModel.Tests.Valid;

namespace ScenarioModel.Tests;

[TestClass]
public class InvalidSystemTests
{
    [TestMethod]
    [TestCategory("Invalid"), TestCategory("System")]
    public void Scenario1_Invalid_DoesNotValidate()
    {
        // Arrange && Act
        // ==============
        var context =
            Context.New()
                   .UseSerialiser<HumanReadablePromptSerialiserV1>()
                   .LoadSystem(InvalidSystem1.System, out System system)
                   .Initialise();


        // Assert
        // ======
        context.ValidationErrors.Should().HaveCount(1);
    }
}