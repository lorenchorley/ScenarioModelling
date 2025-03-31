using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions.MetaStateObjects;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.TestDataAndTools.Attributes;

namespace ScenarioModelling.TestDataAndTools.CodeHooks;

public class CodeHookTestData
{
    #region Systems
    [ExpectedSerialisedForm(
    """
    
    """)]
    public static void EmptyMetaState(MetaStateHookDefinition sysConf)
    {
    }

    [ExpectedSerialisedForm(
    """
    Entity Actor {
    }
    """)]
    public static void MetaStateOneActor(MetaStateHookDefinition sysConf)
    {
        sysConf.Entity("Actor");

        // TODO new test on SetType
    }

    [ExpectedSerialisedForm(
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
    public static void MetaStateOneActorTwoStates(MetaStateHookDefinition sysConf)
    {
        sysConf.Entity("Actor")
               .SetState("S1");

        sysConf.StateMachine("SM1")
               .WithState("S1")
               .WithState("S2")
               .WithTransition("S1", "S2", "T1");
    }

    [ExpectedSerialisedForm(
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
    public static void MetaStateOneActorThreeStates(MetaStateHookDefinition sysConf)
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

    [ExpectedSerialisedForm(
    """
    Entity Actor {
      State S1
      Aspect Name {
        State A1
      }
    }

    StateMachine SM1 {
      State S1
      State S2
      S1 -> S2 : T1
    }

    StateMachine SM2 {
      State A1
      State A2
      A1 -> A2 : T2
    }
    """)]
    public static void MetaStateOneActorWithAspectTwoStateMachines(MetaStateHookDefinition config)
    {
        config.Entity("Actor")
              .SetState("S1")
              .WithAspect("Name", aspect =>
                  aspect.SetState("A1")
              );

        config.StateMachine("SM1")
              .WithState("S1")
              .WithState("S2")
              .WithTransition("S1", "S2", "T1");

        config.StateMachine("SM2")
              .WithState("A1")
              .WithState("A2")
              .WithTransition("A1", "A2", "T2");
    }

    [ExpectedSerialisedForm(
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
    public static void MetaStateOneActorTwoStatesWithConstraint(MetaStateHookDefinition config)
    {
        config.Entity("Actor")
              .SetState("S1");

        config.StateMachine("SM1")
              .WithState("S1")
              .WithState("S2")
              .WithTransition("S1", "S2", "T1");

        config.DefineConstraint("State must never be S2")
              .SetExpression("Actor.State != S2");
    }

    [ExpectedSerialisedForm(
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
    public static void MetaStateOneActorThreeStatesSingleTransition(MetaStateHookDefinition sysConf)
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
    [ExpectedSerialisedForm(
    """
    Metadata {
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(EmptyMetaState))]
    public static void EmptyMetadata(HookOrchestrator hooks)
    {
        hooks.Metadata("")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
    """
    Metadata {
        Value "Some value"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(EmptyMetaState))]
    public static void MetadataWithValue(HookOrchestrator hooks)
    {
        hooks.Metadata("Some value")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
    """
    Metadata "A key" {
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(EmptyMetaState))]
    public static void MetadataWithKey(HookOrchestrator hooks)
    {
        hooks.Metadata("")
             .WithKey("A key")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
    """
    Metadata "A key" {
        Value "Some value"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(EmptyMetaState))]
    public static void MetadataWithKeyAndValue(HookOrchestrator hooks)
    {
        hooks.Metadata("Some value")
             .WithKey("A key")
             .BuildAndRegister();
    }
    #endregion

    #region Dialog
    [ExpectedSerialisedForm(
    """
    Assert <Actor.State == S1>
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void Assert_OneState_Succeeds(HookOrchestrator hooks)
    {
        hooks.Assert("Actor.State == S1")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
    """
    Transition {
      Actor : T1
    }
    Assert <Actor.State == S1>
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void Assert_Transition_Fails(HookOrchestrator hooks)
    {
        hooks.Transition("Actor", "T1")
             .BuildAndRegister();

        hooks.Assert("Actor.State == S1")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
    """
    Transition {
      Actor : T1
    }
    Assert "Actor is in first state" <Actor.State == S1>
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void Assert_WithName_Transition_Fails(HookOrchestrator hooks)
    {
        hooks.Transition("Actor", "T1")
             .BuildAndRegister();

        hooks.Assert("Actor is in first state", "Actor.State == S1")
             .BuildAndRegister();
    }
    #endregion

    #region Dialog
    [ExpectedSerialisedForm(
    """
    Dialog {
      Text Hello
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActor))]
    public static void OneDialog(HookOrchestrator hooks)
    {
        hooks.Dialog("Hello")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
    """
    Dialog {
      Text "Hello with multiple words"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActor))]
    public static void OneDialogWithMultipleWords(HookOrchestrator hooks)
    {
        hooks.Dialog("Hello with multiple words")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
    """
    Dialog "custom dialog id" {
      Text Hello
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActor))]
    public static void OneDialogWithId(HookOrchestrator hooks)
    {
        hooks.Dialog("Hello")
             .WithId("custom dialog id")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
    """
    Dialog {
      Text Hello
      Character Actor
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActor))]
    public static void OneDialogWithCharacter(HookOrchestrator hooks)
    {
        hooks.Dialog("Hello")
             .WithCharacter("Actor")
             .BuildAndRegister();
    }
    #endregion

    #region Transition
    [ExpectedSerialisedForm(
    """
    Transition {
      Actor : T1
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void TwoStatesOneTransition(HookOrchestrator hooks)
    {
        hooks.Transition("Actor", "T1")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
    """
    Transition "custom transition id" {
      Actor : T1
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void TwoStatesOneTransitionWithId(HookOrchestrator hooks)
    {
        hooks.Transition("Actor", "T1")
             .SetId("custom transition id")
             .BuildAndRegister();
    }
    #endregion

    #region Jump
    [ExpectedSerialisedForm(
    """
    Jump {
      Target D1
    }
    Dialog D1 {
      Text "Some text"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(EmptyMetaState))]
    public static void OneDialogAndOneJump(HookOrchestrator hooks)
    {
        hooks.Jump("D1")
             .BuildAndRegister();

        hooks.Dialog("Some text")
             .WithId("D1")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(EmptyMetaState))]
    public static void TwoDialogsAndOneJump(HookOrchestrator hooks)
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

    #region Loop
    [ExpectedSerialisedForm(
    """
    Loop {
    }
    Dialog {
      Text "After loop block"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStatesSingleTransition))]
    public static void LoopDoesNotExecute(HookOrchestrator hooks)
    {
        hooks.Loop()
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

        hooks.Dialog("After loop block")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
    """
    Loop {
      Transition {
        Actor : T1
      }
    }
    Dialog {
      Text "After loop block"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStatesSingleTransition))]
    public static void LoopExecutesOnce(HookOrchestrator hooks)
    {
        // TODO How to set the number of loops to do since it's non-deterministic from just the model
        // It's a behaviour to program into the story runner

        hooks.Loop()
             .GetConditionHook(out BifurcatingHook φ)
             .Build();

        int count = 1;
        while (φ(count > 0))
        {
            // This part is not executed so it should not appear in the meta story
            hooks.Transition("Actor", "T1")
                 .BuildAndRegister();

            count--;
        }

        hooks.Dialog("After loop block")
             .BuildAndRegister();
    }

    #endregion

    #region Choose
    // TODO
    #endregion

    #region Constraint
    [ExpectedSerialisedForm(
    """
    Dialog D1 {
      Text "Some text"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStatesWithConstraint))]
    public static void OneConstraintAlwaysValid(HookOrchestrator hooks)
    {
        hooks.Dialog("Some text")
             .WithId("D1")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
    """
    Dialog D1 {
      Text "Some text"
    }
    Transition {
      Actor : T1
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStatesWithConstraint))]
    public static void OneConstraintFailsOnTransition(HookOrchestrator hooks)
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
    [ExpectedSerialisedForm(
    """
    If <Actor.State == S2> {
    }
    Dialog {
      Text "After if block only"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfDoesNotExecute_DialogAfterOnly(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
    """
    Dialog {
      Text "Before if block only"
    }
    If <Actor.State == S2> {
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfDoesNotExecute_DialogBeforeOnly(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfDoesNotExecute_DialogBeforeAndAfter(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
    """
    If <Actor.State == S2> {
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfDoesNotExecute_NoDialogAround(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfExecutesWithDialog_DialogBeforeOnly(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfExecutesWithDialog_DialogAfterOnly(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfExecutesWithDialog_DialogBeforeAndAfter(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
    """
    If <Actor.State == S1> {
      Dialog {
        Text "Inside naked if block"
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfExecutesWithDialog_NoDialogAround(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
    """
    If <Actor.State == S2> {
    }
    Dialog {
      Text "After if block"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfDoesNotExecute_HookOutsideBlock(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfExecutesWithDialog_HookOutsideBlock(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
    """
    If <Actor.State == S2> {
    }
    Dialog {
      Text "After if block"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfDoesNotExecute_UsingHook(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void IfExecutesWithDialog_UsingHook(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
    """
    If <Actor.Name.State == A1> {
      Dialog {
        Text "Inside first if block"
      }
    }
    Dialog {
      Text "Between if blocks 1"
    }
    If <Actor.Name.State == A2> {
    }
    Dialog {
      Text "Between if blocks 2"
    }
    Transition {
      Actor.Name : T2
    }
    If <Actor.Name.State == A2> {
      Dialog {
        Text "Inside third if block"
      }
    }
    Dialog {
      Text "After if block"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorWithAspectTwoStateMachines))]
    public static void IfExecutes_ExpressionUsingAspectState(HookOrchestrator hooks)
    {
        string aspectState = "A1";

        hooks.If(@"Actor.Name.State == A1")
             .GetConditionHook(out BifurcatingHook φ)
             .GetScopeHook(out ScopeDefiningHook ifBlockUsingHook)
             .Build();

        using (ifBlockUsingHook())
        {
            if (φ(aspectState.Equals("A1")))
            {
                hooks.Dialog("Inside first if block")
                     .BuildAndRegister();
            }
        }

        hooks.Dialog("Between if blocks 1")
             .BuildAndRegister();

        hooks.If(@"Actor.Name.State == A2")
             .GetConditionHook(out BifurcatingHook ψ)
             .GetScopeHook(out ifBlockUsingHook)
             .Build();

        using (ifBlockUsingHook())
        {
            if (ψ(aspectState.Equals("A2")))
            {
                hooks.Dialog("Inside second if block") // Never run so shouldn't appear in any of the results
                     .BuildAndRegister();
            }
        }

        hooks.Dialog("Between if blocks 2")
             .BuildAndRegister();


        aspectState = "A2";

        hooks.Transition("Actor.Name", "T2")
             .BuildAndRegister();


        hooks.If(@"Actor.Name.State == A2")
             .GetConditionHook(out BifurcatingHook Θ)
             .GetScopeHook(out ifBlockUsingHook)
             .Build();

        using (ifBlockUsingHook())
        {
            if (Θ(aspectState.Equals("A2")))
            {
                hooks.Dialog("Inside third if block")
                     .BuildAndRegister();
            }
        }

        hooks.Dialog("After if block")
             .BuildAndRegister();
    }

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStatesSingleTransition))]
    public static void TwoNestedIfsThatExecute(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStatesSingleTransition))]
    public static void TwoConsecutiveIfsThatExecute(HookOrchestrator hooks)
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
    [ExpectedSerialisedForm(
    """
    While <Actor.State != S1> {
    }
    Dialog {
      Text "After while block"
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStatesSingleTransition))]
    public static void WhileDoesNotExecute(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStatesSingleTransition))]
    public static void WhileExecutesOnceWithTransition(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStatesSingleTransition))]
    public static void WhileExecutesTwiceWithTransition(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStatesSingleTransition))]
    public static void WhileExecutesTwiceWithTransitionAndDialog(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStatesSingleTransition))]
    public static void WhileExecutesTwiceWithNestedIf(HookOrchestrator hooks)
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

    [ExpectedSerialisedForm(
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
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStatesSingleTransition))]
    public static void WhileExecutesTwiceWithNestedIf_NoDialogAfter(HookOrchestrator hooks)
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


    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Before call : {Actor.State}"
      }
      CallMetaStory {
        MetaStoryName "First MetaStory"
      }
      Dialog {
        Text "Between calls : {Actor.State}"
      }
      CallMetaStory {
        MetaStoryName "Second MetaStory"
      }
      Dialog {
        Text "After call : {Actor.State}"
      }
    }

    MetaStory "First MetaStory" {
      Transition {
        Actor : T1
      }
      Dialog {
        Text "Inside the first meta story"
      }
    }

    MetaStory "Second MetaStory" {
      Transition {
        Actor : T1
      }
      Dialog {
        Text "Inside the second meta story"
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStates))]
    public static void CallMetaStory_OneLevel_TwoDifferentCalls(HookOrchestrator hooks)
    {
        void FirstMetaStory(HookOrchestrator hooks)
        {
            hooks.StartMetaStory("First MetaStory");

            hooks.Transition("Actor", "T1").BuildAndRegister();

            hooks.Dialog("Inside the first meta story").BuildAndRegister();

            hooks.EndMetaStory();
        }

        void SecondMetaStory(HookOrchestrator hooks)
        {
            hooks.StartMetaStory("Second MetaStory");

            hooks.Transition("Actor", "T1").BuildAndRegister();

            hooks.Dialog("Inside the second meta story").BuildAndRegister();

            hooks.EndMetaStory();
        }

        hooks.StartMetaStory("MetaStory recorded by hooks");

        hooks.Dialog("Before call : {Actor.State}").BuildAndRegister();

        hooks.CallMetaStory("First MetaStory").BuildAndRegister();
        FirstMetaStory(hooks);

        hooks.Dialog("Between calls : {Actor.State}").BuildAndRegister();

        hooks.CallMetaStory("Second MetaStory").BuildAndRegister();
        SecondMetaStory(hooks);

        hooks.Dialog("After call : {Actor.State}").BuildAndRegister();

        hooks.EndMetaStory();
    }

    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Before call : {Actor.State}"
      }
      CallMetaStory {
        MetaStoryName "Secondary MetaStory"
      }
      Dialog {
        Text "Between calls : {Actor.State}"
      }
      CallMetaStory {
        MetaStoryName "Secondary MetaStory"
      }
      Dialog {
        Text "After call : {Actor.State}"
      }
    }

    MetaStory "Secondary MetaStory" {
      Dialog {
        Text "Inside the secondary meta story"
      }
      Transition {
        Actor : T1
      }
    }

    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStates))]
    public static void CallMetaStory_OneLevel_TwoCallsSameStory(HookOrchestrator hooks)
    {
        void SecondaryMetaStory(HookOrchestrator hooks)
        {
            hooks.StartMetaStory("Secondary MetaStory");

            hooks.Dialog("Inside the secondary meta story").BuildAndRegister();

            // TODO Change state
            hooks.Transition("Actor", "T1").BuildAndRegister();

            hooks.EndMetaStory();
        }

        hooks.StartMetaStory("MetaStory recorded by hooks");

        hooks.Dialog("Before call : {Actor.State}").BuildAndRegister();

        hooks.CallMetaStory("Secondary MetaStory").BuildAndRegister();
        SecondaryMetaStory(hooks);

        hooks.Dialog("Between calls : {Actor.State}").BuildAndRegister();

        hooks.CallMetaStory("Secondary MetaStory").BuildAndRegister();
        SecondaryMetaStory(hooks);

        hooks.Dialog("After call : {Actor.State}").BuildAndRegister();

        hooks.EndMetaStory();
    }

    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Before call : {Actor.State}"
      }
      CallMetaStory {
        MetaStoryName "Secondary MetaStory"
      }
      Dialog {
        Text "After call : {Actor.State}"
      }
    }
    
    MetaStory "Secondary MetaStory" {
      Dialog {
        Text "Before call inside the second meta story"
      }
      CallMetaStory {
        MetaStoryName "Tertiary MetaStory"
      }
      Dialog {
        Text "After call inside the second meta story"
      }
    }

    MetaStory "Tertiary MetaStory" {
      Transition {
        Actor : T1
      }
      Dialog {
        Text "Inside the third meta story"
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void CallMetaStory_TwoLevels(HookOrchestrator hooks)
    {
        void SecondaryMetaStory(HookOrchestrator hooks)
        {
            hooks.StartMetaStory("Secondary MetaStory");

            hooks.Dialog("Before call inside the second meta story").BuildAndRegister();

            hooks.CallMetaStory("Tertiary MetaStory").BuildAndRegister();
            TertiaryMetaStory(hooks);

            hooks.Dialog("After call inside the second meta story").BuildAndRegister();

            hooks.EndMetaStory();
        }

        void TertiaryMetaStory(HookOrchestrator hooks)
        {
            hooks.StartMetaStory("Tertiary MetaStory");

            hooks.Transition("Actor", "T1").BuildAndRegister();

            hooks.Dialog("Inside the third meta story").BuildAndRegister();

            hooks.EndMetaStory();
        }

        hooks.StartMetaStory("MetaStory recorded by hooks");

        hooks.Dialog("Before call : {Actor.State}").BuildAndRegister();

        hooks.CallMetaStory("Secondary MetaStory").BuildAndRegister();
        SecondaryMetaStory(hooks);

        hooks.Dialog("After call : {Actor.State}").BuildAndRegister();

        hooks.EndMetaStory();
    }

    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Before call : {Actor.State}"
      }
      CallMetaStory {
        MetaStoryName "First Secondary MetaStory"
      }
      Dialog {
        Text "Between calls : {Actor.State}"
      }
      CallMetaStory {
        MetaStoryName "Second Secondary MetaStory"
      }
      Dialog {
        Text "After call : {Actor.State}"
      }
    }
    
    MetaStory "First Secondary MetaStory" {
      Dialog {
        Text "Before call inside the first secondary meta story"
      }
      CallMetaStory {
        MetaStoryName "Tertiary MetaStory"
      }
      Dialog {
        Text "After call inside the first secondary meta story"
      }
    }
    
    MetaStory "Tertiary MetaStory" {
      Dialog {
        Text "Inside the tertiary meta story"
      }
      Transition {
        Actor : T1
      }
    }
    
    MetaStory "Second Secondary MetaStory" {
      Dialog {
        Text "Before call inside the second secondary meta story"
      }
      CallMetaStory {
        MetaStoryName "Tertiary MetaStory"
      }
      Dialog {
        Text "After call inside the second secondary meta story"
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStates))]
    public static void CallMetaStory_TwoLevelsCallSameTertiaryStory(HookOrchestrator hooks)
    {
        void FirstSecondaryMetaStory(HookOrchestrator hooks)
        {
            hooks.StartMetaStory("First Secondary MetaStory");

            hooks.Dialog("Before call inside the first secondary meta story").BuildAndRegister();

            hooks.CallMetaStory("Tertiary MetaStory").BuildAndRegister();
            TertiaryMetaStory(hooks);

            hooks.Dialog("After call inside the first secondary meta story").BuildAndRegister();

            hooks.EndMetaStory();
        }

        void SecondSecondaryMetaStory(HookOrchestrator hooks)
        {
            hooks.StartMetaStory("Second Secondary MetaStory");

            hooks.Dialog("Before call inside the second secondary meta story").BuildAndRegister();

            hooks.CallMetaStory("Tertiary MetaStory").BuildAndRegister();
            TertiaryMetaStory(hooks);

            hooks.Dialog("After call inside the second secondary meta story").BuildAndRegister();

            hooks.EndMetaStory();
        }

        void TertiaryMetaStory(HookOrchestrator hooks)
        {
            hooks.StartMetaStory("Tertiary MetaStory");

            hooks.Dialog("Inside the tertiary meta story").BuildAndRegister();

            // TODO Change state
            hooks.Transition("Actor", "T1").BuildAndRegister();

            hooks.EndMetaStory();
        }

        hooks.StartMetaStory("MetaStory recorded by hooks");

        hooks.Dialog("Before call : {Actor.State}").BuildAndRegister();

        hooks.CallMetaStory("First Secondary MetaStory").BuildAndRegister();
        FirstSecondaryMetaStory(hooks);

        hooks.Dialog("Between calls : {Actor.State}").BuildAndRegister();

        hooks.CallMetaStory("Second Secondary MetaStory").BuildAndRegister();
        SecondSecondaryMetaStory(hooks);

        hooks.Dialog("After call : {Actor.State}").BuildAndRegister();

        hooks.EndMetaStory();
    }

    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Before call : {Actor.State}"
      }
      CallMetaStory {
        MetaStoryName "Secondary MetaStory"
      }
      Dialog {
        Text "After call : {Actor.State}"
      }
    }

    MetaStory "Secondary MetaStory" {
      Dialog {
        Text "Inside the inner meta story"
      }
      Transition {
        Actor : T1
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void CallMetaStory_OneLevel(HookOrchestrator hooks)
    {
        void SecondaryMetaStory(HookOrchestrator hooks)
        {
            hooks.StartMetaStory("Secondary MetaStory");

            hooks.Dialog("Inside the inner meta story").BuildAndRegister();

            // TODO Change state
            hooks.Transition("Actor", "T1").BuildAndRegister();

            hooks.EndMetaStory();
        }

        hooks.StartMetaStory("MetaStory recorded by hooks");

        hooks.Dialog("Before call : {Actor.State}").BuildAndRegister();

        hooks.CallMetaStory("Secondary MetaStory").BuildAndRegister();
        SecondaryMetaStory(hooks);

        hooks.Dialog("After call : {Actor.State}").BuildAndRegister();

        hooks.EndMetaStory();
    }

    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Before call : {Actor.State}"
      }
      If <Actor.State == S1> {
        Transition {
          Actor : T1
        }
        CallMetaStory {
          MetaStoryName "MetaStory recorded by hooks"
        }
      }
      Dialog {
        Text "After call : {Actor.State}"
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void CallMetaStory_ReiterateMainMetaStoryOnce(HookOrchestrator hooks)
    {
        void MainMetaStory(string state)
        {
            hooks.StartMetaStory("MetaStory recorded by hooks");

            hooks.Dialog("Before call : {Actor.State}").BuildAndRegister();

            hooks.If("Actor.State == S1")
                 .GetConditionHook(out BifurcatingHook φ)
                 .GetScopeHook(out ScopeDefiningHook ifBlockUsingHook)
                 .Build();

            using (ifBlockUsingHook())
            {
                if (φ(state.Equals("Reiterate meta story")))
                {
                    state = "Don't reiterate meta story";
                    hooks.Transition("Actor", "T1").BuildAndRegister();

                    hooks.CallMetaStory("MetaStory recorded by hooks").BuildAndRegister();
                    MainMetaStory(state);
                }
            }

            hooks.Dialog("After call : {Actor.State}").BuildAndRegister();

            hooks.EndMetaStory();
        }

        MainMetaStory("Reiterate meta story");
    }

    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Before call : {Actor.State}"
      }
      CallMetaStory {
        MetaStoryName "Secondary MetaStory"
      }
      Dialog {
        Text "After call : {Actor.State}"
      }
    }

    MetaStory "Secondary MetaStory" {
      Dialog {
        Text "Inside the inner meta story"
      }
      If <Actor.State == S1> {
        Transition {
          Actor : T1
        }
        CallMetaStory {
          MetaStoryName "MetaStory recorded by hooks"
        }
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void CallMetaStory_ReiterateOnceFromSecondaryStory(HookOrchestrator hooks)
    {
        void MainMetaStory(string state)
        {
            hooks.StartMetaStory("MetaStory recorded by hooks");

            hooks.Dialog("Before call : {Actor.State}").BuildAndRegister();

            hooks.CallMetaStory("Secondary MetaStory").BuildAndRegister();
            SecondaryMetaStory(state);

            hooks.Dialog("After call : {Actor.State}").BuildAndRegister();

            hooks.EndMetaStory();
        }

        void SecondaryMetaStory(string state)
        {
            hooks.StartMetaStory("Secondary MetaStory");

            hooks.Dialog("Inside the inner meta story").BuildAndRegister();

            hooks.If("Actor.State == S1")
                 .GetConditionHook(out BifurcatingHook φ)
                 .GetScopeHook(out ScopeDefiningHook ifBlockUsingHook)
                 .Build();

            using (ifBlockUsingHook())
            {
                if (φ(state.Equals("Reiterate meta story")))
                {
                    state = "Don't reiterate meta story";
                    hooks.Transition("Actor", "T1").BuildAndRegister();

                    hooks.CallMetaStory("MetaStory recorded by hooks").BuildAndRegister();
                    MainMetaStory(state);
                }
            }

            // There's no dialog at the end on purpose, so that there is a jump of two up the graph stack which requires a loop in the method NextNode

            hooks.EndMetaStory();
        }

        MainMetaStory("Reiterate meta story");
    }

    #endregion
}
