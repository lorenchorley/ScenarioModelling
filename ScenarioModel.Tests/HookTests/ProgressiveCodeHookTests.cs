using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Expressions.Evaluation;
using ScenarioModelling.Interpolation;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.Tests.Stories;
using System.Diagnostics;
using System.Reflection;

namespace ScenarioModelling.Tests.HookTests;

[TestClass]
[UsesVerify]
public partial class ProgressiveCodeHookTests
{
    private record TestCase(string MetaStoryMethodName, string SystemMethodName);
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
            new(nameof(TwoDialogsAndOneJump), nameof(SystemEmpty)),

            new(nameof(OneConstraintAlwaysValid), nameof(SystemOneActorTwoStatesWithConstraint)),
            new(nameof(OneConstraintFailsOnTransition), nameof(SystemOneActorTwoStatesWithConstraint)),

            new(nameof(IfDoesNotExecute_DialogAfterOnly), nameof(SystemOneActorTwoStates)),
            new(nameof(IfDoesNotExecute_DialogBeforeAndAfter), nameof(SystemOneActorTwoStates)),
            new(nameof(IfDoesNotExecute_DialogBeforeOnly), nameof(SystemOneActorTwoStates)),
            new(nameof(IfDoesNotExecute_NoDialogAround), nameof(SystemOneActorTwoStates)),
            new(nameof(IfExecutesWithDialog_DialogAfterOnly), nameof(SystemOneActorTwoStates)),
            new(nameof(IfExecutesWithDialog_DialogBeforeAndAfter), nameof(SystemOneActorTwoStates)),
            new(nameof(IfExecutesWithDialog_DialogBeforeOnly), nameof(SystemOneActorTwoStates)),
            new(nameof(IfExecutesWithDialog_NoDialogAround), nameof(SystemOneActorTwoStates)),
            new(nameof(IfDoesNotExecute_HookOutsideBlock), nameof(SystemOneActorTwoStates)),
            new(nameof(IfExecutesWithDialog_HookOutsideBlock), nameof(SystemOneActorTwoStates)),
            new(nameof(IfDoesNotExecute_UsingHook), nameof(SystemOneActorTwoStates)),
            new(nameof(IfExecutesWithDialog_UsingHook), nameof(SystemOneActorTwoStates)),
            new(nameof(TwoNestedIfsThatExecute), nameof(SystemOneActorThreeStatesSingleTransition)),
            new(nameof(TwoConsecutiveIfsThatExecute), nameof(SystemOneActorThreeStatesSingleTransition)),

            new(nameof(WhileDoesNotExecute), nameof(SystemOneActorThreeStatesSingleTransition)),
            new(nameof(WhileExecutesOnceWithTransition), nameof(SystemOneActorThreeStatesSingleTransition)),
            new(nameof(WhileExecutesTwiceWithTransition), nameof(SystemOneActorThreeStatesSingleTransition)),
            new(nameof(WhileExecutesTwiceWithTransitionAndDialog), nameof(SystemOneActorThreeStatesSingleTransition)),
            new(nameof(WhileExecutesTwiceWithNestedIf), nameof(SystemOneActorThreeStatesSingleTransition)),
            new(nameof(WhileExecutesTwiceWithNestedIf_NoDialogAfter), nameof(SystemOneActorThreeStatesSingleTransition)),
        ];

        public IEnumerable<object[]> GetData(MethodInfo methodInfo) => TestData.Select((Func<TestCase, object[]>)(t => [t.MetaStoryMethodName, t.SystemMethodName]));
        public string GetDisplayName(MethodInfo methodInfo, object?[]? data) => data?[0]?.ToString() ?? "";
    }

    #region Systems
    private static void SystemEmpty(SystemHookDefinition sysConf)
    {
    }

    private static void SystemOneActor(SystemHookDefinition sysConf)
    {
        sysConf.Entity("Actor");

        // TODO new test on SetType
    }

    private static void SystemOneActorTwoStates(SystemHookDefinition sysConf)
    {
        sysConf.Entity("Actor")
               .SetState("S1");

        sysConf.StateMachine("SM1")
               .WithState("S1")
               .WithState("S2")
               .WithTransition("S1", "S2", "T1");
    }

    private static void SystemOneActorTwoStatesWithConstraint(SystemHookDefinition sysConf)
    {
        sysConf.Entity("Actor")
               .SetState("S1");

        sysConf.StateMachine("SM1")
               .WithState("S1")
               .WithState("S2")
               .WithTransition("S1", "S2", "T1");

        sysConf.DefineConstraint("State must never be S2")
               .SetExpression("Actor.State != S2");
    }

    private static void SystemOneActorThreeStatesSingleTransition(SystemHookDefinition sysConf)
    {
        sysConf.Entity("Actor")
               .SetState("S1");

        sysConf.StateMachine("SM1")
               .WithState("S1")
               .WithState("S2")
               .WithState("S3")
               .WithTransition("S1", "S2", "T1")
               .WithTransition("S2", "S3", "T1");
    }
    #endregion

    #region Dialog
    private static void OneDialog(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Hello")
             .BuildAndRegister();
    }

    private static void OneDialogWithMultipleWords(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Hello with multiple words")
             .BuildAndRegister();
    }

    private static void OneDialogWithId(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Hello")
             .WithId("custom dialog id")
             .BuildAndRegister();
    }

    private static void OneDialogWithCharacter(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Hello")
             .WithCharacter("Actor")
             .BuildAndRegister();
    }
    #endregion

    #region Transition
    private static void TwoStatesOneTransition(MetaStoryHookOrchestrator hooks)
    {
        hooks.Transition("Actor", "T1")
             .BuildAndRegister();
    }

    private static void TwoStatesOneTransitionWithId(MetaStoryHookOrchestrator hooks)
    {
        hooks.Transition("Actor", "T1")
             .SetId("custom transition id")
             .BuildAndRegister();
    }
    #endregion

    #region Jump
    private static void OneDialogAndOneJump(MetaStoryHookOrchestrator hooks)
    {
        hooks.Jump("D1")
             .BuildAndRegister();

        hooks.Dialog("Some text")
             .WithId("D1")
             .BuildAndRegister();
    }

    private static void TwoDialogsAndOneJump(MetaStoryHookOrchestrator hooks)
    {
        hooks.Jump("D2")
             .BuildAndRegister();

        hooks.Dialog("Some text")
             .WithId("D1")
             .BuildAndRegister();

        hooks.Dialog("Some more text")
             .WithId("D2")
             .BuildAndRegister();
    }

    #endregion

    #region Choose
    // TODO
    #endregion

    #region Constraint
    private static void OneConstraintAlwaysValid(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Some text")
             .WithId("D1")
             .BuildAndRegister();
    }

    private static void OneConstraintFailsOnTransition(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Some text")
             .WithId("D1")
             .BuildAndRegister();

        hooks.Transition("Actor", "T1")
             .BuildAndRegister();
    }

    // TODO more constraints

    #endregion

    #region If
    private static void IfDoesNotExecute_DialogAfterOnly(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State == S2")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = false;
        if (φ(condition))
        {
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }

        hooks.Dialog("After if block only")
             .BuildAndRegister();
    }

    private static void IfDoesNotExecute_DialogBeforeOnly(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Before if block only")
             .BuildAndRegister();

        hooks.If(@"Actor.State == S2")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = false;
        if (φ(condition))
        {
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }
    }

    private static void IfDoesNotExecute_DialogBeforeAndAfter(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Before if block")
             .BuildAndRegister();

        hooks.If(@"Actor.State == S2")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = false;
        if (φ(condition))
        {
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }

        hooks.Dialog("After if block")
             .BuildAndRegister();
    }

    private static void IfDoesNotExecute_NoDialogAround(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State == S2")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = false;
        if (φ(condition))
        {
            hooks.Dialog("Inside naked if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }
    }

    private static void IfExecutesWithDialog_DialogBeforeOnly(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Before if block only")
             .BuildAndRegister();

        hooks.If(@"Actor.State == S1")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = true;
        if (φ(condition))
        {
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }
    }

    private static void IfExecutesWithDialog_DialogAfterOnly(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State == S1")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = true;
        if (φ(condition))
        {
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }

        hooks.Dialog("After if block only")
             .BuildAndRegister();
    }

    private static void IfExecutesWithDialog_DialogBeforeAndAfter(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Before if block")
             .BuildAndRegister();

        hooks.If(@"Actor.State == S1")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = true;
        if (φ(condition))
        {
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }

        hooks.Dialog("After if block")
             .BuildAndRegister();
    }

    private static void IfExecutesWithDialog_NoDialogAround(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State == S1")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = true;
        if (φ(condition))
        {
            hooks.Dialog("Inside naked if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }
    }

    private static void IfDoesNotExecute_HookOutsideBlock(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State == S2")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = false;
        if (φ(condition))
        {
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();
        }
        ifBlockEndHook();

        hooks.Dialog("After if block")
             .BuildAndRegister();
    }

    private static void IfExecutesWithDialog_HookOutsideBlock(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State == S1")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = true;
        if (φ(condition))
        {
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();
        }
        ifBlockEndHook();

        hooks.Dialog("After if block")
             .BuildAndRegister();
    }

    private static void IfDoesNotExecute_UsingHook(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State == S2")
             .GetConditionHook(out BifurcatingHook φ)
             .GetScopeHook(out ScopeDefiningHook ifBlockUsingHook)
             .Build();

        bool condition = false;
        using (ifBlockUsingHook())
        {
            if (φ(condition))
            {
                hooks.Dialog("Inside if block")
                     .BuildAndRegister();
            }
        }

        hooks.Dialog("After if block")
             .BuildAndRegister();
    }

    private static void IfExecutesWithDialog_UsingHook(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State == S1")
             .GetConditionHook(out BifurcatingHook φ)
             .GetScopeHook(out ScopeDefiningHook ifBlockUsingHook)
             .Build();

        bool condition = true;
        using (ifBlockUsingHook())
        {
            if (φ(condition))
            {
                hooks.Dialog("Inside if block")
                     .BuildAndRegister();
            }
        }

        hooks.Dialog("After if block")
             .BuildAndRegister();
    }

    private static void TwoNestedIfsThatExecute(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State != S2")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook1)
             .Build();

        bool condition = true;
        if (φ(condition))
        {
            hooks.If(@"Actor.State != S3")
                 .GetConditionHook(out BifurcatingHook ψ)
                 .GetEndBlockHook(out BlockEndHook ifBlockEndHook2)
                 .Build();

            if (ψ(condition))
            {
                hooks.Dialog("Inside if block")
                     .BuildAndRegister();

                ifBlockEndHook2();
            }

            ifBlockEndHook1();
        }

        hooks.Dialog("After if blocks")
             .BuildAndRegister();
    }

    private static void TwoConsecutiveIfsThatExecute(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State != S2")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook1)
             .Build();

        bool condition = true;
        if (φ(condition))
        {
            hooks.Dialog("Inside if first block")
                 .BuildAndRegister();

            ifBlockEndHook1();
        }

        hooks.If(@"Actor.State != S3")
             .GetConditionHook(out BifurcatingHook ψ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook2)
             .Build();

        if (ψ(condition))
        {
            hooks.Dialog("Inside if second block")
                 .BuildAndRegister();

            ifBlockEndHook2();
        }

        hooks.Dialog("After if blocks")
             .BuildAndRegister();
    }

    // TODO Combining MetaStories (new test class maybe)

    // if (true)
    //   first dialog
    // if (false)

    // if (false)
    // if (true)
    //   second dialog

    // Should give :
    // if (condition1)
    //   first dialog
    // if (condition2)
    //   second dialog

    #endregion

    #region While
    private static void WhileDoesNotExecute(MetaStoryHookOrchestrator hooks)
    {
        hooks.While(@"Actor.State != S1")
             .GetConditionHook(out BifurcatingHook φ)
             .Build();

        int count = 0;
        while (φ(count > 0))
        {
            hooks.Transition("Actor", "T1")
                 .BuildAndRegister();

            count--;
        }

        hooks.Dialog("After while block")
             .BuildAndRegister();
    }

    private static void WhileExecutesOnceWithTransition(MetaStoryHookOrchestrator hooks)
    {
        hooks.While(@"Actor.State != S2")
             .GetConditionHook(out BifurcatingHook φ)
             .Build();

        int count = 1;
        while (φ(count > 0))
        {
            hooks.Transition("Actor", "T1")
                 .BuildAndRegister();

            count--;
        }

        hooks.Dialog("After while block")
             .BuildAndRegister();
    }

    private static void WhileExecutesTwiceWithTransition(MetaStoryHookOrchestrator hooks)
    {
        hooks.While(@"Actor.State != S3")
             .GetConditionHook(out BifurcatingHook φ)
             .Build();

        int count = 2;
        while (φ(count > 0))
        {
            hooks.Transition("Actor", "T1")
                 .BuildAndRegister();

            count--;
        }

        hooks.Dialog("After while block")
             .BuildAndRegister();
    }

    private static void WhileExecutesTwiceWithTransitionAndDialog(MetaStoryHookOrchestrator hooks)
    {
        hooks.While(@"Actor.State != S3")
             .GetConditionHook(out BifurcatingHook φ)
             .Build();

        int count = 2;
        while (φ(count > 0))
        {
            hooks.Transition("Actor", "T1")
                 .BuildAndRegister();

            hooks.Dialog("Doing T1")
                 .BuildAndRegister();

            count--;
        }

        hooks.Dialog("After while block")
             .BuildAndRegister();
    }

    private static void WhileExecutesTwiceWithNestedIf(MetaStoryHookOrchestrator hooks)
    {
        hooks.While(@"Actor.State != S3")
             .GetConditionHook(out BifurcatingHook φ)
             .Build();

        int count = 2;
        while (φ(count > 0))
        {
            hooks.Transition("Actor", "T1")
                 .BuildAndRegister();

            hooks.If(@"Actor.State != S3")
                 .GetConditionHook(out BifurcatingHook ψ)
                 .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
                 .Build();

            bool condition = true;
            if (ψ(condition))
            {
                hooks.Dialog("Some text")
                     .BuildAndRegister();

                ifBlockEndHook();
            }

            count--;
        }

        hooks.Dialog("After while block")
             .BuildAndRegister();
    }

    private static void WhileExecutesTwiceWithNestedIf_NoDialogAfter(MetaStoryHookOrchestrator hooks)
    {
        hooks.While(@"Actor.State != S3")
             .GetConditionHook(out BifurcatingHook φ)
             .Build();

        int count = 2;
        while (φ(count > 0))
        {
            hooks.Transition("Actor", "T1")
                 .BuildAndRegister();

            hooks.If(@"Actor.State != S3")
                 .GetConditionHook(out BifurcatingHook ψ)
                 .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
                 .Build();

            bool condition = true;
            if (ψ(condition))
            {
                hooks.Dialog("Some text")
                     .BuildAndRegister();

                ifBlockEndHook();
            }

            count--;
        }
    }
    #endregion

    [DataTestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory Construction")]
    [ProgressiveCodeHookTestDataProvider]
    public async Task ProgressiveDevelopment_CodeHooks_metaStoryConstructionTests(string MetaStoryMethodName, string systemMethodName)
    {
        // Arrange
        // =======
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestrator orchestrator = new MetaStoryHookOrchestratorForConstruction(context);

        var systemHooksMethod = GetAction<SystemHookDefinition>(systemMethodName);
        var MetaStoryHooksMethod = GetAction<MetaStoryHookOrchestrator>(MetaStoryMethodName);


        // Act
        // ===

        // Build system
        orchestrator.DefineSystem(systemHooksMethod);

        // Build MetaStory
        orchestrator.StartMetaStory("MetaStory recorded by hooks");
        MetaStoryHooksMethod(orchestrator);
        orchestrator.EndMetaStory();


        // Assert
        // ======
        var serialisedContext =
            context.Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        await Verify(serialisedContext)
            .UseParameters(MetaStoryMethodName);
    }

    [DataTestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory -> Story")]
    [ProgressiveCodeHookTestDataProvider]
    public async Task ProgressiveDevelopment_CodeHooks_StoryExtractionTests(string MetaStoryMethodName, string systemMethodName)
    {
        // Arrange
        // =======
        var context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestrator orchestrator = new MetaStoryHookOrchestratorForConstruction(context);

        var systemHooksMethod = GetAction<SystemHookDefinition>(systemMethodName);
        var MetaStoryHooksMethod = GetAction<MetaStoryHookOrchestrator>(MetaStoryMethodName);

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
            .UseParameters(MetaStoryMethodName);

    }

    [DebuggerNonUserCode]
    private Action<T> GetAction<T>(string systemMethodName)
    {
        var methodRef =
            GetType().GetMethod(systemMethodName, BindingFlags.NonPublic | BindingFlags.Static)
                     ?? throw new Exception($"Method {systemMethodName} not found in {GetType().Name}");

        return (T parameter) => methodRef.Invoke(this, [parameter]);
    }
}
