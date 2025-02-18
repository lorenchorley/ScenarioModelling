﻿using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CodeHooks.Tests;
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
public partial class ImplicitNodeCodeHookTests
{
    [DataTestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory Construction"), TestCategory("Implicit Nodes")]
    [ImplicitNodeCodeHookTestDataProvider]
    public void ImplicitNode_CodeHook_metaStoryConstructionTests(string MetaStoryWithImplicitNodeMethodName, string MetaStoryWithoutImplicitNodeMethodName, string systemMethodName)
    {
        // Arrange
        // =======
        ScenarioModellingContainer container = new();

        var context =
            container.Context
                     .UseSerialiser<ContextSerialiser>()
                     .Initialise();

        MetaStoryHookOrchestrator orchestrator = container.GetService<MetaStoryHookOrchestratorForConstruction>();

        var systemHooksMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(systemMethodName);
        var MetaStoryWithImplicitNodeMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(MetaStoryWithImplicitNodeMethodName);
        var MetaStoryWithoutImplicitNodeMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(MetaStoryWithoutImplicitNodeMethodName);

        // Build system
        orchestrator.DefineMetaState(sysConf =>
        {
            systemHooksMethod(sysConf);
        });


        // Act
        // ===

        // Build MetaStory with the implicit defintion first
        orchestrator.StartMetaStory(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);
        MetaStoryWithoutImplicitNodeMethod(orchestrator);
        orchestrator.EndMetaStory();

        var firstSerialisedContext =
            context.Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        // The build MetaStory without the implicit defintion to make sure that the implicit definition doesn't cause any problems when it's not defined at the right time in the second MetaStory
        orchestrator.StartMetaStory(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);
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
        TestContainer container = new();

        var context =
            container.Context
                     .UseSerialiser<ContextSerialiser>()
                     .Initialise();

        MetaStoryHookOrchestrator orchestrator = container.GetService<MetaStoryHookOrchestratorForConstruction>();
        StoryTestRunner runner = container.GetService<StoryTestRunner>();

        var systemHooksMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(systemMethodName);
        var MetaStoryWithImplicitNodeMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(MetaStoryWithImplicitNodeMethodName);
        var MetaStoryWithoutImplicitNodeMethod = ImplicitNodeCodeHookTestDataProviderAttribute.GetAction<MetaStoryHookOrchestrator>(MetaStoryWithoutImplicitNodeMethodName);

        // Build system
        orchestrator.DefineMetaState(systemHooksMethod);


        // Act
        // ===

        // Build MetaStory with the implicit defintion first
        orchestrator.StartMetaStory(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);
        MetaStoryWithoutImplicitNodeMethod(orchestrator);
        orchestrator.EndMetaStory();

        Story firstRun = runner.Run(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);

        // The build MetaStory without the implicit defintion to make sure that the implicit definition doesn't cause any problems when it's not defined at the right time in the second MetaStory
        orchestrator.StartMetaStory(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);
        MetaStoryWithImplicitNodeMethod(orchestrator);
        orchestrator.EndMetaStory();

        Story secondRun = runner.Run(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);


        // Assert
        // ======
        string firstSerialisedEvents = firstRun.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();
        string secondSerialisedEvents = secondRun.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();

        DiffAssert.DiffIfNotEqual(firstSerialisedEvents, secondSerialisedEvents);
    }
}
