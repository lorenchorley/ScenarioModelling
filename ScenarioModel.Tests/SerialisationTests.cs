using FluentAssertions;
using ProtoBuf;
using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.Serialisation;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.Serialisation.ProtoBuf;
using ScenarioModelling.Tests.HookTests.Providers;
using ScenarioModelling.Tests.Valid;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace ScenarioModelling.Tests;

[TestClass]
public class SerialisationTests
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
    public void HumanReadable_Context_DeserialiseReserialise_FromHookTestData(string metaStoryMethodName, string systemMethodName)
    {
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestrator orchestrator = new MetaStoryHookOrchestratorForConstruction(context);

        var systemHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<SystemHookDefinition>(systemMethodName);
        var metaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(metaStoryMethodName);

        // Build system
        orchestrator.DefineSystem(systemHooksMethod);

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

        var expectedText =
            $$"""
            {{expectedSystemText.Trim()}}

            MetaStory "{{MetaStoryName}}" {
            {{expectedMetaStoryText.Trim().AddIndent("  ")}}
            }
            """;

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

        Context loadedContext =
            Context.New()
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

                Context reloadedContext = Context.New()
                       .UseSerialiser<ContextSerialiser>()
                       .LoadContext(reserialisedContextText)
                       .Initialise();


                // Assert
                // ======
                reloadedContext.ValidationErrors.Count.Should().Be(0, $"because {string.Join('\n', reloadedContext.ValidationErrors)}");

                DiffAssert.DiffIfNotEqual(expectedFinalContextText.Trim(), reserialisedContextText.Trim(), leftName: $@"Expected_{testCaseName.Replace(' ', '_')}", rightName: $@"Result_{testCaseName.Replace(' ', '_')}");
            },
            ex => Assert.Fail(ex.Message)
        );
    }

    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("Yaml")]
    [ReserialisationDataProvider]
    public void Yaml_Context_DeserialiseReserialise(string testCaseName, string originalContextText, string expectedFinalContextText)
        => Yaml_Context_SerialiseDeserialise_Common(testCaseName, originalContextText, expectedFinalContextText);

    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("Yaml")]
    [ProgressiveCodeHookTestDataProvider]
    public void Yaml_Context_DeserialiseReserialise_FromHookTestData(string metaStoryMethodName, string systemMethodName)
    {
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestrator orchestrator = new MetaStoryHookOrchestratorForConstruction(context);

        var systemHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<SystemHookDefinition>(systemMethodName);
        var metaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(metaStoryMethodName);

        // Build system
        orchestrator.DefineSystem(systemHooksMethod);

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

        Context originalContext =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .UseSerialiser<YamlSerialiser>()
                   .LoadContext<ContextSerialiser>(originalContextText)
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

                Context reloadedContext =
                    Context.New()
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

    
    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("ProtoBuf")]
    [ReserialisationDataProvider]
    public void ProtoBuf_Context_DeserialiseReserialise(string testCaseName, string originalContextText, string expectedFinalContextText)
        => ProtoBuf_Context_SerialiseDeserialise_Common(testCaseName, originalContextText, expectedFinalContextText);

    [TestMethod]
    [TestCategory("Serialisation"), TestCategory("ProtoBuf")]
    [ProgressiveCodeHookTestDataProvider]
    public void ProtoBuf_Context_DeserialiseReserialise_FromHookTestData(string metaStoryMethodName, string systemMethodName)
    {
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestrator orchestrator = new MetaStoryHookOrchestratorForConstruction(context);

        var systemHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<SystemHookDefinition>(systemMethodName);
        var metaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(metaStoryMethodName);

        // Build system
        orchestrator.DefineSystem(systemHooksMethod);

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

        Context originalContext =
            Context.New()
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

                Context reloadedContext =
                    Context.New()
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
