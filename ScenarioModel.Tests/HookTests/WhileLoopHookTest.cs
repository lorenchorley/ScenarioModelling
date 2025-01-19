using FluentAssertions;
using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Expressions.Evaluation;
using ScenarioModelling.Interpolation;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.Tests.Stories;
using System.Diagnostics;

namespace ScenarioModelling.Tests.HookTests;

[TestClass]
[UsesVerify]
public partial class WhileLoopHookTest
{
    private string _metaStoryText = """
        Entity Actor {
          EntityType ET1
          State "Amy Stake"
          CharacterStyle Red
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
        
        MetaStory "MetaStory recorded by hooks" {
          Dialog SayName {
            Text "The actor {Actor.State} says hello and introduces themselves"
            Character Actor
          }
          While <Actor.State != "Dee Zaster"> {
            If <Actor.State == "Amy Stake"> {
              Dialog {
                Text "The actor Mrs Stake makes a bad pun to do with their name"
                Character Actor
              }
            }
            If <Actor.State == "Brock Lee"> {
              Dialog {
                Text "The actor Mr Lee makes a bad pun to do with their name"
                Character Actor
              }
            }
            If <Actor.State == "Clara Nett"> {
              Dialog {
                Text "The actor Mrs Nett makes a bad pun to do with their name"
                Character Actor
              }
            }
            Transition {
              Actor : ChangeName
            }
          }
          If <Actor.State == "Dee Zaster"> {
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

    void ProducerMethod(MetaStoryHookOrchestrator hooks)
    {
        hooks.DefineSystem(configuration =>
        {
            configuration.DefineEntity("Actor")
                         .SetState("Amy Stake")
                         .SetType("ET1")
                         .SetCharacterStyle("Red");

            configuration.DefineStateMachine("Name")
                         .WithTransition("Amy Stake", "Brock Lee", "ChangeName")
                         .WithTransition("Brock Lee", "Clara Nett", "ChangeName")
                         .WithTransition("Clara Nett", "Dee Zaster", "ChangeName");

        });

        ActorName actorName = ActorName.AmyStake;


        hooks.DeclareDialog("The actor {Actor.State} says hello and introduces themselves")
             .SetId("SayName")
             .SetCharacter("Actor")
             .Build();

        Debug.WriteLine($"Hi, this is {actorName}");


        hooks.DeclareWhileBranch(@"Actor.State != ""Dee Zaster""")
             .GetConditionHook(out WhileHook whileHook)
             .Build();

        IfBlockEndHook ifBlockEndHook;

        while (whileHook(actorName != ActorName.DeeZaster))
        {
            hooks.DeclareIfBranch(@"Actor.State == ""Amy Stake""")
                 .GetConditionHooks(out IfConditionHook ifHookAmy, out ifBlockEndHook)
                 .Build();
            if (ifHookAmy(actorName == ActorName.AmyStake))
            {
                hooks.DeclareDialog("Actor", "The actor Mrs Stake makes a bad pun to do with their name").Build();
                Debug.WriteLine($"Amy's name was well chosen");

                ifBlockEndHook();
            }


            hooks.DeclareIfBranch(@"Actor.State == ""Brock Lee""")
                 .GetConditionHooks(out IfConditionHook ifHookBrock, out ifBlockEndHook)
                 .Build();
            if (ifHookBrock(actorName == ActorName.BrockLee))
            {
                hooks.DeclareDialog("Actor", "The actor Mr Lee makes a bad pun to do with their name").Build();
                Debug.WriteLine($"Brock didn't like his vegies");

                ifBlockEndHook();
            }


            hooks.DeclareIfBranch(@"Actor.State == ""Clara Nett""")
                 .GetConditionHooks(out IfConditionHook ifHookClara, out ifBlockEndHook)
                 .Build();

            if (ifHookClara(actorName == ActorName.ClaraNett))
            {
                hooks.DeclareDialog("Actor", "The actor Mrs Nett makes a bad pun to do with their name").Build();
                Debug.WriteLine($"Clara hated music");

                ifBlockEndHook();
            }


            hooks.DeclareTransition("Actor", "ChangeName")
                 .Build();

            actorName = actorName switch
            {
                ActorName.AmyStake => ActorName.BrockLee,
                ActorName.BrockLee => ActorName.ClaraNett,
                ActorName.ClaraNett => ActorName.DeeZaster,
                _ => throw new Exception("This should not happen")
            };

        }

        hooks.DeclareIfBranch(@"Actor.State == ""Dee Zaster""")
             .GetConditionHooks(out IfConditionHook ifHookDee, out ifBlockEndHook)
             .Build();
        if (ifHookDee(actorName == ActorName.DeeZaster))
        {
            hooks.DeclareDialog("Actor", "The actor Mr Zaster makes a bad pun to do with their name").Build();
            Debug.WriteLine($"Well, that went well !");

            ifBlockEndHook();
        }

    }

    [TestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory Construction")]
    public void WhileLoop_MetaStoryConstructionTest()
    {
        // Arrange
        // =======
        Context context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestratorForConstruction hooks = new MetaStoryHookOrchestratorForConstruction(context);

        var reserialisedContext =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .LoadContext(_metaStoryText)
                   .Initialise()
                   .Serialise()
                   .Match(v => v, e => throw e);


        // Act
        // ===

        // The MetaStory declaration is made outside the producer because the MetaStory depends on how the producer is called (here the choices could be different)
        hooks.StartMetaStory("MetaStory recorded by hooks");

        // Run the code and produce the MetaStory from the called hooks
        Debug.WriteLine("");
        Debug.WriteLine("Producer method output :");
        ProducerMethod(hooks);

        MetaStory generatedMetaStory = hooks.EndMetaStory();


        // Assert
        // ======
        generatedMetaStory.Should().NotBeNull();

        var contextBuiltFromHooks =
            context.Serialise()
                   .Match(v => v, e => throw e);

        Debug.WriteLine("");
        Debug.WriteLine("Final serialised context :");
        Debug.WriteLine(contextBuiltFromHooks);

        var originalContext = _metaStoryText;
        DiffAssert.DiffIfNotEqual(originalContext, reserialisedContext, contextBuiltFromHooks);
    }

    [TestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory -> Story")]
    public async Task WhileLoop_StoryExtractionTest()
    {
        // Arrange
        // =======
        Context context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .Initialise();

        MetaStoryHookOrchestratorForConstruction hooks = new MetaStoryHookOrchestratorForConstruction(context);

        // Everything necessary to run the MetaStory
        ExpressionEvalator evalator = new(context.System);
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);
        StoryTestRunner runner = new(executor, dependencies);


        // Act
        // ===

        // The MetaStory declaration is made outside the producer because the MetaStory depends on how the producer is called (here the choices could be different)
        hooks.StartMetaStory("MetaStory recorded by hooks");

        // Run the code and produce the MetaStory from the called hooks
        Debug.WriteLine("");
        Debug.WriteLine("Producer method output :");
        ProducerMethod(hooks);

        hooks.EndMetaStory();

        Story story = runner.Run("MetaStory recorded by hooks");


        // Assert
        // ======
        string events = story.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();

        Debug.WriteLine("");
        Debug.WriteLine("Final serialised events :");
        Debug.WriteLine(events);

        await Verify(events);
    }

}
