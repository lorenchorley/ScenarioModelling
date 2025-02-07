using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Exhaustiveness.Common;
using ScenarioModelling.Expressions.Evaluation;
using ScenarioModelling.Interpolation;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.Tests.Stories;
using System.Reflection;

namespace ScenarioModelling.Tests.HookTests;

[TestClass]
[UsesVerify]
public partial class ImplicitNodeCodeHookTests
{
    [DataTestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory Construction"), TestCategory("Implicit Nodes")]
    [ImplicitNodeCodeHookTestDataProvider]
    public void ImplicitNode_CodeHook_metaStoryConstructionTests(string MetaStoryWithImplicitNodeMethodName, string MetaStoryWithoutImplicitNodeMethodName, string systemMethodName)
    {
        // Arrange
        // =======
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestrator orchestrator = new MetaStoryHookOrchestratorForConstruction(context);

        var systemHooksMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<SystemHookDefinition>(systemMethodName);
        var MetaStoryWithImplicitNodeMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(MetaStoryWithImplicitNodeMethodName);
        var MetaStoryWithoutImplicitNodeMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(MetaStoryWithoutImplicitNodeMethodName);

        // Build system
        orchestrator.DefineSystem(sysConf =>
        {
            systemHooksMethod(sysConf);
        });


        // Act
        // ===

        // Build MetaStory with the implicit defintion first
        orchestrator.StartMetaStory("MetaStory recorded by hooks");
        MetaStoryWithoutImplicitNodeMethod(orchestrator);
        orchestrator.EndMetaStory();

        var firstSerialisedContext =
            context.Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        // The build MetaStory without the implicit defintion to make sure that the implicit definition doesn't cause any problems when it's not defined at the right time in the second MetaStory
        orchestrator.StartMetaStory("MetaStory recorded by hooks");
        MetaStoryWithImplicitNodeMethod(orchestrator);
        orchestrator.EndMetaStory();

        var secondSerialisedContext =
            context.Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();


        // Assert
        // ======

        DiffAssert.DiffIfNotEqual(firstSerialisedContext, secondSerialisedContext);
    }

    [DataTestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory -> Story"), TestCategory("Implicit Nodes")]
    [ImplicitNodeCodeHookTestDataProvider]
    public void ImplicitNode_CodeHook_StoryExtractionTests(string MetaStoryWithImplicitNodeMethodName, string MetaStoryWithoutImplicitNodeMethodName, string systemMethodName)
    {
        // Arrange
        // =======
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestrator orchestrator = new MetaStoryHookOrchestratorForConstruction(context);

        var systemHooksMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<SystemHookDefinition>(systemMethodName);
        var MetaStoryWithImplicitNodeMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(MetaStoryWithImplicitNodeMethodName);
        var MetaStoryWithoutImplicitNodeMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(MetaStoryWithoutImplicitNodeMethodName);

        // Build system
        orchestrator.DefineSystem(systemHooksMethod);

        ExpressionEvalator evalator = new(context.System);
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);
        StoryTestRunner runner = new(executor, dependencies);


        // Act
        // ===

        // Build MetaStory with the implicit defintion first
        orchestrator.StartMetaStory("MetaStory recorded by hooks");
        MetaStoryWithoutImplicitNodeMethod(orchestrator);
        orchestrator.EndMetaStory();

        Story firstRun = runner.Run("MetaStory recorded by hooks");

        // The build MetaStory without the implicit defintion to make sure that the implicit definition doesn't cause any problems when it's not defined at the right time in the second MetaStory
        orchestrator.StartMetaStory("MetaStory recorded by hooks");
        MetaStoryWithImplicitNodeMethod(orchestrator);
        orchestrator.EndMetaStory();

        Story secondRun = runner.Run("MetaStory recorded by hooks");


        // Assert
        // ======
        string firstSerialisedEvents = firstRun.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();
        string secondSerialisedEvents = secondRun.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();

        DiffAssert.DiffIfNotEqual(firstSerialisedEvents, secondSerialisedEvents);
    }
}
