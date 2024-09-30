using FluentAssertions;
using ScenarioModel.Serialisation;
using ScenarioModel.Tests.Valid;

namespace ScenarioModel.Tests;

[TestClass]
public class SerialisationTests
{
    [TestMethod]
    [TestCategory("Serialisation")]
    public void Deserialise_Serialise_AllSystems()
    {
        // Arrange
        // =======
        List<string> serialisedSystems =
        [
            //ValidScenario1.System,
            //ValidScenario2.System,
            //InvalidSystem1.System
        ];

        foreach (string serialisedSystem in serialisedSystems)
        {
            // Act
            // ===
            var context =
                Context.New()
                       .UseSerialiser<HumanReadablePromptSerialiserV1>()
                       .UseSerialiser<YamlSerialiserV1>()
                       .LoadSystem(serialisedSystem)
                       .Initialise();

            context.Systems.Should().HaveCount(1);
            var system = context.Systems.First();

            // Assert
            // ======

        }
    }
    
    [TestMethod]
    [TestCategory("Serialisation")]
    public void Serialise_Deserialise_AllSystems()
    {
        // Arrange
        // =======
        List<System> systems = 
        [
            ValidScenario1.System,
            ValidScenario2.System,
            InvalidSystem1.System
        ];

        TestSerialiseDeserialiseSystemsForSerialiser<HumanReadablePromptSerialiserV1>(systems);
        TestSerialiseDeserialiseSystemsForSerialiser<YamlSerialiserV1>(systems);
    }

    private static void TestSerialiseDeserialiseSystemsForSerialiser<T>(List<System> systems) where T : ISerialiser, new()
    {
        foreach (System originalSystem in systems)
        {
            // Act
            // ===
            string serialisedSystem =
                Context.New()
                       .UseSerialiser<T>()
                       .LoadSystem(originalSystem)
                       .Initialise()
                       .Serialise<T>();

            Context.New()
                   .UseSerialiser<T>()
                   .LoadSystem(serialisedSystem, out System finalSystem)
                   .Initialise();

            // Assert
            // ======
            originalSystem.Should().BeEquivalentTo(finalSystem);
        }
    }

    [TestMethod]
    [TestCategory("Serialisation")]
    public void Deserialise_Serialise_AllScenarios()
    {
        // Arrange
        // =======
        List<string> serialisedScenarios =
        [
            //ValidScenario1.System,
            //ValidScenario2.System,
            //InvalidSystem1.System
        ];

        foreach (string serialisedScenario in serialisedScenarios)
        {
            // Act
            // ===
            var context =
                Context.New()
                       .UseSerialiser<HumanReadablePromptSerialiserV1>()
                       .UseSerialiser<YamlSerialiserV1>()
                       .LoadScenario(serialisedScenario)
                       .Initialise();

            context.Scenarios.Should().HaveCount(1);
            var system = context.Scenarios.First();

            // Assert
            // ======

        }
    }

    [TestMethod]
    [TestCategory("Serialisation")]
    public void Serialise_Deserialise_AllScenarios()
    {
        // Arrange
        // =======
        List<Scenario> scenarios = 
        [
            ValidScenario1.Scenario,
            ValidScenario2.Scenario
        ];

        TestSerialiseDeserialiseScenariosForSerialiser<YamlSerialiserV1>(scenarios);
    }

    private static void TestSerialiseDeserialiseScenariosForSerialiser<T>(List<Scenario> scenarios) where T : ISerialiser, new()
    {
        foreach (Scenario originalScenario in scenarios)
        {
            // Act
            // ===
            string serialisedScenario =
                Context.New()
                       .UseSerialiser<T>()
                       .LoadScenario(originalScenario)
                       .Initialise()
                       .Serialise<T>();

            Context.New()
                   .UseSerialiser<T>()
                   .LoadScenario(serialisedScenario, out Scenario finalScenario)
                   .Initialise();

            // Assert
            // ======
            originalScenario.Should().BeEquivalentTo(finalScenario);
        }
    }
}