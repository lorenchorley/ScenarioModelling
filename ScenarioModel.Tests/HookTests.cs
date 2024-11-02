using FluentAssertions;
using ScenarioModel.CodeHooks;
using ScenarioModel.CodeHooks.HookDefinitions;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using System.Diagnostics;

namespace ScenarioModel.Tests;

[TestClass]
public class HookTests
{
    private string _scenarioText = """
        Entity Actor {
            State Bob
        }

        SM Name {
            Bob -> Alice : ChangeName
            Alice -> Bob : ChangeName
        }

        Scenario First {
            Dialog {
                Character Actor
                Text "Hello"
            }
            Dialog {
                Character Actor
                Text "My name is {Actor.State}"
            }
            Choose LoopStart {
                Change "Change name and repeat"
                GoodBye "Ciao"
            }
            Transition Change {
                Actor : ChangeName
            }
            if <Actor.State == "Alice"> {
                Dialog {
                    Character Actor
                    Text "I am now Alice !"
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

    void ProducerMethod(Hooks hooks, Queue<string> choices)
    {
        hooks.DefineEntity("Actor")
             .SetState("Bob");

        hooks.DefineStateMachine("Name")
             .WithTransition("Bob", "Alice", "ChangeName")
             .WithTransition("Alice", "Bob", "ChangeName");

        string ActorName = "Bob";


        hooks.DeclareDialog("The actor {Actor.State} says hello and introduces themselves")
             .SetCharacter("Actor");

        Debug.WriteLine($"Hello");
        Debug.WriteLine($"My name is {ActorName}");


        hooks.DeclareChoose("Change name and repeat", "Ciao")
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
                 .GetConditionHook(out IfHook ifHook);

            if (ifHook(ActorName == "Alice"))
            {
                hooks.DeclareDialog("Actor", "The actor declares themselves to be Alice");
                Debug.WriteLine($"I am now Alice !");
            }

            hooks.DeclareJump("LoopStart");
        }

        hooks.DeclareDialog("Actor", "The actor {Actor.State} says goodbye");
        Debug.WriteLine($"Bubye (Actor called {ActorName} in the end)");
    }

    [TestMethod]
    [TestCategory("Hooks")]
    public void ScenarioWithChooseAndIfTest()
    {
        // Arrange
        // =======
        Context context =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .Initialise();

        Hooks hooks = new HooksForScenarioCreation(context);

        Queue<string> choices = new();
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Ciao");


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

        var serialisedResult = context.Serialise<HumanReadableSerialiser>();
        serialisedResult.IsSuccess.Should().Be(true);

        Debug.WriteLine("");
        Debug.WriteLine("Final serialised context :");
        serialisedResult.IfSucc(v => Debug.WriteLine(v));
    }
}
