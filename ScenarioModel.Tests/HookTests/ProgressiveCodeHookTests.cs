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
public partial class ProgressiveCodeHookTests
{
    private record TestCase(string ScenarioMethodName, string SystemMethodName);
    private class ProgressiveCodeHookTestDataProviderAttribute : Attribute, ITestDataSource
    {
        private List<TestCase> TestData = [
            new(nameof(OneDialog), nameof(SystemOneActor)),
            new(nameof(OneDialogWithMultipleWords), nameof(SystemOneActor)),
            new(nameof(OneDialogWithId), nameof(SystemOneActor)),
            new(nameof(OneDialogWithCharacter), nameof(SystemOneActor)),
            new(nameof(TwoStatesOneTransition), nameof(SystemOneActorTwoStates)),
            new(nameof(TwoStatesOneTransitionWithId), nameof(SystemOneActorTwoStates)),
            new(nameof(OneDialogAndOneJump), nameof(SystemEmpty)),
            new(nameof(OneConstraintAlwaysValid), nameof(SystemOneActorTwoStatesWithConstraint)),
            new(nameof(OneConstraintFailsOnTransition), nameof(SystemOneActorTwoStatesWithConstraint)),
            new(nameof(IfDoesNotExecute), nameof(SystemOneActorTwoStates)),
            new(nameof(IfExecutesWithDialog), nameof(SystemOneActorTwoStates)),
            new(nameof(TwoNestedIfsThatExecute), nameof(SystemOneActorThreeStatesSingleTransition)),
            new(nameof(TwoConsecutiveIfsThatExecute), nameof(SystemOneActorThreeStatesSingleTransition)),
            new(nameof(WhileDoesNotExecute), nameof(SystemOneActorThreeStatesSingleTransition)),
            new(nameof(WhileExecutesOnceWithDialog), nameof(SystemOneActorThreeStatesSingleTransition)),
            new(nameof(WhileExecutesTwiceWithTransition), nameof(SystemOneActorThreeStatesSingleTransition)),
            new(nameof(WhileExecutesTwiceWithTransitionAndDialog), nameof(SystemOneActorThreeStatesSingleTransition)),
        ];

        public IEnumerable<object[]> GetData(MethodInfo methodInfo) => TestData.Select((Func<TestCase, object[]>)(t => [t.ScenarioMethodName, t.SystemMethodName]));
        public string GetDisplayName(MethodInfo methodInfo, object?[]? data) => data?[0]?.ToString() ?? "";
    }

    #region System
    private static void SystemEmpty(SystemHookDefinition sysConf)
    {
    }

    private static void SystemOneActor(SystemHookDefinition sysConf)
    {
        sysConf.DefineEntity("Actor");

        // TODO new test on SetType
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
    
    private static void SystemOneActorTwoStatesWithConstraint(SystemHookDefinition sysConf)
    {
        sysConf.DefineEntity("Actor")
               .SetState("S1");

        sysConf.DefineStateMachine("SM1")
               .WithState("S1")
               .WithState("S2")
               .WithTransition("S1", "S2", "T1");

        sysConf.DefineConstraint("State must never be S2")
               .SetExpression("Actor.State != S2");
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

    #region Dialog
    private static void OneDialog(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareDialog("Hello");
    }

    private static void OneDialogWithMultipleWords(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareDialog("Hello with multiple words");
    }

    private static void OneDialogWithId(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareDialog("Hello")
             .SetId("custom dialog id");
    }

    private static void OneDialogWithCharacter(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareDialog("Hello")
             .SetCharacter("Actor");
    }
    #endregion

    #region Transition
    private static void TwoStatesOneTransition(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareTransition("Actor", "T1");
    }

    private static void TwoStatesOneTransitionWithId(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareTransition("Actor", "T1")
             .SetId("custom transition id");
    }
    #endregion

    #region Jump
    private static void OneDialogAndOneJump(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareJump("D1");

        hooks.DeclareDialog("Some text")
             .SetId("D1");
    }
    #endregion
    
    #region Constraint
    private static void OneConstraintAlwaysValid(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareDialog("Some text")
             .SetId("D1");
    }

    private static void OneConstraintFailsOnTransition(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareDialog("Some text")
             .SetId("D1");

        hooks.DeclareTransition("Actor", "T1");
    }

    // TODO more constraints

    #endregion

    #region If
    private static void IfDoesNotExecute(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareIfBranch(@"Actor.State == S2")
             .GetConditionHooks(out IfConditionHook hook, out IfBlockEndHook ifBlockEndHook);

        bool condition = false;
        if (hook(condition))
        {
            hooks.DeclareDialog("Some text");

            ifBlockEndHook();
        }
    }

    private static void IfExecutesWithDialog(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareIfBranch(@"Actor.State == S1")
             .GetConditionHooks(out IfConditionHook hook, out IfBlockEndHook ifBlockEndHook);

        bool condition = true;
        if (hook(condition))
        {
            hooks.DeclareDialog("Some text");

            ifBlockEndHook();
        }
    }

    private static void TwoNestedIfsThatExecute(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareIfBranch(@"Actor.State != S2")
             .GetConditionHooks(out IfConditionHook hook1, out IfBlockEndHook ifBlockEndHook1);

        bool condition = true;
        if (hook1(condition))
        {

            hooks.DeclareIfBranch(@"Actor.State != S3")
                 .GetConditionHooks(out IfConditionHook hook2, out IfBlockEndHook ifBlockEndHook2);

            if (hook2(condition))
            {
                hooks.DeclareDialog("Some text");

                ifBlockEndHook2();
            }

            ifBlockEndHook1();
        }
    }

    private static void TwoConsecutiveIfsThatExecute(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareIfBranch(@"Actor.State != S2")
             .GetConditionHooks(out IfConditionHook hook1, out IfBlockEndHook ifBlockEndHook1);

        bool condition = true;
        if (hook1(condition))
        {
            hooks.DeclareDialog("Some text");

            ifBlockEndHook1();
        }

        hooks.DeclareIfBranch(@"Actor.State != S3")
             .GetConditionHooks(out IfConditionHook hook2, out IfBlockEndHook ifBlockEndHook2);

        if (hook2(condition))
        {
            hooks.DeclareDialog("Some more text");

            ifBlockEndHook2();
        }
    }
    #endregion

    #region While
    private static void WhileDoesNotExecute(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareWhileBranch(@"Actor.State != S1")
             .GetConditionHook(out WhileHook hook);

        int count = 0;
        while (hook(count > 0))
        {
            hooks.DeclareTransition("Actor", "T1");
            count--;
        }
    }

    private static void WhileExecutesOnceWithDialog(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareWhileBranch(@"Actor.State != S2")
             .GetConditionHook(out WhileHook hook);

        int count = 1;
        while (hook(count > 0))
        {
            hooks.DeclareTransition("Actor", "T1");
            count--;
        }
    }

    private static void WhileExecutesTwiceWithTransition(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareWhileBranch(@"Actor.State != S3")
             .GetConditionHook(out WhileHook hook);

        int count = 2;
        while (hook(count > 0))
        {
            hooks.DeclareTransition("Actor", "T1");
            count--;
        }
    }

    private static void WhileExecutesTwiceWithTransitionAndDialog(ScenarioHookOrchestrator hooks)
    {
        hooks.DeclareWhileBranch(@"Actor.State != S3")
             .GetConditionHook(out WhileHook hook);

        int count = 2;
        while (hook(count > 0))
        {
            hooks.DeclareTransition("Actor", "T1");
            hooks.DeclareDialog("Doing T1");
            count--;
        }
    }
    #endregion

    [DataTestMethod]
    [TestCategory("CodeHooks")]
    [ProgressiveCodeHookTestDataProvider]
    public async Task ProgressiveCodeHookTestSuite(string scenarioMethodName, string systemMethodName)
    {
        // Arrange
        // =======
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        ScenarioHookOrchestrator orchestrator = new ScenarioHookOrchestratorForConstruction(context);

        var systemHooksMethod = GetAction<SystemHookDefinition>(systemMethodName);
        var scenarioHooksMethod = GetAction<ScenarioHookOrchestrator>(scenarioMethodName);


        // Act
        // ===

        // Build system
        orchestrator.DefineSystem(sysConf =>
        {
            systemHooksMethod(sysConf);
        });

        // Build scenario
        orchestrator.DeclareScenarioStart("Scenario recorded by hooks");
        scenarioHooksMethod(orchestrator);
        orchestrator.DeclareScenarioEnd();


        // Assert
        // ======
        var serialisedContext =
            context.Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        await Verify(serialisedContext)
            .UseParameters(scenarioMethodName);

    }

    [DataTestMethod]
    [TestCategory("CodeHooks"), TestCategory("ScenarioRuns")]
    [ProgressiveCodeHookTestDataProvider]
    public async Task ProgressiveCodeHookRerunTestSuite(string scenarioMethodName, string systemMethodName)
    {
        // Arrange
        // =======
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        ScenarioHookOrchestrator orchestrator = new ScenarioHookOrchestratorForConstruction(context);

        var systemHooksMethod = GetAction<SystemHookDefinition>(systemMethodName);
        var scenarioHooksMethod = GetAction<ScenarioHookOrchestrator>(scenarioMethodName);

        // Build system
        orchestrator.DefineSystem(sysConf =>
        {
            systemHooksMethod(sysConf);
        });

        // Build scenario
        orchestrator.DeclareScenarioStart("Scenario recorded by hooks");
        scenarioHooksMethod(orchestrator);
        orchestrator.DeclareScenarioEnd();

        ExpressionEvalator evalator = new(context.System);
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);
        ScenarioTestRunner runner = new(executor, dependencies);


        // Act
        // ===
        ScenarioRun scenarioRun = runner.Run("Scenario recorded by hooks");


        // Assert
        // ======
        string target = scenarioRun.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();

        await Verify(target)
            .UseParameters(scenarioMethodName);

    }

    private Action<T> GetAction<T>(string systemMethodName)
    {
        var methodRef =
            GetType().GetMethod(systemMethodName, BindingFlags.NonPublic | BindingFlags.Static)
                     ?? throw new Exception($"Method {systemMethodName} not found in {GetType().Name}");

        return (T parameter) => methodRef.Invoke(this, [parameter]);
    }
}
