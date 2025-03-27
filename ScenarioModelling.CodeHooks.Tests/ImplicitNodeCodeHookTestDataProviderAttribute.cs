using ScenarioModelling.CodeHooks.HookDefinitions.MetaStateObjects;
using ScenarioModelling.CodeHooks.Utils;
using System.Diagnostics;
using System.Reflection;

namespace ScenarioModelling.CodeHooks.Tests;

public class ImplicitNodeCodeHookTestDataProviderAttribute : Attribute, ITestDataSource
{
    private record ImplicitNodeTestCase(string MetaStoryWithImplicitNodeMethodName, string MetaStoryWithoutImplicitNodeMethodName, string SystemMethodName);

    private List<ImplicitNodeTestCase> TestData = [
        new(nameof(OneDialogAndOneJump_ImplicitMetaStory), nameof(OneDialogAndOneJump_FullMetaStory), nameof(SystemEmpty)),
            new(nameof(TwoDialogsAndOneJump_ImplicitMetaStory), nameof(TwoDialogsAndOneJump_FullMetaStory), nameof(SystemEmpty)),
            new(nameof(IfExecutesWithDialog_HookOutsideBlock_ImplicitIfNode), nameof(IfExecutesWithDialog_HookOutsideBlock_FullMetaStory), nameof(SystemOneActorTwoStates)),
            new(nameof(WhileExecutesTwiceWithTransition_ImplicitWhileNode), nameof(WhileExecutesTwiceWithTransition_FullMetaStory), nameof(SystemOneActorThreeStatesSingleTransition)),
        ];

    public IEnumerable<object[]> GetData(MethodInfo methodInfo) => TestData.Select((Func<ImplicitNodeTestCase, object[]>)(t => [t.MetaStoryWithImplicitNodeMethodName, t.MetaStoryWithoutImplicitNodeMethodName, t.SystemMethodName]));
    public string GetDisplayName(MethodInfo methodInfo, object?[]? data) => data?[0]?.ToString() ?? "";

    [DebuggerNonUserCode]
    public static Action<T> GetAction<T>(string metaStateMethodName)
    {
        var methodRef =
            typeof(ImplicitNodeCodeHookTestDataProviderAttribute)
                .GetMethod(metaStateMethodName, BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new Exception($"Method {metaStateMethodName} not found in {nameof(ImplicitNodeCodeHookTestDataProviderAttribute)}");

        return (parameter) => methodRef.Invoke(typeof(ImplicitNodeCodeHookTestDataProviderAttribute), [parameter]);
    }

    #region Systems
    private static void SystemEmpty(MetaStateHookDefinition sysConf)
    {
    }

    private static void SystemOneActorTwoStates(MetaStateHookDefinition sysConf)
    {
        sysConf.Entity("Actor")
               .SetState("S1");

        sysConf.StateMachine("SM1")
               .WithState("S1")
               .WithState("S2")
               .WithTransition("S1", "S2", "T1");
    }

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

    #region Jump
    private static void OneDialogAndOneJump_FullMetaStory(HookOrchestrator hooks)
    {
        hooks.Jump("D1")
             .SetAsImplicit()
             .BuildAndRegister();

        hooks.Dialog("Some text")
             .WithId("D1")
             .BuildAndRegister();
    }

    private static void OneDialogAndOneJump_ImplicitMetaStory(HookOrchestrator hooks)
    {
        //hooks.DeclareJump("D1")
        //     .SetAsImplicit();

        hooks.Dialog("Some text")
             .WithId("D1")
             .BuildAndRegister();
    }

    private static void TwoDialogsAndOneJump_FullMetaStory(HookOrchestrator hooks)
    {
        hooks.Jump("D2")
             .SetAsImplicit()
             .BuildAndRegister();

        hooks.Dialog("Some text")
             .WithId("D1")
             .BuildAndRegister();

        hooks.Dialog("Some more text")
             .WithId("D2")
             .BuildAndRegister();
    }

    private static void TwoDialogsAndOneJump_ImplicitMetaStory(HookOrchestrator hooks)
    {
        //hooks.DeclareJump("D2")
        //     .SetAsImplicit();

        //hooks.DeclareDialog("Some text")
        //     .SetId("D1");

        hooks.Dialog("Some more text")
             .WithId("D2")
             .BuildAndRegister();
    }
    #endregion

    #region If
    private static void IfExecutesWithDialog_HookOutsideBlock_FullMetaStory(HookOrchestrator hooks)
    {
        hooks.If(@"Actor.State == S1")
             .SetAsImplicit()
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

    private static void IfExecutesWithDialog_HookOutsideBlock_ImplicitIfNode(HookOrchestrator hooks)
    {
        //hooks.DeclareIfBranch(@"Actor.State == S1")
        //     .SetAsImplicit()
        //     .GetConditionHooks(out IfConditionHook φ, out IfBlockEndHook ifBlockEndHook);

        bool condition = true;
        //if (φ(condition))
        if (condition)
        {
            hooks.Dialog("Inside if block")
                 .BuildAndRegister();
        }
        //ifBlockEndHook();

        hooks.Dialog("After if block")
             .BuildAndRegister();
    }
    #endregion

    #region While
    private static void WhileExecutesTwiceWithTransition_FullMetaStory(HookOrchestrator hooks)
    {
        hooks.While(@"Actor.State != S3")
             .SetAsImplicit()
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

    private static void WhileExecutesTwiceWithTransition_ImplicitWhileNode(HookOrchestrator hooks)
    {
        //hooks.DeclareWhileBranch(@"Actor.State != S3")
        //     .SetAsImplicit()
        //     .GetConditionHook(out WhileHook φ);

        int count = 2;
        //while (φ(count > 0))
        while (count > 0)
        {
            hooks.Transition("Actor", "T1")
                 .BuildAndRegister();

            count--;
        }

        hooks.Dialog("After while block")
             .BuildAndRegister();
    }
    #endregion

    // TODO Test cases :
    // * Two consecutive implicit nodes
    // * An implicit node followed by an implicit node in a subgraph
    // * An implicit node followed by an implicit node in the parent graph
    // More ?
}
