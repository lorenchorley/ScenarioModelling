using FluentAssertions;
using ScenarioModel.CodeHooks;
using ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using System.Diagnostics;

namespace ScenarioModel.Tests;

[TestClass]
public class WhileLoopHookTest
{
    private string _scenarioText = """
        Entity Actor {
            EntityType ET1
            State "Amy Stake"
            CharacterStyle "Red"
        }
        
        EntityType ET1 {
            SM Name
        }
        
        SM Name {
            State "Amy Stake"
            State "Brock Lee"
            State "Clara Nett"
            State "Dee Zaster"
            "Amy Stake" -> "Brock Lee" : ChangeName
            "Brock Lee" -> "Clara Nett" : ChangeName
            "Clara Nett" -> "Dee Zaster" : ChangeName
        }
        
        Scenario NameSwappingPuns {
            Dialog SayName {
                Text "The actor {Actor.State} says hello and introduces themselves"
                Character Actor
            }
            While <Actor.State != "Dee Zaster"> {
                if <Actor.State == "Amy Stake"> {
                    Dialog {
                        Text "The actor Mrs Stake makes a bad pun to do with their name"
                        Character Actor
                    }
                }
                if <Actor.State == "Brock Lee"> {
                    Dialog {
                        Text "The actor Mr Lee makes a bad pun to do with their name"
                        Character Actor
                    }
                }
                if <Actor.State == "Clara Nett"> {
                    Dialog {
                        Text "The actor Mrs Nett makes a bad pun to do with their name"
                        Character Actor
                    }
                }
                Transition {
                    Actor : ChangeName
                }
            }
            if <Actor.State == "Dee Zaster"> {
                Dialog {
                    Text "The actor Mr Zaster makes a bad pun to do with their name"
                    Character Actor
                }
            }
        }
        """;

    private enum ActorName
    {
        AmyStake,
        BrockLee,
        ClaraNett,
        DeeZaster
    }

    void ProducerMethod(ScenarioHookOrchestrator hooks)
    {
        hooks.DefineSystem(configuration =>
        {
            configuration.DefineEntity("Actor")
                         .SetState("Amy Stake");

            configuration.DefineStateMachine("Name")
                         .WithTransition("Amy Stake", "Brock Lee", "ChangeName")
                         .WithTransition("Brock Lee", "Clara Nett", "ChangeName")
                         .WithTransition("Clara Nett", "Dee Zaster", "ChangeName");

        });

        ActorName actorName = ActorName.AmyStake;


        hooks.DeclareDialog("The actor {Actor.State} says hello and introduces themselves")
             .SetCharacter("Actor");

        Debug.WriteLine($"Hi, this is {actorName}");


        hooks.DeclareWhileBranch(@"Actor.State != ""Dee Zaster""")
             .GetConditionHook(out WhileHook whileHook);

        while (whileHook(actorName != ActorName.DeeZaster))
        {
            hooks.DeclareIfBranch(@"Actor.State == ""Amy Stake""")
                 .GetConditionHook(out IfHook ifHookAmy);
            if (ifHookAmy(actorName == ActorName.AmyStake))
            {
                hooks.DeclareDialog("Actor", "The actor Mrs Stake makes a bad pun to do with their name");
                Debug.WriteLine($"Amy's name was well chosen");
            }


            hooks.DeclareIfBranch(@"Actor.State == ""Brock Lee""")
                 .GetConditionHook(out IfHook ifHookBrock);
            if (ifHookBrock(actorName == ActorName.BrockLee))
            {
                hooks.DeclareDialog("Actor", "The actor Mr Lee makes a bad pun to do with their name");
                Debug.WriteLine($"Brock didn't like his vegies");
            }


            hooks.DeclareIfBranch(@"Actor.State == ""Clara Nett""")
                 .GetConditionHook(out IfHook ifHookClara);
            if (ifHookClara(actorName == ActorName.ClaraNett))
            {
                hooks.DeclareDialog("Actor", "The actor Mrs Nett makes a bad pun to do with their name");
                Debug.WriteLine($"Clara hated music");
            }


            hooks.DeclareTransition("Actor", "ChangeName");

            actorName = actorName switch
            {
                ActorName.AmyStake => ActorName.BrockLee,
                ActorName.BrockLee => ActorName.ClaraNett,
                ActorName.ClaraNett => ActorName.DeeZaster,
                _ => throw new Exception("This should not happen")
            };

        }

        hooks.DeclareIfBranch(@"Actor.State == ""Dee Zaster""")
             .GetConditionHook(out IfHook ifHookDee);
        if (ifHookDee(actorName == ActorName.DeeZaster))
        {
            hooks.DeclareDialog("Actor", "The actor Mr Zaster makes a bad pun to do with their name");
            Debug.WriteLine($"Well, that went well !");
        }

    }

    [TestMethod]
    [TestCategory("Hooks")]
    public void ScenarioWithWhileLoop_ConstructionTest()
    {
        // Arrange
        // =======
        Context context =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .Initialise();

        ScenarioHookOrchestratorForConstruction hooks = new ScenarioHookOrchestratorForConstruction(context);

        var deserialisedContext =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .LoadContext<HumanReadableSerialiser>(_scenarioText)
                   .Initialise()
                   .Serialise<HumanReadableSerialiser>()
                   .Match(v => v, e => throw e);


        // Act
        // ===

        // The scenario declaration is made outside the producer because the scenario depends on how the producer is called (here the choices could be different)
        hooks.DeclareScenarioStart("NameSwappingPuns");

        // Run the code and produce the scenario from the called hooks

        Debug.WriteLine("");
        Debug.WriteLine("Producer method output :");
        ProducerMethod(hooks);

        Scenario generatedScenario = hooks.DeclareScenarioEnd();


        // Assert
        // ======
        generatedScenario.Should().NotBeNull();

        var serialisedResult =
            context.Serialise<HumanReadableSerialiser>()
                   .Match(v => v, e => throw e);

        Debug.WriteLine("");
        Debug.WriteLine("Final serialised context :");
        Debug.WriteLine(serialisedResult);

        serialisedResult.Should().Be(deserialisedContext);
    }

    [TestMethod]
    [TestCategory("Hooks")]
    public void ScenarioWithWhileLoop_ValidationTest()
    {
        // Arrange
        // =======
        Context context =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .Initialise();

        var deserialisedContext =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .LoadContext<HumanReadableSerialiser>(_scenarioText)
                   .Initialise()
                   .Serialise<HumanReadableSerialiser>()
                   .Match(v => v, e => throw e);

        ScenarioHookOrchestratorForValidation hooks = new ScenarioHookOrchestratorForValidation(context);


        // Act
        // ===

        // The scenario declaration is made outside the producer because the scenario depends on how the producer is called (here the choices could be different)
        hooks.DeclareScenarioStart("NameSwappingPuns");

        // Run the code and produce the scenario from the called hooks

        Debug.WriteLine("");
        Debug.WriteLine("Producer method output :");
        ProducerMethod(hooks);

        hooks.DeclareScenarioEnd();


        // Assert
        // ======
        var serialisedResult =
            context.Serialise<HumanReadableSerialiser>()
                   .Match(v => v, e => throw e);

        Debug.WriteLine("");
        Debug.WriteLine("Final serialised context :");
        Debug.WriteLine(serialisedResult);

        serialisedResult.Should().Be(deserialisedContext);
    }
}
