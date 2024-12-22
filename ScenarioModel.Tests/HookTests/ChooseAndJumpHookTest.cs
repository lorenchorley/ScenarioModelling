using FluentAssertions;
using ScenarioModel.CodeHooks;
using ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using System.Diagnostics;

namespace ScenarioModel.Tests.HookTests;

[TestClass]
public class ChooseAndJumpHookTest
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

        Scenario First {
          Dialog {
            Text Hello
            Character Actor
          }
          Dialog {
            Text "My name is {Actor.State}"
            Character Actor
          }
          Choose LoopStart {
            Change "Change name and repeat"
            GoodBye Ciao
          }
          Transition Change {
            Actor : ChangeName
          }
          if <Actor.State == Alice> {
            Dialog {
              Text "I am now Alice !"
              Character Actor
            }
          }
          Jump {
            LoopStart
          }
          Dialog GoodBye {
            Text "Bubye (Actor called {Actor.State} in the end)"
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

        Debug.WriteLine($"Hello");
        Debug.WriteLine($"My name is {ActorName}");


        hooks.DeclareChoose(new ChoiceList() { ("Change name and repeat", ""), ("Ciao", "") }) // TODO
             .GetConditionHook(out ChooseHook chooseCondition)
             .SetId("LoopStart")
             .WithJump("Change name and repeat", "Change")
             .WithJump("Ciao", "GoodBye");

        while (chooseCondition(choices.Dequeue() == "Change name and repeat"))
        {
            hooks.DeclareTransition("Actor", "ChangeName")
                 .SetId("Change");

            if (ActorName == "Bob")
            {
                ActorName = "Alice";
            }
            else
            {
                ActorName = "Bob";
            }


            hooks.DeclareIfBranch(@"Actor.State != ""Alice""")
                 .GetConditionHooks(out IfConditionHook ifHook, out IfBlockEndHook ifBlockEndHook);

            if (ifHook(ActorName == "Alice"))
            {
                hooks.DeclareDialog("Actor", "The actor declares themselves to be Alice");
                Debug.WriteLine($"I am now Alice !");

                ifBlockEndHook();
            }

            hooks.DeclareJump("LoopStart");
        }

        hooks.DeclareDialog("Actor", "The actor {Actor.State} says goodbye");
        Debug.WriteLine($"Bubye (Actor called {ActorName} in the end)");
    }

    [TestMethod]
    [TestCategory("CodeHooks")]
    public void ScenarioWithChooseAndIfTest()
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
        hooks.DeclareScenarioStart("First");

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
}
