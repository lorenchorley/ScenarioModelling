using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CoreObjects.Expressions.Evaluation;
using ScenarioModelling.CoreObjects.Interpolation;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.TestDataAndTools;
using ScenarioModelling.TestDataAndTools.CodeHooks;

namespace ScenarioModelling.Tests.HookTests;

[TestClass]
[UsesVerify]
public partial class ProgressiveCodeHookTests
{
    [DataTestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory Construction")]
    [ProgressiveCodeHookTestDataProvider]
    public async Task ProgressiveDevelopment_CodeHooks_metaStoryConstructionTests(string metaStoryMethodName, string systemMethodName)
    {
        // Arrange
        // =======
        ScenarioModellingContainer container = new();

        var context =
            container.Context
                     .UseSerialiser<ContextSerialiser>()
                     .Initialise();

        MetaStoryHookOrchestrator orchestrator = container.GetService<MetaStoryHookOrchestratorForConstruction>();

        var systemHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(systemMethodName);
        var metaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(metaStoryMethodName);


        // Act
        // ===

        // Build system
        orchestrator.DefineMetaState(systemHooksMethod);

        // Build MetaStory
        orchestrator.StartMetaStory("MetaStory recorded by hooks");
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
    public async Task ProgressiveDevelopment_CodeHooks_StoryExtractionTests(string metaStoryMethodName, string systemMethodName)
    {
        // Arrange
        // =======
        TestContainer container = new();

        var context =
            container.Context
                     .UseSerialiser<ContextSerialiser>()
                     .Initialise();

        MetaStoryHookOrchestrator orchestrator = container.GetService<MetaStoryHookOrchestratorForConstruction>(); 

        var systemHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(systemMethodName);
        var MetaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(metaStoryMethodName);

        // Build system
        orchestrator.DefineMetaState(sysConf =>
        {
            systemHooksMethod(sysConf);
        });

        // Build MetaStory
        orchestrator.StartMetaStory("MetaStory recorded by hooks");
        MetaStoryHooksMethod(orchestrator);
        orchestrator.EndMetaStory();

        StoryTestRunner runner = container.GetService<StoryTestRunner>();


        // Act
        // ===
        Story story = runner.Run("MetaStory recorded by hooks");


        // Assert
        // ======
        string serialisedStory = story.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();

        await Verify(serialisedStory)
            .UseParameters(metaStoryMethodName);

    }

}
