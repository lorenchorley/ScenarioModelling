using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions.MetaStateObjects;
using ScenarioModelling.TestDataAndTools.Attributes;

namespace ScenarioModelling.TestDataAndTools.TestCases;

public class TestCaseTestData
{
    #region Systems
    [ExpectedSerialisedForm(
    """
    
    """)]
    public static void EmptyMetaState(MetaStateHookDefinition _)
    {
    }

    [ExpectedSerialisedForm(
    """
    Entity Actor {
    }
    """)]
    public static void MetaStateOneActor(MetaStateHookDefinition config)
    {
        config.Entity("Actor");

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
    public static void MetaStateOneActorTwoStates(MetaStateHookDefinition config)
    {
        config.Entity("Actor")
               .SetState("S1");

        config.StateMachine("SM1")
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
    public static void MetaStateOneActorThreeStates(MetaStateHookDefinition config)
    {
        config.Entity("Actor")
              .SetState("S1");

        config.StateMachine("SM1")
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
    public static void MetaStateOneActorThreeStatesSingleTransition(MetaStateHookDefinition config)
    {
        config.Entity("Actor")
               .SetState("S1");

        config.StateMachine("SM1")
               .WithState("S1")
               .WithState("S2")
               .WithState("S3")
               .WithTransition("S1", "S2", "T1")
               .WithTransition("S2", "S3", "T1");
    }
    #endregion

    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Nothing going on here"
      }
    }

    TestCase "Main test case" {
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(EmptyMetaState))]
    public static void NoStateChanges_NoAssertions_Succeeds(HookOrchestrator hooks)
    {
        hooks.TestCase("Main test case", "MetaStory recorded by hooks")
             .BuildAndRegister();

        hooks.StartMetaStory("MetaStory recorded by hooks");

        hooks.Dialog("Nothing going on here")
             .BuildAndRegister();

        hooks.EndMetaStory();
    }
    
    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Changing the state"
      }
      Transition {
        Actor : T1
      }
    }

    TestCase "Main test case" {
      InitialStates {
      }
      ExpectedStates {
        Actor.State S2
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorTwoStates))]
    public static void OneStateChange_OneAssertion_Succeeds(HookOrchestrator hooks)
    {
        hooks.TestCase("Main test case", "MetaStory recorded by hooks")
             .AddFinalState("Actor.State", "S2")
             .BuildAndRegister();

        hooks.StartMetaStory("MetaStory recorded by hooks");

        hooks.Transition("Actor", "T1")
             .BuildAndRegister();

        hooks.EndMetaStory();
    }
    
    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Changing the state"
      }
      Transition {
        Actor : T1
      }
    }

    TestCase "Main test case" {
      InitialStates {
        Actor.State S2
      }
      ExpectedStates {
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStates))]
    public static void OneStateChange_InitialStateDifferent_NoAssertions_Succeeds(HookOrchestrator hooks)
    {
        hooks.TestCase("Main test case", "MetaStory recorded by hooks")
             .AddInitialState("Actor.State", "S2")
             .BuildAndRegister();

        hooks.Transition("Actor", "T1")
             .BuildAndRegister();

        hooks.EndMetaStory();
    }
    
    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Changing the state"
      }
      Transition {
        Actor : T1
      }
    }

    TestCase "Main test case" {
      InitialStates {
        Actor.State S2
      }
      ExpectedStates {
        Actor.State S3
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStates))]
    public static void OneStateChange_InitialStateDifferent_OneAssertion_Succeeds(HookOrchestrator hooks)
    {
        hooks.TestCase("Main test case", "MetaStory recorded by hooks")
             .AddInitialState("Actor.State", "S2")
             .AddFinalState("Actor.State", "S3")
             .BuildAndRegister();

        hooks.Transition("Actor", "T1")
             .BuildAndRegister();

        hooks.EndMetaStory();
    }
    
    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Changing the state"
      }
      Transition {
        Actor : T1
      }
    }

    TestCase "Main test case" {
      InitialStates {
        Actor.State S2
      }
      ExpectedStates {
        Actor.State S3
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStates))]
    public static void OneState_MultipleInitialStates_OneAssertion_Fails_OnUnknownEntity(HookOrchestrator hooks)
    {
        hooks.TestCase("Main test case", "MetaStory recorded by hooks")
             .AddInitialState("Actor.State", "S2")
             .AddFinalState("Actor.State", "S3")
             .BuildAndRegister();

        hooks.Transition("Actor", "T1")
             .BuildAndRegister();

        hooks.EndMetaStory();
    }
    
    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Changing the state"
      }
      Transition {
        Actor : T1
      }
    }

    TestCase "Main test case" {
      InitialStates {
        Actor.State S2
      }
      ExpectedStates {
        Actor.State S3
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStates))]
    public static void OneState_MultipleInitialStates_OneAssertion_Fails_OnEntityWithoutState(HookOrchestrator hooks)
    {
        hooks.TestCase("Main test case", "MetaStory recorded by hooks")
             .AddInitialState("Actor.State", "S2")
             .AddFinalState("Actor.State", "S3")
             .BuildAndRegister();

        hooks.Transition("Actor", "T1")
             .BuildAndRegister();

        hooks.EndMetaStory();
    }
    
    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Changing the state"
      }
      Transition {
        Actor : T1
      }
    }

    TestCase "Main test case" {
      InitialStates {
        Actor.State S2
      }
      ExpectedStates {
        Actor.State S3
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStates))]
    public static void OneState_MultipleAssertions_Fails_OnUnknownEntity(HookOrchestrator hooks)
    {
        hooks.TestCase("Main test case", "MetaStory recorded by hooks")
             .AddInitialState("Actor.State", "S2")
             .AddFinalState("Actor.State", "S3")
             .BuildAndRegister();

        hooks.Transition("Actor", "T1")
             .BuildAndRegister();

        hooks.EndMetaStory();
    }
    
    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Changing the state"
      }
      Transition {
        Actor : T1
      }
    }

    TestCase "Main test case" {
      InitialStates {
        Actor.State S2
      }
      ExpectedStates {
        Actor.State S3
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStates))]
    public static void OneState_MultipleAssertions_Fails_OnEntityWithoutState(HookOrchestrator hooks)
    {
        hooks.TestCase("Main test case", "MetaStory recorded by hooks")
             .AddInitialState("Actor.State", "S2")
             .AddFinalState("Actor.State", "S3")
             .BuildAndRegister();

        hooks.Transition("Actor", "T1")
             .BuildAndRegister();

        hooks.EndMetaStory();
    }
    
    [ExpectedSerialisedForm(
    """
    MetaStory "MetaStory recorded by hooks" {
      Dialog {
        Text "Changing the state"
      }
      Transition {
        Actor : T1
      }
    }

    TestCase "Main test case" {
      InitialStates {
        Actor.State S2
      }
      ExpectedStates {
        Actor.State S3
      }
    }
    """)]
    [AssociatedMetaStateHookMethod(nameof(MetaStateOneActorThreeStates))]
    public static void MultipleStates_OneAssertion_Succeeds(HookOrchestrator hooks)
    {
        hooks.TestCase("Main test case", "MetaStory recorded by hooks")
             .AddInitialState("Actor.State", "S2")
             .AddFinalState("Actor.State", "S3")
             .BuildAndRegister();

        hooks.Transition("Actor", "T1")
             .BuildAndRegister();

        hooks.EndMetaStory();
    }
}
