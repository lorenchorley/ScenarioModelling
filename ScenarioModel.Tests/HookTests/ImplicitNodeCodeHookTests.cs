using ScenarioModel.CodeHooks;
using ScenarioModel.CodeHooks.HookDefinitions;
using ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;
using ScenarioModel.Execution;
using ScenarioModel.Execution.Dialog;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Interpolation;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModel.Tests.ScenarioRuns;
using System.Reflection;

namespace ScenarioModel.Tests.HookTests;

[TestClass]
[UsesVerify]
public partial class ImplicitNodeCodeHookTests
{
    
    private record ImplicitNodeTestCase(string ScenarioWithImplicitNodeMethodName, string ScenarioWithoutImplicitNodeMethodName, string SystemMethodName);
    private class ImplicitNodeCodeHookTestDataProviderAttribute : Attribute, ITestDataSource
    {
        private List<ImplicitNodeTestCase> TestData = [
            new(nameof(OneDialogAndOneJump_ImplicitScenario), nameof(OneDialogAndOneJump_FullScenario), nameof(SystemEmpty)),
            new(nameof(TwoDialogsAndOneJump_ImplicitScenario), nameof(TwoDialogsAndOneJump_FullScenario), nameof(SystemEmpty)),
            new(nameof(IfExecutesWithDialog_HookOutsideBlock_ImplicitIfNode), nameof(IfExecutesWithDialog_HookOutsideBlock_FullScenario), nameof(SystemOneActorTwoStates)),
            new(nameof(WhileExecutesTwiceWithTransition_ImplicitWhileNode), nameof(WhileExecutesTwiceWithTransition_FullScenario), nameof(SystemOneActorThreeStatesSingleTransition)),
        ];

        public IEnumerable<object[]> GetData(MethodInfo methodInfo) => TestData.Select((Func<ImplicitNodeTestCase, object[]>)(t => [t.ScenarioWithImplicitNodeMethodName, t.ScenarioWithoutImplicitNodeMethodName, t.SystemMethodName]));
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
    private static void OneDialogAndOneJump_FullScenario(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareJump("D1")
             .SetAsImplicit()
             .Build();

        hooks.DeclareDialog("Some text")
             .SetId("D1")
             .Build();
    }

    private static void OneDialogAndOneJump_ImplicitScenario(ScenarioHookOrchestrator hooks)
    {
        //hooks.DeclareJump("D1")
        //     .SetAsImplicit();

        hooks.DeclareDialog("Some text")
             .SetId("D1")
             .Build();
    }
    
    private static void TwoDialogsAndOneJump_FullScenario(ScenarioHookOrchestrator hooks)
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

    private static void TwoDialogsAndOneJump_ImplicitScenario(ScenarioHookOrchestrator hooks)
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
    private static void IfExecutesWithDialog_HookOutsideBlock_FullScenario(ScenarioHookOrchestrator hooks)
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

    private static void IfExecutesWithDialog_HookOutsideBlock_ImplicitIfNode(ScenarioHookOrchestrator hooks)
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
    private static void WhileExecutesTwiceWithTransition_FullScenario(ScenarioHookOrchestrator hooks)
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

    private static void WhileExecutesTwiceWithTransition_ImplicitWhileNode(ScenarioHookOrchestrator hooks)
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
    public void ImplicitNode_CodeHook_MetaStoryConstructionTests(string scenarioWithImplicitNodeMethodName, string scenarioWithoutImplicitNodeMethodName, string systemMethodName)
    {
        // Arrange
        // =======
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        ScenarioHookOrchestrator orchestrator = new ScenarioHookOrchestratorForConstruction(context);

        var systemHooksMethod = GetAction<SystemHookDefinition>(systemMethodName);
        var scenarioWithImplicitNodeMethod = GetAction<ScenarioHookOrchestrator>(scenarioWithImplicitNodeMethodName);
        var scenarioWithoutImplicitNodeMethod = GetAction<ScenarioHookOrchestrator>(scenarioWithoutImplicitNodeMethodName);

        // Build system
        orchestrator.DefineSystem(sysConf =>
        {
            systemHooksMethod(sysConf);
        });


        // Act
        // ===

        // Build scenario with the implicit defintion first
        orchestrator.DeclareScenarioStart("Scenario recorded by hooks");
        scenarioWithoutImplicitNodeMethod(orchestrator);
        orchestrator.DeclareScenarioEnd();

        var firstSerialisedContext =
            context.Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        // The build scenario without the implicit defintion to make sure that the implicit definition doesn't cause any problems when it's not defined at the right time in the second scenario
        orchestrator.DeclareScenarioStart("Scenario recorded by hooks");
        scenarioWithImplicitNodeMethod(orchestrator);
        orchestrator.DeclareScenarioEnd();

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
    public void ImplicitNode_CodeHook_StoryExtractionTests(string scenarioWithImplicitNodeMethodName, string scenarioWithoutImplicitNodeMethodName, string systemMethodName)
    {
        // Arrange
        // =======
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        ScenarioHookOrchestrator orchestrator = new ScenarioHookOrchestratorForConstruction(context);

        var systemHooksMethod = GetAction<SystemHookDefinition>(systemMethodName);
        var scenarioWithImplicitNodeMethod = GetAction<ScenarioHookOrchestrator>(scenarioWithImplicitNodeMethodName);
        var scenarioWithoutImplicitNodeMethod = GetAction<ScenarioHookOrchestrator>(scenarioWithoutImplicitNodeMethodName);

        // Build system
        orchestrator.DefineSystem(sysConf =>
        {
            systemHooksMethod(sysConf);
        });

        ExpressionEvalator evalator = new(context.System);
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);
        ScenarioTestRunner runner = new(executor, dependencies);


        // Act
        // ===

        // Build scenario with the implicit defintion first
        orchestrator.DeclareScenarioStart("Scenario recorded by hooks");
        scenarioWithoutImplicitNodeMethod(orchestrator);
        orchestrator.DeclareScenarioEnd();

        Story firstRun = runner.Run("Scenario recorded by hooks");

        // The build scenario without the implicit defintion to make sure that the implicit definition doesn't cause any problems when it's not defined at the right time in the second scenario
        orchestrator.DeclareScenarioStart("Scenario recorded by hooks");
        scenarioWithImplicitNodeMethod(orchestrator);
        orchestrator.DeclareScenarioEnd();

        Story secondRun = runner.Run("Scenario recorded by hooks");


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
