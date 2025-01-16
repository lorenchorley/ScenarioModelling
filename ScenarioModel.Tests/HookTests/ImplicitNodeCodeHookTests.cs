using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;
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

    private record ImplicitNodeTestCase(string MetaStoryWithImplicitNodeMethodName, string MetaStoryWithoutImplicitNodeMethodName, string SystemMethodName);
    private class ImplicitNodeCodeHookTestDataProviderAttribute : Attribute, ITestDataSource
    {
        private List<ImplicitNodeTestCase> TestData = [
            new(nameof(OneDialogAndOneJump_ImplicitMetaStory), nameof(OneDialogAndOneJump_FullMetaStory), nameof(SystemEmpty)),
            new(nameof(TwoDialogsAndOneJump_ImplicitMetaStory), nameof(TwoDialogsAndOneJump_FullMetaStory), nameof(SystemEmpty)),
            new(nameof(IfExecutesWithDialog_HookOutsideBlock_ImplicitIfNode), nameof(IfExecutesWithDialog_HookOutsideBlock_FullMetaStory), nameof(SystemOneActorTwoStates)),
            new(nameof(WhileExecutesTwiceWithTransition_ImplicitWhileNode), nameof(WhileExecutesTwiceWithTransition_FullMetaStory), nameof(SystemOneActorThreeStatesSingleTransition)),
        ];

        public IEnumerable<object[]> GetData(MethodInfo methodInfo) => TestData.Select((Func<ImplicitNodeTestCase, object[]>)(t => [t.MetaStoryWithImplicitNodeMethodName, t.MetaStoryWithoutImplicitNodeMethodName, t.SystemMethodName]));
        public string GetDisplayName(MethodInfo methodInfo, object?[]? data) => data?[0]?.ToString() ?? "";
    }

    #region Systems
    private static void SystemEmpty(SystemHookDefinition sysConf)
    {
    }

    private static void SystemOneActorTwoStates(SystemHookDefinition sysConf)
    {
        sysConf.DefineEntity("Actor")
               .SetState("S1");

        sysConf.DefineStateMachine("SM1")
               .WithState("S1")
               .WithState("S2")
               .WithTransition("S1", "S2", "T1");
    }

    private static void SystemOneActorThreeStatesSingleTransition(SystemHookDefinition sysConf)
    {
        sysConf.DefineEntity("Actor")
               .SetState("S1");

        sysConf.DefineStateMachine("SM1")
               .WithState("S1")
               .WithState("S2")
               .WithState("S3")
               .WithTransition("S1", "S2", "T1")
               .WithTransition("S2", "S3", "T1");
    }
    #endregion

    #region Jump
    private static void OneDialogAndOneJump_FullMetaStory(MetaStoryHookOrchestrator hooks)
    {
        hooks.DeclareJump("D1")
             .SetAsImplicit()
             .Build();

        hooks.DeclareDialog("Some text")
             .SetId("D1")
             .Build();
    }

    private static void OneDialogAndOneJump_ImplicitMetaStory(MetaStoryHookOrchestrator hooks)
    {
        //hooks.DeclareJump("D1")
        //     .SetAsImplicit();

        hooks.DeclareDialog("Some text")
             .SetId("D1")
             .Build();
    }

    private static void TwoDialogsAndOneJump_FullMetaStory(MetaStoryHookOrchestrator hooks)
    {
        hooks.DeclareJump("D2")
             .SetAsImplicit()
             .Build();

        hooks.DeclareDialog("Some text")
             .SetId("D1")
             .Build();

        hooks.DeclareDialog("Some more text")
             .SetId("D2")
             .Build();
    }

    private static void TwoDialogsAndOneJump_ImplicitMetaStory(MetaStoryHookOrchestrator hooks)
    {
        //hooks.DeclareJump("D2")
        //     .SetAsImplicit();

        //hooks.DeclareDialog("Some text")
        //     .SetId("D1");

        hooks.DeclareDialog("Some more text")
             .SetId("D2")
             .Build();
    }
    #endregion

    #region If
    private static void IfExecutesWithDialog_HookOutsideBlock_FullMetaStory(MetaStoryHookOrchestrator hooks)
    {
        hooks.DeclareIfBranch(@"Actor.State == S1")
             .SetAsImplicit()
             .GetConditionHooks(out IfConditionHook φ, out IfBlockEndHook ifBlockEndHook)
             .Build();

        bool condition = true;
        if (φ(condition))
        {
            hooks.DeclareDialog("Inside if block")
                 .Build();
        }
        ifBlockEndHook();

        hooks.DeclareDialog("After if block")
             .Build();
    }

    private static void IfExecutesWithDialog_HookOutsideBlock_ImplicitIfNode(MetaStoryHookOrchestrator hooks)
    {
        //hooks.DeclareIfBranch(@"Actor.State == S1")
        //     .SetAsImplicit()
        //     .GetConditionHooks(out IfConditionHook φ, out IfBlockEndHook ifBlockEndHook);

        bool condition = true;
        //if (φ(condition))
        if (condition)
        {
            hooks.DeclareDialog("Inside if block")
                 .Build();
        }
        //ifBlockEndHook();

        hooks.DeclareDialog("After if block")
             .Build();
    }
    #endregion

    #region While
    private static void WhileExecutesTwiceWithTransition_FullMetaStory(MetaStoryHookOrchestrator hooks)
    {
        hooks.DeclareWhileBranch(@"Actor.State != S3")
             .SetAsImplicit()
             .GetConditionHook(out WhileHook φ)
             .Build();

        int count = 2;
        while (φ(count > 0))
        {
            hooks.DeclareTransition("Actor", "T1")
                 .Build();

            count--;
        }

        hooks.DeclareDialog("After while block")
             .Build();
    }

    private static void WhileExecutesTwiceWithTransition_ImplicitWhileNode(MetaStoryHookOrchestrator hooks)
    {
        //hooks.DeclareWhileBranch(@"Actor.State != S3")
        //     .SetAsImplicit()
        //     .GetConditionHook(out WhileHook φ);

        int count = 2;
        //while (φ(count > 0))
        while (count > 0)
        {
            hooks.DeclareTransition("Actor", "T1")
                 .Build();

            count--;
        }

        hooks.DeclareDialog("After while block")
             .Build();
    }
    #endregion

    // TODO Test cases :
    // * Two consecutive implicit nodes
    // * An implicit node followed by an implicit node in a subgraph
    // * An implicit node followed by an implicit node in the parent graph
    // More ?

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

        var systemHooksMethod = GetAction<SystemHookDefinition>(systemMethodName);
        var MetaStoryWithImplicitNodeMethod = GetAction<MetaStoryHookOrchestrator>(MetaStoryWithImplicitNodeMethodName);
        var MetaStoryWithoutImplicitNodeMethod = GetAction<MetaStoryHookOrchestrator>(MetaStoryWithoutImplicitNodeMethodName);

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

        var systemHooksMethod = GetAction<SystemHookDefinition>(systemMethodName);
        var MetaStoryWithImplicitNodeMethod = GetAction<MetaStoryHookOrchestrator>(MetaStoryWithImplicitNodeMethodName);
        var MetaStoryWithoutImplicitNodeMethod = GetAction<MetaStoryHookOrchestrator>(MetaStoryWithoutImplicitNodeMethodName);

        // Build system
        orchestrator.DefineSystem(sysConf =>
        {
            systemHooksMethod(sysConf);
        });

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

    private Action<T> GetAction<T>(string systemMethodName)
    {
        var methodRef =
            GetType().GetMethod(systemMethodName, BindingFlags.NonPublic | BindingFlags.Static)
                     ?? throw new Exception($"Method {systemMethodName} not found in {GetType().Name}");

        return (T parameter) => methodRef.Invoke(this, [parameter]);
    }
}
