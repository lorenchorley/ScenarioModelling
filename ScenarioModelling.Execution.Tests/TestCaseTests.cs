using ScenarioModelling.CodeHooks;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;
using ScenarioModelling.TestDataAndTools;
using ScenarioModelling.TestDataAndTools.TestCases;
using ScenarioModelling.CodeHooks.HookDefinitions.MetaStateObjects;
using ScenarioModelling.TestDataAndTools.CodeHooks;

namespace ScenarioModelling.Execution.Tests;

[TestClass]
[UsesVerify]
public partial class TestCaseTests
{
    [DataTestMethod]
    [TestCategory("Code Hooks"), TestCategory("Test Cases")]
    [TestCaseTestDataProvider]
    public async Task TestCaseTest(string metaStoryMethodName, string metaStateMethodName, bool testDefinedFirstMetaStory)
    {
        // Arrange
        // =======
        using TestContainer container = new();
        using var scope = container.StartScope();

        var context =
            scope.GetService<Context>()
                 .UseSerialiser<CustomContextSerialiser>()
                 .Initialise();

        HookOrchestrator orchestrator = scope.GetService<MetaStoryHookOrchestratorForConstruction>();

        var metaStateHooksMethod = TestCaseTestDataProviderAttribute.GetAction<MetaStateHookDefinition>(metaStateMethodName);
        var metaStoryHooksMethod = TestCaseTestDataProviderAttribute.GetAction<HookOrchestrator>(metaStoryMethodName);
        var expectedSerialisedContext = TestCaseTestDataProviderAttribute.GetExpectedContextText(metaStateMethodName, metaStoryMethodName, testDefinedFirstMetaStory);


        // Act
        // ===

        // Build MetaState
        orchestrator.DefineMetaState(metaStateHooksMethod);

        // Build MetaStory
        if (!testDefinedFirstMetaStory)
            orchestrator.StartMetaStory(TestCaseTestDataProviderAttribute.PrimaryMetaStoryName);

        metaStoryHooksMethod(orchestrator);

        if (!testDefinedFirstMetaStory)
            orchestrator.EndMetaStory();


        // Assert
        // ======
        var serialisedContext =
            context.ResetToInitialState()
                   .Serialise()
                   .Match(v => v, e => throw e)
                   .Trim();

        DiffAssert.DiffIfNotEqual(expectedSerialisedContext, serialisedContext);
        //await Verify(serialisedContext)
        //    .UseParameters(metaStoryMethodName);
    }
}
