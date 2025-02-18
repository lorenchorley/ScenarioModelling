using FluentAssertions;
using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.Serialisation.ProtoBuf;
using ScenarioModelling.TestDataAndTools;
using ScenarioModelling.TestDataAndTools.CodeHooks;
using ScenarioModelling.TestDataAndTools.Serialisation;
using System.Diagnostics;

namespace ScenarioModelling.Serialisation.Tests;

[TestClass]
public class ProtoBufSerialisationTests
{
    private const string MetaStoryName = "MetaStory recorded by hooks";

    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("ProtoBuf")]
    [ReserialisationDataProvider]
    public void ProtoBuf_Context_DeserialiseReserialise(string testCaseName, string originalContextText, string expectedFinalContextText)
        => ProtoBuf_Context_SerialiseDeserialise_Common(testCaseName, originalContextText, expectedFinalContextText);

    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("ProtoBuf")]
    [ProgressiveCodeHookTestDataProvider]
    public void ProtoBuf_Context_DeserialiseReserialise_FromHookTestData(string metaStoryMethodName, string systemMethodName, bool autoDefineMetaStory)
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
            context.Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        ProtoBuf_Context_SerialiseDeserialise_Common(metaStoryMethodName, serialisedContext, "");
    }

    public void ProtoBuf_Context_SerialiseDeserialise_Common(string testCaseName, string originalContextText, string expectedFinalContextText)
    {
        // Arrange
        // =======

        // Act
        // ===

        ScenarioModellingContainer container = new();

        Context originalContext =
            container.Context
                     .UseSerialiser<ContextSerialiser>()
                     .UseSerialiser<ProtoBufSerialiser>()
                     .UseSerialiser<ProtoBufSerialiser_Uncompressed>()
                     .LoadContext<ContextSerialiser>(originalContextText)
                     .Initialise();

        originalContext.ValidationErrors.Count.Should().Be(0, because: originalContext.ValidationErrors.ToString());

        string uncompressedContextText = "";
        originalContext.Serialise<ProtoBufSerialiser_Uncompressed>().Switch(str => uncompressedContextText = str, ex => Assert.Fail(ex.Message));

        originalContext.Serialise<ProtoBufSerialiser>()
                       .Switch(
            serialisedContextText =>
            {
                Debug.WriteLine("");
                Debug.WriteLine("Serialised context");
                Debug.WriteLine("==================");
                Debug.WriteLine("");
                Debug.WriteLine(serialisedContextText);
                Debug.WriteLine("");
                Debug.WriteLine("Uncompressed serialised context");
                Debug.WriteLine("===============================");
                Debug.WriteLine("");
                Debug.WriteLine(uncompressedContextText);
                Debug.WriteLine("");
                Debug.WriteLine("Comparison");
                Debug.WriteLine("==========");
                Debug.WriteLine("");
                Debug.WriteLine($"{uncompressedContextText.Length} > {serialisedContextText.Length}");
                Debug.WriteLine("");

                ScenarioModellingContainer reloadingContainer = new();

                Context reloadedContext =
                    reloadingContainer.Context
                                      .UseSerialiser<ProtoBufSerialiser>()
                                      .LoadContext(serialisedContextText)
                                      .Initialise();

                string reserialisedContext =
                    reloadedContext.Serialise<ProtoBufSerialiser>()
                                   .Match(v => v, e => throw e);

                // Assert
                // ======


                reloadedContext.ValidationErrors.Count.Should().Be(0, $"because {string.Join('\n', reloadedContext.ValidationErrors)}");

                DiffAssert.DiffIfNotEqual(serialisedContextText.Trim(), reserialisedContext.Trim(), leftName: $@"Expected_{testCaseName.Replace(' ', '_')}", rightName: $@"Result_{testCaseName.Replace(' ', '_')}");

                //reloadedContext.IsEqv(originalContext).Should().Be(true);

            },
            ex => Assert.Fail(ex.Message)
        );
    }
}
