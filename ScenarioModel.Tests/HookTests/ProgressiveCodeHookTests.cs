using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Expressions.Evaluation;
using ScenarioModelling.Interpolation;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.Tests.HookTests.Providers;
using ScenarioModelling.Tests.Stories;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestrator orchestrator = new MetaStoryHookOrchestratorForConstruction(context);

        var systemHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<SystemHookDefinition>(systemMethodName);
        var metaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(metaStoryMethodName);

        
        // Act
        // ===

        // Build system
        orchestrator.DefineSystem(systemHooksMethod);

        // Build MetaStory
        orchestrator.StartMetaStory("MetaStory recorded by hooks");
        metaStoryHooksMethod(orchestrator);
        orchestrator.EndMetaStory();


        // Assert
        // ======
        var serialisedContext =
            context.Serialise()
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
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestrator orchestrator = new MetaStoryHookOrchestratorForConstruction(context);

        var systemHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<SystemHookDefinition>(systemMethodName);
        var MetaStoryHooksMethod = ProgressiveCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(metaStoryMethodName);

        // Build system
        orchestrator.DefineSystem(sysConf =>
        {
            systemHooksMethod(sysConf);
        });

        // Build MetaStory
        orchestrator.StartMetaStory("MetaStory recorded by hooks");
        MetaStoryHooksMethod(orchestrator);
        orchestrator.EndMetaStory();

        ExpressionEvalator evalator = new(context.System);
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);
        StoryTestRunner runner = new(executor, dependencies);


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
