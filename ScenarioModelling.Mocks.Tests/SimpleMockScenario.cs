using ScenarioModelling.Exhaustiveness.Common;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;

namespace ScenarioModelling.Mocks.Tests;

[TestClass]
public sealed class SimpleMockScenario
{
    [AssemblyInitialize()]
    public static void AssemblyInit(TestContext context)
    {
        ExhaustivityFunctions.Active = true;
    }

    private readonly string _context = """
    Entity Parameter {
      State P1
    }
    
    StateMachine Parameter_SM {
      State P1
      State P2
    }
    
    Entity Actor {
      State S1
    }
    
    StateMachine Actor_SM {
      S1 -> S2 : T1
      S1 -> S3 : T2
    }
    
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
      If <Parameter.State == P1> {
        Transition {
          Actor : T1
        }
      }
      If <Parameter.State == P2> {
        Transition {
          Actor : T2
        }
      }
    }
    """;

    [TestMethod]
    [TestCategory("Mocks")]
    public void MetaStoryMock_UsingValues()
    {
        // Arrange
        // =======

        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();


        var context =
            scope.Context
                 .UseSerialiser<CustomContextSerialiser>()
                 .LoadContext(_context)
                 .Initialise();


        // Act
        // ===
        MetaStoryMock mock =
            scope.CreateMetaStoryMockBuilder()
                 .WithTargetMetaStoryName("Secondary MetaStory")
                 .RegisterBusinessType<int>("InputValue")
                 .RegisterBusinessType<string>("OutputValue")
                 .AddBusinessTypeToStatefulObjectRelation(businessTypeName: "InputValue", statefulObjectName: "Parameter")
                 .AddValueToStateTranslator(businessTypeName: "InputValue", (int businessValue) =>
                     businessValue switch
                     {
                         >= 1 and <= 49 => "P1",
                         >= 50 and <= 100 => "P2",
                         _ => throw new Exception("Unexpected business value")
                     })
                 .AddBusinessTypeToStatefulObjectRelation(businessTypeName: "OutputValue", statefulObjectName: "Actor")
                 .AddStateToValueTranslation(stateName: "S2", businessTypeName: "OutputValue", businessValue: "Default Branch")
                 .AddStateToValueTranslation(stateName: "S3", businessTypeName: "OutputValue", businessValue: "Secondary Branch")
                 .Build();

        MockCallResult callResultWithDefaultOption =
            mock.CreateCallBuilder()
                .Call();

        MockCallResult callResultWithExplicitDefaultOption =
            mock.CreateCallBuilder()
                .SetValue(businessType: "InputValue", businessValue: 3)
                .Call();

        MockCallResult callResultWithSecondOption =
            mock.CreateCallBuilder()
                .SetValue(businessType: "InputValue", businessValue: 99)
                .Call();


        // Assert
        // ======
        var defaultValue = callResultWithDefaultOption.GetValue<string>(businessType: "OutputValue");
        Assert.AreEqual("Default Branch", defaultValue);

        var firstValue = callResultWithExplicitDefaultOption.GetValue<string>(businessType: "OutputValue");
        Assert.AreEqual("Default Branch", firstValue);

        var secondValue = callResultWithSecondOption.GetValue<string>("OutputValue");
        Assert.AreEqual("Secondary Branch", secondValue);

    }

    [TestMethod]
    [TestCategory("Mocks")]
    public void MetaStoryMock_UsingStates()
    {
        // Arrange
        // =======

        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();

        var context =
            scope.Context
                 .UseSerialiser<CustomContextSerialiser>()
                 .LoadContext(_context)
                 .Initialise();


        // Act
        // ===
        MetaStoryMock mock =
            scope.CreateMetaStoryMockBuilder()
                 .WithTargetMetaStoryName("Secondary MetaStory")
                 .Build();

        MockCallResult callResultWithDefaultStates =
            mock.CreateCallBuilder()
                .Call();

        MockCallResult callResultWithExplicitDefaultState =
            mock.CreateCallBuilder()
                .SetState(statefulObjectName: "Parameter", stateName: "P1")
                .Call();

        MockCallResult callResultWithP2 =
            mock.CreateCallBuilder()
                .SetState(statefulObjectName: "Parameter", stateName: "P2")
                .Call();


        // Assert
        // ======
        Assert.IsTrue(callResultWithDefaultStates.FinalStatesByStatefulObjectName.ContainsKey("Actor"));
        Assert.IsTrue(callResultWithDefaultStates.FinalStatesByStatefulObjectName.ContainsKey("Parameter"));

        var defaultStateResult = callResultWithDefaultStates.GetStateOf("Actor");
        Assert.AreEqual("S2", defaultStateResult);

        var p1Result = callResultWithExplicitDefaultState.GetStateOf("Actor");
        Assert.AreEqual("S2", p1Result);

        var p2Result = callResultWithP2.GetStateOf("Actor");
        Assert.AreEqual("S3", p2Result);

    }
}
