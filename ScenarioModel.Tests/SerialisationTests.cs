using FluentAssertions;
using LanguageExt.Common;
using ScenarioModel.Serialisation;
using ScenarioModel.Serialisation.HumanReadable;
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
                       .LoadContext<HumanReadablePromptSerialiserV1>(serialisedSystem)
                       .Initialise();


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
        //TestSerialiseDeserialiseSystemsForSerialiser<YamlSerialiserV1>(systems);
    }

    private static void TestSerialiseDeserialiseSystemsForSerialiser<T>(List<System> systems) where T : ISerialiser, new()
    {
        foreach (System originalSystem in systems)
        {
            // Act
            // ===
            Result<string> serialisedSystem =
                Context.New()
                       .UseSerialiser<T>()
                       .LoadSystem(originalSystem)
                       .Initialise()
                       .Serialise<T>();

            serialisedSystem.IfFail(ex => Assert.Fail(ex.Message));

            serialisedSystem.IfSucc(newContext =>
            {
                var finalContext =
                    Context.New()
                       .UseSerialiser<T>()
                       .LoadContext<T>(newContext)
                       .Initialise();

                // Assert
                // ======
                originalSystem.Should().BeEquivalentTo(finalContext.System);
            });
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
                       //.UseSerialiser<YamlSerialiserV1>()
                       .LoadContext<HumanReadablePromptSerialiserV1>(serialisedScenario)
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

        TestSerialiseDeserialiseScenariosForSerialiser<HumanReadablePromptSerialiserV1>(scenarios);
        //TestSerialiseDeserialiseScenariosForSerialiser<YamlSerialiserV1>(scenarios);
    }

    private static void TestSerialiseDeserialiseScenariosForSerialiser<T>(List<Scenario> scenarios) where T : ISerialiser, new()
    {
        foreach (Scenario originalScenario in scenarios)
        {
            // Act
            // ===
            Result<string> serialisedScenario =
                Context.New()
                       .UseSerialiser<T>()
                       .LoadScenario(originalScenario)
                       .Initialise()
                       .Serialise<T>();

            serialisedScenario.IfFail(ex => Assert.Fail(ex.Message));

            serialisedScenario.IfSucc(newContext =>
            {
                Context reloadedContext =
                    Context.New()
                       .UseSerialiser<T>()
                       .LoadContext<T>(newContext)
                       .Initialise();

                // Assert
                // ======
                reloadedContext.Scenarios.Should().HaveCount(1);
                originalScenario.Should().BeEquivalentTo(reloadedContext.Scenarios.First());
            });
        }
    }

    [TestMethod]
    [TestCategory("Serialisation")]
    public void Deserialise_Serialise_AllContexts()
    {
        // Arrange
        // =======
        List<string> contexts =
        [
            ValidScenario1.SerialisedContext
        ];

        TestDeserialiseSerialiseContextForSerialiser<HumanReadablePromptSerialiserV1>(contexts);
        //TestSerialiseDeserialiseSystemsForSerialiser<YamlSerialiserV1>(systems);
    }

    private static void TestDeserialiseSerialiseContextForSerialiser<T>(List<string> contexts) where T : ISerialiser, new()
    {
        foreach (string context in contexts)
        {
            // Act
            // ===
            Context loadedContext =
                Context.New()
                       .UseSerialiser<T>()
                       .LoadContext<T>(context)
                       .Initialise();

            Result<string> reserialisedContext = loadedContext.Serialise<T>();

            reserialisedContext.IfFail(ex => Assert.Fail(ex.Message));

            reserialisedContext.IfSucc(newContext =>
            {
                Context reloadedContext = Context.New()
                       .UseSerialiser<T>()
                       .LoadContext<T>(newContext)
                       .Initialise();

                // Assert
                // ======
                loadedContext.Should().BeEquivalentTo(reloadedContext);
            });
        }
    }

}
