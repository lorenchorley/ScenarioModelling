using FluentAssertions;
using ScenarioModel.CodeHooks;
using ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;
using ScenarioModel.Execution;
using ScenarioModel.Execution.Dialog;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Interpolation;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModel.Tests.ScenarioRuns;
using System.Diagnostics;

namespace ScenarioModel.Tests.HookTests;

[TestClass]
[UsesVerify]
public partial class ChooseAndJumpHookTest
{
    private string _scenarioText = """
        Entity Actor {
          State Bob
        }

        SM Name {
          State Bob
          State Alice
          Bob -> Alice : ChangeName
          Alice -> Bob : ChangeName
        }

        Scenario "Scenario recorded by hooks" {
          Dialog {
            Text "The actor {Actor.State} says hello and introduces themselves"
            Character Actor
          }
          Choose LoopStart {
            Change "Change name and repeat"
            GoodBye Ciao
          }
          Transition Change {
            Actor : ChangeName
          }
          If <Actor.State != Alice> {
            Dialog {
              Text "The actor declares themselves to be Alice"
              Character Actor
            }
          }
          Jump {
            Target LoopStart
          }
          Dialog GoodBye {
            Text "The actor {Actor.State} says goodbye"
            Character Actor
          }
        }
        """;

    void ProducerMethod(ScenarioHookOrchestrator hooks, Queue<string> choices)
    {
        hooks.DefineSystem(configuration =>
        {
            configuration.DefineEntity("Actor")
                         .SetState("Bob");

            configuration.DefineStateMachine("Name")
                         .WithTransition("Bob", "Alice", "ChangeName")
                         .WithTransition("Alice", "Bob", "ChangeName");
        });


        string ActorName = "Bob";


        hooks.DeclareDialog("The actor {Actor.State} says hello and introduces themselves")
             .SetCharacter("Actor");

        Debug.WriteLine($"{ActorName}: Hello");
        Debug.WriteLine($"{ActorName}: My name is {ActorName}");


        hooks.DeclareChoose(new ChoiceList() { ("Change", "Change name and repeat") })
             .GetConditionHook(out ChooseHook φ)
             .SetId("LoopStart")
             .WithJump("GoodBye", "Ciao");

        while (φ(choices.Dequeue()) == "Change name and repeat")
        {
            hooks.DeclareTransition("Actor", "ChangeName")
                 .SetId("Change");

            if (ActorName == "Bob")
                ActorName = "Alice";
            else
                ActorName = "Bob";


            hooks.DeclareIfBranch(@"Actor.State != ""Alice""")
                 .GetConditionHooks(out IfConditionHook ψ, out IfBlockEndHook ifBlockEndHook);

            if (ψ(ActorName == "Alice"))
            {
                hooks.DeclareDialog("Actor", "The actor declares themselves to be Alice");
                Debug.WriteLine($"{ActorName}: I am now Alice !");

                ifBlockEndHook();
            }

            hooks.DeclareJump("LoopStart");
        }

        hooks.DeclareDialog("Actor", "The actor {Actor.State} says goodbye")
            .SetId("GoodBye");

        Debug.WriteLine($"{ActorName}: Bubye");
    }

    [TestMethod]
    [TestCategory("Code Hooks"), TestCategory("Scenario Construction")]
    public void ScenarioWithChooseAndIf_ScenarioConstructionTest()
    {
        // Arrange
        // =======
        Context context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        ScenarioHookOrchestrator hooks = new ScenarioHookOrchestratorForConstruction(context);

        Queue<string> choices = new();
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Ciao");

        var reserialisedContext =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .LoadContext(_scenarioText)
                   .Initialise()
                   .Serialise()
                   .Match(v => v, e => throw e);


        // Act
        // ===

        // The scenario declaration is made outside the producer because the scenario depends on how the producer is called (here the choices could be different)
        hooks.DeclareScenarioStart("Scenario recorded by hooks");

        // Run the code and produce the scenario from the called hooks

        Debug.WriteLine("");
        Debug.WriteLine("Producer method output :");
        ProducerMethod(hooks, choices);

        Scenario generatedScenario = hooks.DeclareScenarioEnd();


        // Assert
        // ======
        generatedScenario.Should().NotBeNull();

        var contextBuiltFromHooks =
            context.Serialise()
                   .Match(v => v, e => throw e);

        Debug.WriteLine("");
        Debug.WriteLine("Final serialised context :");
        Debug.WriteLine(contextBuiltFromHooks);

        var originalContext = _scenarioText;
        DiffAssert.DiffIfNotEqual(originalContext, reserialisedContext, contextBuiltFromHooks);
    }

    [TestMethod]
    [TestCategory("Code Hooks"), TestCategory("Scenario Execution")]
    [Ignore]
    public async Task ScenarioWithChooseAndIf_ScenarioExecutionTest()
    {
        // Arrange
        // =======
        Context context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        ScenarioHookOrchestrator hooks = new ScenarioHookOrchestratorForConstruction(context);

        Queue<string> choices = new();
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Ciao");

        var reserialisedContext =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .LoadContext(_scenarioText)
                   .Initialise()
                   .Serialise()
                   .Match(v => v, e => throw e);

        // Everything necessary to run the scenario
        ExpressionEvalator evalator = new(context.System);
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);
        ScenarioTestRunner runner = new(executor, dependencies);


        // Act
        // ===

        // The scenario declaration is made outside the producer because the scenario depends on how the producer is called (here the choices could be different)
        hooks.DeclareScenarioStart("Scenario recorded by hooks");

        // Run the code and produce the scenario from the called hooks

        Debug.WriteLine("");
        Debug.WriteLine("Producer method output :");
        ProducerMethod(hooks, choices);

        Scenario generatedScenario = hooks.DeclareScenarioEnd();

        ScenarioRun scenarioRun = runner.Run("Scenario recorded by hooks");


        // Assert
        // ======
        generatedScenario.Should().NotBeNull();

        string events = scenarioRun.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();

        Debug.WriteLine("");
        Debug.WriteLine("Final serialised events :");
        Debug.WriteLine(events);

        await Verify(events);
    }
}
