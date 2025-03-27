using FluentAssertions;
using LanguageExt;
using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions.MetaStateObjects;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Exhaustiveness.Common;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;
using ScenarioModelling.TestDataAndTools;
using ScenarioModelling.TestDataAndTools.CodeHooks;

namespace ScenarioModelling.Tests.HookTests;

[TestClass]
[UsesVerify]
public partial class ProgressiveCodeHookTests
{
    [AssemblyInitialize()]
    public static void AssemblyInit(TestContext context)
    {
        ExhaustivityFunctions.Active = true;
    }

    [DataTestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory Construction")]
    [ProgressiveCodeHookTestDataProvider]
    public async Task ProgressiveDevelopment_CodeHooks_MetaStoryConstructionTests(string metaStoryMethodName, string metaStateMethodName, bool testDefinedFirstMetaStory, int loopCount)
    {
        // Arrange
        // =======
        using TestContainer container = new();
        using var scope = container.StartScope();

        var context =
            scope.GetService<Context>()
                 .UseSerialiser<CustomContextSerialiser>()
                 .Initialise();

        HookOrchestrator orchestrator = scope.GetService<MetaStoryHookOrchestratorForConstruction>();
        orchestrator.ThrowOnConstraintOrAssertionFailure();

        var metaStateHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(metaStateMethodName);
        var metaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<HookOrchestrator>(metaStoryMethodName);
        var expectedSerialisedContext = ProgressiveCodeHookTestDataProviderAttribute.GetExpectedContextText(metaStateMethodName, metaStoryMethodName, testDefinedFirstMetaStory);


        // Act
        // ===

        // Build metaState
        orchestrator.DefineMetaState(metaStateHooksMethod);

        // Build MetaStory
        if (!testDefinedFirstMetaStory)
            orchestrator.StartMetaStory(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);

        metaStoryHooksMethod(orchestrator);

        if (!testDefinedFirstMetaStory)
            orchestrator.EndMetaStory();


        // Assert
        // ======
        var serialisedContext =
            context.ResetToInitialState()
                   .Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        Assert.AreEqual(expectedSerialisedContext.Trim(), serialisedContext);

        await Verify(serialisedContext)
            .UseParameters(metaStoryMethodName);
    }

    [DataTestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory -> Story")]
    [ProgressiveCodeHookTestDataProvider]
    public async Task ProgressiveDevelopment_CodeHooks_StoryExtractionTests(string metaStoryMethodName, string metaStateMethodName, bool testDefinedFirstMetaStory, int loopCount)
    {
        // Arrange
        // =======
        using TestContainer container = new();
        using var scope = container.StartScope();

        var context =
            scope.GetService<Context>()
                 .UseSerialiser<CustomContextSerialiser>()
                 .Initialise();

        HookOrchestrator orchestrator = scope.GetService<MetaStoryHookOrchestratorForConstruction>();

        var metaStateHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(metaStateMethodName);
        var metaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<HookOrchestrator>(metaStoryMethodName);

        // Build metaState
        orchestrator.DefineMetaState(metaStateHooksMethod);

        // Build MetaStory
        if (!testDefinedFirstMetaStory)
            orchestrator.StartMetaStory(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);

        metaStoryHooksMethod(orchestrator);

        Story? hookGeneratedStory = null;
        if (!testDefinedFirstMetaStory)
            (_, hookGeneratedStory) = orchestrator.EndMetaStory();

        StoryTestRunner runner = scope.GetService<StoryTestRunner>();


        // Act
        // ===
        Story story = runner.Run(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);


        // Assert
        // ======

        // Check the rerun story
        string serialisedStory = story.EventSourceLog.GetEnumerable().Select(e => e?.ToString() ?? "").BulletPointList().Trim();

        await Verify(serialisedStory)
            .UseParameters(metaStoryMethodName);

        int metaStoryCallCount = story.EventSourceLog.GetEnumerable().Count(e => e is MetaStoryCalledEvent);
        int metaStoryReturnedCount = story.EventSourceLog.GetEnumerable().Count(e => e is MetaStoryReturnedEvent);

        metaStoryCallCount.Should().Be(metaStoryReturnedCount, "MetaStoryCalledEvent and MetaStoryReturnedEvent should be of equal quantity in the event log");

        // Check the story generated in parallel while the hooks where generating the metastory
        string serialisedParallelStory = story.EventSourceLog.GetEnumerable().Select(e => e?.ToString() ?? "").BulletPointList().Trim();

        DiffAssert.DiffIfNotEqual(serialisedParallelStory, serialisedStory);

    }

}
