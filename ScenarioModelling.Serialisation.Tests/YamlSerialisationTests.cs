using FluentAssertions;
using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;
using ScenarioModelling.Serialisation.Yaml;
using ScenarioModelling.TestDataAndTools;
using ScenarioModelling.TestDataAndTools.CodeHooks;
using ScenarioModelling.TestDataAndTools.Serialisation;
using System.Diagnostics;

namespace ScenarioModelling.Serialisation.Tests;

[TestClass]
public class YamlSerialisationTests
{
    private const string MetaStoryName = "MetaStory recorded by hooks";

    [Ignore("I don't see an advantage to YAML over the custom serialisation")]
    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("Yaml")]
    [ReserialisationDataProvider]
    public void Yaml_Context_DeserialiseReserialise(string testCaseName, string originalContextText, string expectedFinalContextText)
        => Yaml_Context_SerialiseDeserialise_Common(testCaseName, originalContextText, expectedFinalContextText);

    [Ignore("I don't see an advantage to YAML over the custom serialisation")]
    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("Yaml")]
    [ProgressiveCodeHookTestDataProvider]
    public void Yaml_Context_DeserialiseReserialise_FromHookTestData(string metaStoryMethodName, string systemMethodName, bool autoDefineMetaStory)
    {
        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();

        var context =
            scope.Context
                 .UseSerialiser<CustomContextSerialiser>()
                 .Initialise();

        MetaStoryHookOrchestrator orchestrator = scope.GetService<MetaStoryHookOrchestratorForConstruction>();

        var systemHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(systemMethodName);
        var metaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(metaStoryMethodName);

        // Build system
        orchestrator.DefineMetaState(systemHooksMethod);

        // Build MetaStory
        orchestrator.StartMetaStory(MetaStoryName);
        metaStoryHooksMethod(orchestrator);
        orchestrator.EndMetaStory();

        var serialisedContext =
            context.Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        Yaml_Context_SerialiseDeserialise_Common(metaStoryMethodName, serialisedContext, "");
    }

    public void Yaml_Context_SerialiseDeserialise_Common(string testCaseName, string originalContextText, string expectedFinalContextText)
    {
        // Arrange
        // =======

        // Act
        // ===
        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();

        Context originalContext =
            scope.Context
                 .UseSerialiser<CustomSerialiser.Reserialisation.CustomContextSerialiser>()
                 .UseSerialiser<YamlSerialiser>()
                 .LoadContext<CustomSerialiser.Reserialisation.CustomContextSerialiser>(originalContextText)
                 .Initialise();

        originalContext.ValidationErrors.Count.Should().Be(0, because: originalContext.ValidationErrors.ToString());

        originalContext.Serialise<YamlSerialiser>()
                       .Switch(
            serialisedContextText =>
            {
                Debug.WriteLine("");
                Debug.WriteLine("serialised context");
                Debug.WriteLine("==================");
                Debug.WriteLine("");
                Debug.WriteLine(serialisedContextText);

                using ScenarioModellingContainer reloadingContainer = new();
                using var reloadingScope = reloadingContainer.StartScope();


                Context reloadedContext =
                    reloadingScope.Context
                                  .UseSerialiser<YamlSerialiser>()
                                  .LoadContext(serialisedContextText)
                                  .Initialise();

                string reserialisedContext =
                    reloadedContext.Serialise<YamlSerialiser>()
                                   .Match(v => v, e => throw e);

                // Assert
                // ======

                //reloadedContext.IsEqv(originalContext).Should().Be(true);

                reloadedContext.ValidationErrors.Count.Should().Be(0, $"because {string.Join('\n', reloadedContext.ValidationErrors)}");

                DiffAssert.DiffIfNotEqual(serialisedContextText.Trim(), reserialisedContext.Trim(), leftName: $@"Expected_{testCaseName.Replace(' ', '_')}", rightName: $@"Result_{testCaseName.Replace(' ', '_')}");
            },
            ex => Assert.Fail(ex.Message)
        );
    }
}
