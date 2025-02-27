using FluentAssertions.Common;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Execution;
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
    public async Task ProgressiveDevelopment_CodeHooks_MetaStoryConstructionTests(string metaStoryMethodName, string metaStateMethodName, bool autoDefineMetaStory)
    {
        // Arrange
        // =======
        using TestContainer container = new();
        using var scope = container.StartScope();
        
        var context =
            scope.GetService<Context>()
                 .UseSerialiser<CustomContextSerialiser>()
                 .Initialise();

        MetaStoryHookOrchestrator orchestrator = scope.GetService<MetaStoryHookOrchestratorForConstruction>();

        var metaStateHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(metaStateMethodName);
        var metaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(metaStoryMethodName);


        // Act
        // ===

        // Build metaState
        orchestrator.DefineMetaState(metaStateHooksMethod);

        // Build MetaStory
        orchestrator.StartMetaStory(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);
        metaStoryHooksMethod(orchestrator);
        orchestrator.EndMetaStory();


        // Assert
        // ======
        var serialisedContext =
            context.ResetToInitialState()
                   .Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        await Verify(serialisedContext)
            .UseParameters(metaStoryMethodName);
    }

    [DataTestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory -> Story")]
    [ProgressiveCodeHookTestDataProvider]
    public async Task ProgressiveDevelopment_CodeHooks_StoryExtractionTests(string metaStoryMethodName, string systemMethodName, bool autoDefineMetaStory)
    {
        // Arrange
        // =======
        using TestContainer container = new();
        using var scope = container.StartScope();

        var context =
            scope.GetService<Context>()
                 .UseSerialiser<CustomContextSerialiser>()
                 .Initialise();

        MetaStoryHookOrchestrator orchestrator = scope.GetService<MetaStoryHookOrchestratorForConstruction>();

        var metaStateHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(systemMethodName);
        var MetaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(metaStoryMethodName);

        // Build metaState
        orchestrator.DefineMetaState(metaStateHooksMethod);

        // Build MetaStory
        orchestrator.StartMetaStory(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);
        MetaStoryHooksMethod(orchestrator);
        orchestrator.EndMetaStory();

        StoryTestRunner runner = scope.GetService<StoryTestRunner>();


        // Act
        // ===
        Story story = runner.Run(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);


        // Assert
        // ======
        string serialisedStory = story.Events.GetEnumerable().Select(e => e?.ToString() ?? "").BulletPointList().Trim();

        await Verify(serialisedStory)
            .UseParameters(metaStoryMethodName);

    }

}
