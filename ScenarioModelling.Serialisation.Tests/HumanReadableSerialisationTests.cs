using FluentAssertions;
using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions.MetaStateObjects;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;
using ScenarioModelling.TestDataAndTools;
using ScenarioModelling.TestDataAndTools.CodeHooks;
using ScenarioModelling.TestDataAndTools.Serialisation;
using System.Diagnostics;

namespace ScenarioModelling.Serialisation.Tests;

[TestClass]
public class CustomSerialiserSerialisationTests
{
    private const string MetaStoryName = "MetaStory recorded by hooks";

    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("CustomSerialiser")]
    [ReserialisationDataProvider]
    public void CustomSerialiser_Context_DeserialiseReserialise(string testCaseName, string originalContextText, string expectedFinalContextText)
        => CustomSerialiser_Context_DeserialiseReserialise_Common(testCaseName, originalContextText, expectedFinalContextText);

    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("CustomSerialiser")]
    [ProgressiveCodeHookTestDataProvider]
    public void CustomSerialiser_Context_DeserialiseReserialise_FromHookTestData(string metaStoryMethodName, string systemMethodName, bool testDefinedFirstMetaStory, int loopCount)
    {
        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();

        var context =
            scope.Context
                 .UseSerialiser<CustomContextSerialiser>()
                 .Initialise();

        HookOrchestrator orchestrator = scope.GetService<MetaStoryHookOrchestratorForConstruction>();

        var systemHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(systemMethodName);
        var metaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<HookOrchestrator>(metaStoryMethodName);

        // Build system
        orchestrator.DefineMetaState(systemHooksMethod);

        // Build MetaStory
        if (!testDefinedFirstMetaStory)
            orchestrator.StartMetaStory(MetaStoryName);

        metaStoryHooksMethod(orchestrator);

        if (!testDefinedFirstMetaStory)
            orchestrator.EndMetaStory();


        var serialisedContext =
            context.ResetToInitialState()
                   .Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        var expectedSystemText = ProgressiveCodeHookTestDataProviderAttribute.GetExpectedText(systemMethodName);
        var expectedMetaStoryText = ProgressiveCodeHookTestDataProviderAttribute.GetExpectedText(metaStoryMethodName);
        string expectedText = BuildExpectedText(expectedSystemText, expectedMetaStoryText, testDefinedFirstMetaStory);

        CustomSerialiser_Context_DeserialiseReserialise_Common(metaStoryMethodName, serialisedContext, expectedText);
    }

    public void CustomSerialiser_Context_DeserialiseReserialise_Common(string testCaseName, string originalContextText, string expectedFinalContextText)
    {
        // Arrange
        // =======

        // Act
        // ===
        Debug.WriteLine("Starting Serialised Context");
        Debug.WriteLine("===========================");
        Debug.WriteLine("");
        Debug.WriteLine(originalContextText);

        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();

        Context loadedContext =
            scope.Context
                 .UseSerialiser<CustomContextSerialiser>()
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

                // Redo the same operation but with compression to compare lengths
                loadedContext.RemoveSerialiser<CustomContextSerialiser>()
                             .UseSerialiser<CustomContextSerialiser>(new() { { "Compress", "true" } })
                             .Serialise<CustomContextSerialiser>()
                             .Switch(compressedContextText => 
                             { 
                                 Debug.WriteLine($"Original serialisation length : {originalContextText.Length}");
                                 Debug.WriteLine($"Reserialised length : {reserialisedContextText.Length}");
                                 Debug.WriteLine($"Compressed reserialised length : {compressedContextText.Length}");
                             }, ex => Assert.Fail(ex.Message));

                // Reload the context using the same serialiser so that w're more sure that it's complete
                using ScenarioModellingContainer reloadingContainer = new();
                using var reloadingScope = reloadingContainer.StartScope();

                Context reloadedContext =
                    reloadingScope.Context
                                  .UseSerialiser<CustomContextSerialiser>()
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

    private static string BuildExpectedText(string expectedSystemText, string expectedMetaStoryText, bool testDefinedFirstMetaStory)
    {
        if (testDefinedFirstMetaStory)
        {
            return $$"""
                {{expectedSystemText.Trim()}}

                {{expectedMetaStoryText.Trim()}}
                """;
        }
        else
        {
            return $$"""
                {{expectedSystemText.Trim()}}

                MetaStory "{{MetaStoryName}}" {
                {{expectedMetaStoryText.Trim().AddIndent("  ")}}
                }
                """;
        }
    }
}
