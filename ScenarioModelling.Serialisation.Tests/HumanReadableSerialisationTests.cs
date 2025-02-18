using FluentAssertions;
using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.TestDataAndTools;
using ScenarioModelling.TestDataAndTools.CodeHooks;
using ScenarioModelling.TestDataAndTools.Serialisation;
using System.Diagnostics;

namespace ScenarioModelling.Serialisation.Tests;

[TestClass]
public class HumanReadableSerialisationTests
{
    private const string MetaStoryName = "MetaStory recorded by hooks";

    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("HumanReadable")]
    [ReserialisationDataProvider]
    public void HumanReadable_Context_DeserialiseReserialise(string testCaseName, string originalContextText, string expectedFinalContextText)
        => HumanReadable_Context_DeserialiseReserialise_Common(testCaseName, originalContextText, expectedFinalContextText);

    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("HumanReadable")]
    [ProgressiveCodeHookTestDataProvider]
    public void HumanReadable_Context_DeserialiseReserialise_FromHookTestData(string metaStoryMethodName, string systemMethodName, bool autoDefineMetaStory)
    {
        ScenarioModellingContainer container = new();

        var context =
            container.Context
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestrator orchestrator = container.GetService<MetaStoryHookOrchestratorForConstruction>();

        var systemHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(systemMethodName);
        var metaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(metaStoryMethodName);

        // Build system
        orchestrator.DefineMetaState(systemHooksMethod);

        // Build MetaStory
        orchestrator.StartMetaStory(MetaStoryName);
        metaStoryHooksMethod(orchestrator);
        orchestrator.EndMetaStory();

        var serialisedContext =
            context.ResetToInitialState()
                   .Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        var expectedSystemText = ProgressiveCodeHookTestDataProviderAttribute.GetExpectedText(systemMethodName);
        var expectedMetaStoryText = ProgressiveCodeHookTestDataProviderAttribute.GetExpectedText(metaStoryMethodName);
        string expectedText = BuildExpectedText(expectedSystemText, expectedMetaStoryText, autoDefineMetaStory);

        HumanReadable_Context_DeserialiseReserialise_Common(metaStoryMethodName, serialisedContext, expectedText);
    }

    public void HumanReadable_Context_DeserialiseReserialise_Common(string testCaseName, string originalContextText, string expectedFinalContextText)
    {
        // Arrange
        // =======

        // Act
        // ===
        Debug.WriteLine("Starting Serialised Context");
        Debug.WriteLine("===========================");
        Debug.WriteLine("");
        Debug.WriteLine(originalContextText);

        ScenarioModellingContainer container = new();

        Context loadedContext =
            container.Context
                     .UseSerialiser<ContextSerialiser>()
                     .LoadContext(originalContextText)
                     .Initialise();

        loadedContext.ValidationErrors.Count.Should().Be(0, because: loadedContext.ValidationErrors.ToString());

        loadedContext.Serialise()
                     .Switch(
            reserialisedContextText =>
            {
                Debug.WriteLine("");
                Debug.WriteLine("");
                Debug.WriteLine("Reserialised Context");
                Debug.WriteLine("====================");
                Debug.WriteLine("");
                Debug.WriteLine(reserialisedContextText);

                ScenarioModellingContainer reloadingContainer = new();

                Context reloadedContext =
                    reloadingContainer.Context
                                      .UseSerialiser<ContextSerialiser>()
                                      .LoadContext(reserialisedContextText)
                                      .Initialise();


                // Assert
                // ======
                reloadedContext.ValidationErrors.Count.Should().Be(0, $"because {string.Join('\n', reloadedContext.ValidationErrors)}");

                DiffAssert.DiffIfNotEqual(reserialisedContextText.Trim(), expectedFinalContextText.Trim(), leftName: $@"Result_{testCaseName.Replace(' ', '_')}", rightName: $@"Expected_{testCaseName.Replace(' ', '_')}");
            },
            ex => Assert.Fail(ex.Message)
        );
    }

    private static string BuildExpectedText(string expectedSystemText, string expectedMetaStoryText, bool autoDefineMetaStory)
    {
        if (autoDefineMetaStory)
        {
            return $$"""
                {{expectedSystemText.Trim()}}

                MetaStory "{{MetaStoryName}}" {
                {{expectedMetaStoryText.Trim().AddIndent("  ")}}
                }
                """;
        }
        else
        {
            return $$"""
                {{expectedSystemText.Trim()}}

                {{expectedMetaStoryText.Trim()}}
                """;
        }
    }
}
