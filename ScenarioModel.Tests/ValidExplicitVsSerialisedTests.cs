using FluentAssertions;
using ScenarioModel.Serialisation.HumanReadable;
using ScenarioModel.Tests.Valid;

namespace ScenarioModel.Tests;

[TestClass]
public class ValidExplicitVsSerialisedTests
{
    [TestMethod]
    [TestCategory("Valid"), TestCategory("Serialisation")]
    public void Scenario1_ExplicitAndSerialised_ValidatesRunsEquals()
    {
        // Arrange
        // =======
        var explicitContext =
            Context.New()
                   .UseSerialiser<HumanReadablePromptSerialiserV1>()
                   .LoadSystem(ValidScenario1.System, out System system)
                   .LoadScenario(ValidScenario1.Scenario, out Scenario scenario)
                   .Initialise();

        explicitContext.ValidationErrors.Should().BeEmpty();

        var fromSerialisedContext =
            Context.New()
                   .UseSerialiser<HumanReadablePromptSerialiserV1>()
                   .LoadContext<HumanReadablePromptSerialiserV1>(ValidScenario1.SerialisedContext)
                   .Initialise();

        fromSerialisedContext.ValidationErrors.Should().BeEmpty();


        // Act
        // ===
        StoryRunResult resultFromExplicit = 
            explicitContext.Scenarios
                           .Where(s => s.Name == nameof(ValidScenario1))
                           .First()
                           .StartAtStep("S1");
        
        StoryRunResult resultFromSerialised =
            fromSerialisedContext.Scenarios
                           .Where(s => s.Name == nameof(ValidScenario1))
                           .First()
                           .StartAtStep("S1");


        // Assert
        // ======

        var explicitContextSerialised = explicitContext.Serialise<HumanReadablePromptSerialiserV1>();
        var fromSerialisedContextReserialised = fromSerialisedContext.Serialise<HumanReadablePromptSerialiserV1>();
        explicitContextSerialised.Should().Be(fromSerialisedContextReserialised);

        // Check final state of system
        resultFromExplicit.Should().BeOfType<Successful>().Which.Story.Events.Should().HaveCount(1);
        fromSerialisedContext.Should().BeOfType<Successful>().Which.Story.Events.Should().HaveCount(1);
    }
}