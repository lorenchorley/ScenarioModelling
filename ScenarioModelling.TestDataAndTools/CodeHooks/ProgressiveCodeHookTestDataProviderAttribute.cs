using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CodeHooks.Utils;
using System.Diagnostics;
using System.Reflection;

namespace ScenarioModelling.TestDataAndTools.CodeHooks;

public partial class ProgressiveCodeHookTestDataProviderAttribute : Attribute, ITestDataSource
{
    public static readonly string PrimaryMetaStoryName = "MetaStory recorded by hooks";

    private record TestCase(string MetaStoryMethodName, bool AutoDefineMetaStory = true);

    private List<TestCase> TestData = [
            new(nameof(OneDialog)),
            new(nameof(OneDialogWithMultipleWords)),
            new(nameof(OneDialogWithId)),
            new(nameof(OneDialogWithCharacter)),
            new(nameof(TwoStatesOneTransition)),
            new(nameof(TwoStatesOneTransitionWithId)),
            new(nameof(OneConstraintAlwaysValid)),
            new(nameof(OneConstraintFailsOnTransition)),
            new(nameof(OneDialogAndOneJump)),
            new(nameof(TwoDialogsAndOneJump)),
            new(nameof(IfDoesNotExecute_DialogAfterOnly)),
            new(nameof(IfDoesNotExecute_DialogBeforeAndAfter)),
            new(nameof(IfDoesNotExecute_DialogBeforeOnly)),
            new(nameof(IfDoesNotExecute_NoDialogAround)),
            new(nameof(IfExecutesWithDialog_DialogAfterOnly)),
            new(nameof(IfExecutesWithDialog_DialogBeforeOnly)),
            new(nameof(IfExecutesWithDialog_DialogBeforeAndAfter)),
            new(nameof(IfExecutesWithDialog_NoDialogAround)),
            new(nameof(IfDoesNotExecute_HookOutsideBlock)),
            new(nameof(IfExecutesWithDialog_HookOutsideBlock)),
            new(nameof(IfDoesNotExecute_UsingHook)),
            new(nameof(IfExecutesWithDialog_UsingHook)),
            new(nameof(TwoNestedIfsThatExecute)),
            new(nameof(TwoConsecutiveIfsThatExecute)),
            new(nameof(WhileDoesNotExecute)),
            new(nameof(WhileExecutesOnceWithTransition)),
            new(nameof(WhileExecutesTwiceWithTransition)),
            new(nameof(WhileExecutesTwiceWithTransitionAndDialog)),
            new(nameof(WhileExecutesTwiceWithNestedIf)),
            new(nameof(WhileExecutesTwiceWithNestedIf_NoDialogAfter)),
            new(nameof(CallMetaStory_OneLevel), AutoDefineMetaStory: false),
        ];

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => TestData.Select((Func<TestCase, object[]>)(t => [t.MetaStoryMethodName, GetAssociatedSystemMethodName(t.MetaStoryMethodName), t.AutoDefineMetaStory]));

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data) => data?[0]?.ToString() ?? "";

    [DebuggerNonUserCode]
    public static Action<T> GetAction<T>(string systemMethodName)
    {
        var methodRef =
            typeof(ProgressiveCodeHookTestDataProviderAttribute)
                .GetMethod(systemMethodName, BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new Exception($"Method {systemMethodName} not found in {nameof(ProgressiveCodeHookTestDataProviderAttribute)}");

        return (parameter) => methodRef.Invoke(typeof(ProgressiveCodeHookTestDataProviderAttribute), [parameter]);
    }

    public static string GetExpectedText(string metaStoryMethodName)
    {
        var attr = GetAssociatedExpectedText<ExpectedResultAttribute>(metaStoryMethodName);
        return attr?.Text ?? $"Text not set. To set this text, apply the attribute ExpectedResult to the hook method {metaStoryMethodName}.";
    }

    private static string GetAssociatedSystemMethodName(string metaStoryMethodName)
    {
        var attr = GetAssociatedExpectedText<AssociatedSystemHookMethodAttribute>(metaStoryMethodName);
        return attr?.MethodName ?? $"Method name not set. To set this name, apply the attribute AssociatedSystemHookMethod to the hook method {metaStoryMethodName}.";
    }

    private static T? GetAssociatedExpectedText<T>(string metaStoryMethodName) where T : Attribute
    {
        var methodRef =
            typeof(ProgressiveCodeHookTestDataProviderAttribute)
                .GetMethod(metaStoryMethodName, BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new Exception($"Method {metaStoryMethodName} not found in {nameof(ProgressiveCodeHookTestDataProviderAttribute)}");

        return methodRef.GetCustomAttribute<T>();
    }

    #region Systems
    [ExpectedResult(
    """
    
    """)]
    private static void EmptySystem(MetaStateHookDefinition sysConf)
    {
    }

    [ExpectedResult(
    """
    Entity Actor {
    }
    """)]
    private static void SystemOneActor(MetaStateHookDefinition sysConf)
    {
        sysConf.Entity("Actor");

        // TODO new test on SetType
    }

    [ExpectedResult(
    """
    Entity Actor {
      State S1
    }

    StateMachine SM1 {
      State S1
      State S2
      S1 -> S2 : T1
    }
    """)]
    private static void SystemOneActorTwoStates(MetaStateHookDefinition sysConf)
    {
        sysConf.Entity("Actor")
               .SetState("S1");

        sysConf.StateMachine("SM1")
               .WithState("S1")
               .WithState("S2")
               .WithTransition("S1", "S2", "T1");
    }

    [ExpectedResult(
    """
    Entity Actor {
      State S1
    }

    StateMachine SM1 {
      State S1
      State S2
      S1 -> S2 : T1
    }

    Constraint <Actor.State != S2> {
      Description "State must never be S2"
    }
    """)]
    private static void SystemOneActorTwoStatesWithConstraint(MetaStateHookDefinition sysConf)
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

    [ExpectedResult(
    """
    Entity Actor {
      State S1
    }

    StateMachine SM1 {
      State S1
      State S2
      State S3
      S1 -> S2 : T1
      S2 -> S3 : T1
    }
    """)]
    private static void SystemOneActorThreeStatesSingleTransition(MetaStateHookDefinition sysConf)
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

    #region Metadata
    [ExpectedResult(
    """
    Metadata {
    }
    """)]
    [AssociatedSystemHookMethod(nameof(EmptySystem))]
    private static void EmptyMetadata(MetaStoryHookOrchestrator hooks)
    {
        hooks.Metadata("")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    Metadata {
        Value "Some value"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(EmptySystem))]
    private static void MetadataWithValue(MetaStoryHookOrchestrator hooks)
    {
        hooks.Metadata("Some value")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    Metadata "A key" {
    }
    """)]
    [AssociatedSystemHookMethod(nameof(EmptySystem))]
    private static void MetadataWithKey(MetaStoryHookOrchestrator hooks)
    {
        hooks.Metadata("")
             .WithKey("A key")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    Metadata "A key" {
        Value "Some value"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(EmptySystem))]
    private static void MetadataWithKeyAndValue(MetaStoryHookOrchestrator hooks)
    {
        hooks.Metadata("Some value")
             .WithKey("A key")
             .BuildAndRegister();
    }
    #endregion

    #region Dialog
    [ExpectedResult(
    """
    Dialog {
      Text Hello
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActor))]
    private static void OneDialog(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Hello")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    Dialog {
      Text "Hello with multiple words"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActor))]
    private static void OneDialogWithMultipleWords(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Hello with multiple words")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    Dialog "custom dialog id" {
      Text Hello
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActor))]
    private static void OneDialogWithId(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Hello")
             .WithId("custom dialog id")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    Dialog {
      Text Hello
      Character Actor
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActor))]
    private static void OneDialogWithCharacter(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Hello")
             .WithCharacter("Actor")
             .BuildAndRegister();
    }
    #endregion

    #region Transition
    [ExpectedResult(
    """
    Transition {
      Actor : T1
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
    private static void TwoStatesOneTransition(MetaStoryHookOrchestrator hooks)
    {
        hooks.Transition("Actor", "T1")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    Transition "custom transition id" {
      Actor : T1
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
    private static void TwoStatesOneTransitionWithId(MetaStoryHookOrchestrator hooks)
    {
        hooks.Transition("Actor", "T1")
             .SetId("custom transition id")
             .BuildAndRegister();
    }
    #endregion

    #region Jump
    [ExpectedResult(
    """
    Jump {
      Target D1
    }
    Dialog D1 {
      Text "Some text"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(EmptySystem))]
    private static void OneDialogAndOneJump(MetaStoryHookOrchestrator hooks)
    {
        hooks.Jump("D1")
             .BuildAndRegister();

        hooks.Dialog("Some text")
             .WithId("D1")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    Jump {
      Target D2
    }
    Dialog D1 {
      Text "Some text"
    }
    Dialog D2 {
      Text "Some more text"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(EmptySystem))]
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
    [ExpectedResult(
    """
    Dialog D1 {
      Text "Some text"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStatesWithConstraint))]
    private static void OneConstraintAlwaysValid(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Some text")
             .WithId("D1")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    Dialog D1 {
      Text "Some text"
    }
    Transition {
      Actor : T1
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStatesWithConstraint))]
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
    [ExpectedResult(
    """
    If <Actor.State == S2> {
    }
    Dialog {
      Text "After if block only"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
    private static void IfDoesNotExecute_DialogAfterOnly(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State == S2")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = false;
        if (φ(condition))
        {
            // This part is not executed so it should not appear in the meta story
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }

        hooks.Dialog("After if block only")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    Dialog {
      Text "Before if block only"
    }
    If <Actor.State == S2> {
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
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
            // This part is not executed so it should not appear in the meta story
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }
    }

    [ExpectedResult(
    """
    Dialog {
      Text "Before if block"
    }
    If <Actor.State == S2> {
    }
    Dialog {
      Text "After if block"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
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
            // This part is not executed so it should not appear in the meta story
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }

        hooks.Dialog("After if block")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    If <Actor.State == S2> {
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
    private static void IfDoesNotExecute_NoDialogAround(MetaStoryHookOrchestrator hooks)
    {
        hooks.If(@"Actor.State == S2")
             .GetConditionHook(out BifurcatingHook φ)
             .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
             .Build();

        bool condition = false;
        if (φ(condition))
        {
            // This part is not executed so it should not appear in the meta story
            hooks.Dialog("Inside naked if block")
                 .BuildAndRegister();

            ifBlockEndHook();
        }
    }

    [ExpectedResult(
    """
    Dialog {
      Text "Before if block only"
    }
    If <Actor.State == S1> {
      Dialog {
        Text "Inside if block"
      }
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
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

    [ExpectedResult(
    """
    If <Actor.State == S1> {
      Dialog {
        Text "Inside if block"
      }
    }
    Dialog {
      Text "After if block only"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
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

    [ExpectedResult(
    """
    Dialog {
      Text "Before if block"
    }
    If <Actor.State == S1> {
      Dialog {
        Text "Inside if block"
      }
    }
    Dialog {
      Text "After if block"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
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

    [ExpectedResult(
    """
    If <Actor.State == S1> {
      Dialog {
        Text "Inside naked if block"
      }
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
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

    [ExpectedResult(
    """
    If <Actor.State == S2> {
    }
    Dialog {
      Text "After if block"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
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

    [ExpectedResult(
    """
    If <Actor.State == S1> {
      Dialog {
        Text "Inside if block"
      }
    }
    Dialog {
      Text "After if block"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
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

    [ExpectedResult(
    """
    If <Actor.State == S2> {
    }
    Dialog {
      Text "After if block"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
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

    [ExpectedResult(
    """
    If <Actor.State == S1> {
      Dialog {
        Text "Inside if block"
      }
    }
    Dialog {
      Text "After if block"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
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

    [ExpectedResult(
    """
    If <Actor.State != S2> {
      If <Actor.State != S3> {
        Dialog {
          Text "Inside if block"
        }
      }
    }
    Dialog {
      Text "After if blocks"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorThreeStatesSingleTransition))]
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

    [ExpectedResult(
    """
    If <Actor.State != S2> {
      Dialog {
        Text "Inside if first block"
      }
    }
    If <Actor.State != S3> {
      Dialog {
        Text "Inside if second block"
      }
    }
    Dialog {
      Text "After if blocks"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorThreeStatesSingleTransition))]
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
    [ExpectedResult(
    """
    While <Actor.State != S1> {
    }
    Dialog {
      Text "After while block"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorThreeStatesSingleTransition))]
    private static void WhileDoesNotExecute(MetaStoryHookOrchestrator hooks)
    {
        hooks.While(@"Actor.State != S1")
             .GetConditionHook(out BifurcatingHook φ)
             .Build();

        int count = 0;
        while (φ(count > 0))
        {
            // This part is not executed so it should not appear in the meta story
            hooks.Transition("Actor", "T1")
                 .BuildAndRegister();

            count--;
        }

        hooks.Dialog("After while block")
             .BuildAndRegister();
    }

    [ExpectedResult(
    """
    While <Actor.State != S2> {
      Transition {
        Actor : T1
      }
    }
    Dialog {
      Text "After while block"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorThreeStatesSingleTransition))]
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

    [ExpectedResult(
    """
    While <Actor.State != S3> {
      Transition {
        Actor : T1
      }
    }
    Dialog {
      Text "After while block"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorThreeStatesSingleTransition))]
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

    [ExpectedResult(
    """
    While <Actor.State != S3> {
      Transition {
        Actor : T1
      }
      Dialog {
        Text "Doing T1"
      }
    }
    Dialog {
      Text "After while block"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorThreeStatesSingleTransition))]
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

    [ExpectedResult(
    """
    While <Actor.State != S3> {
      Transition {
        Actor : T1
      }
      If <Actor.State != S3> {
        Dialog {
          Text "Some text"
        }
      }
    }
    Dialog {
      Text "After while block"
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorThreeStatesSingleTransition))]
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

    [ExpectedResult(
    """
    While <Actor.State != S3> {
      Transition {
        Actor : T1
      }
      If <Actor.State != S3> {
        Dialog {
          Text "Some text"
        }
      }
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorThreeStatesSingleTransition))]
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

    #region CallMetaStory

    [ExpectedResult(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Before call"
      }
      CallMetaStory {
        MetaStoryName "Secondary MetaStory"
      }
      Dialog {
        Text "After call"
      }
    }

    MetaStory "Secondary MetaStory" {
      Dialog {
        Text "Inside the inner meta story"
      }
    }
    """)]
    [AssociatedSystemHookMethod(nameof(SystemOneActorTwoStates))]
    private static void CallMetaStory_OneLevel(MetaStoryHookOrchestrator hooks)
    {
        hooks.Dialog("Before call").BuildAndRegister();

        hooks.CallMetaStory("Secondary MetaStory").BuildAndRegister();

        { // Scope to help understand the inner meta story
            hooks.StartMetaStory("Secondary MetaStory");

            hooks.Dialog("Inside the inner meta story").BuildAndRegister();

            hooks.EndMetaStory();
        }

        hooks.Dialog("After call").BuildAndRegister();
    }
    
    #endregion
}
