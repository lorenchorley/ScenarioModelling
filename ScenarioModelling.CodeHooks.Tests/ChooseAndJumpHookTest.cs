using FluentAssertions;
using ScenarioModelling.CodeHooks;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.StoryNodes.DataClasses;
using ScenarioModelling.Execution;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;
using ScenarioModelling.TestDataAndTools;
using ScenarioModelling.TestDataAndTools.CodeHooks;
using System.Diagnostics;

namespace ScenarioModelling.Tests.HookTests;

[TestClass]
[UsesVerify]
public partial class ChooseAndJumpHookTest
{
    private string _metaStoryText = """
        Entity Actor {
          State Bob
        }

        StateMachine Name {
          State Bob
          State Alice
          Bob -> Alice : ChangeName
          Alice -> Bob : ChangeName
        }

        MetaStory "MetaStory recorded by hooks" {
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

    void ProducerMethod(MetaStoryHookOrchestrator hooks, Queue<string> choices)
    {
        hooks.DefineMetaState(configuration =>
        {
            configuration.Entity("Actor")
                         .SetState("Bob");

            configuration.StateMachine("Name")
                         .WithTransition("Bob", "Alice", "ChangeName")
                         .WithTransition("Alice", "Bob", "ChangeName");
        });


        string ActorName = "Bob";


        hooks.Dialog("The actor {Actor.State} says hello and introduces themselves")
             .WithCharacter("Actor")
             .BuildAndRegister();

        Debug.WriteLine($"{ActorName}: Hello");
        Debug.WriteLine($"{ActorName}: My name is {ActorName}");


        hooks.Choose(new ChoiceList() { ("Change", "Change name and repeat") })
             .GetConditionHook(out ArbitraryBranchingHook φ)
             .SetId("LoopStart")
             .WithJump("GoodBye", "Ciao")
             .Build();

        while (φ(choices.Dequeue()) == "Change name and repeat") // TODO choose hook needs to reregister the choice event
        {
            hooks.Transition("Actor", "ChangeName")
                 .SetId("Change")
                 .BuildAndRegister();

            if (ActorName == "Bob")
                ActorName = "Alice";
            else
                ActorName = "Bob";


            hooks.If(@"Actor.State != ""Alice""")
                 .GetConditionHook(out BifurcatingHook ψ)
                 .GetEndBlockHook(out BlockEndHook ifBlockEndHook)
                 .Build();

            if (ψ(ActorName == "Alice"))
            {
                hooks.Dialog("Actor", "The actor declares themselves to be Alice")
                     .BuildAndRegister();

                Debug.WriteLine($"{ActorName}: I am now Alice !");

                ifBlockEndHook();
            }

            hooks.Jump("LoopStart")
                 .BuildAndRegister();
        }

        hooks.Dialog("Actor", "The actor {Actor.State} says goodbye")
             .WithId("GoodBye")
             .BuildAndRegister();

        Debug.WriteLine($"{ActorName}: Bubye");
    }

    [TestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory Construction")]
    public void ChooseAndIf_MetaStoryConstructionTest()
    {
        // Arrange
        // =======
        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();

        Context context =
            scope.Context
                 .UseSerialiser<CustomContextSerialiser>()
                 .Initialise();

        MetaStoryHookOrchestrator hooks = scope.GetService<MetaStoryHookOrchestratorForConstruction>();

        Queue<string> choices = new();
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Ciao");

        using ScenarioModellingContainer reserialisationContainer = new();
        using var reserialisationScope = reserialisationContainer.StartScope();

        var reserialisedContext =
            reserialisationScope.Context
                                .UseSerialiser<CustomContextSerialiser>()
                                .LoadContext(_metaStoryText)
                                .Initialise()
                                .Serialise()
                                .Match(v => v, e => throw e);


        // Act
        // ===

        // The MetaStory declaration is made outside the producer because the MetaStory depends on how the producer is called (here the choices could be different)
        hooks.StartMetaStory(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);

        // Run the code and produce the MetaStory from the called hooks

        Debug.WriteLine("");
        Debug.WriteLine("Producer method output :");
        ProducerMethod(hooks, choices);

        (MetaStory generatedMetaStory, _) = hooks.EndMetaStory();


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
        DiffAssert.DiffIfNotEqual(originalContext.Trim(), reserialisedContext.Trim(), contextBuiltFromHooks.Trim());
    }

    [TestMethod]
    [TestCategory("Code Hooks"), TestCategory("MetaStory -> Story")]
    [Ignore("This tests loops infinitely because the scenario is not correctly constructed")]
    public async Task ChooseAndIf_StoryExtractionTest()
    {
        // Arrange
        // =======
        using TestContainer container = new();
        using var scope = container.StartScope();

        Context context =
            scope.Context
                 .UseSerialiser<CustomContextSerialiser>()
                 .Initialise();

        MetaStoryHookOrchestrator hooks = scope.GetService<MetaStoryHookOrchestratorForConstruction>();
        StoryTestRunner runner = scope.GetService<StoryTestRunner>();

        Queue<string> choices = new();
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Ciao");

        using ScenarioModellingContainer reserialisationContainer = new();
        using var reserialisationScope = container.StartScope();

        var reserialisedContext =
            reserialisationScope.Context
                                .UseSerialiser<CustomContextSerialiser>()
                                .LoadContext(_metaStoryText)
                                .Initialise()
                                .Serialise()
                                .Match(v => v, e => throw e);


        // Act
        // ===

        // The MetaStory declaration is made outside the producer because the MetaStory depends on how the producer is called (here the choices could be different)
        hooks.StartMetaStory(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);

        // Run the code and produce the MetaStory from the called hooks

        Debug.WriteLine("");
        Debug.WriteLine("Producer method output :");
        ProducerMethod(hooks, choices);

        (MetaStory generatedMetaStory, Story hookGeneratedStory) = hooks.EndMetaStory();

        Story rerunStory = runner.Run(ProgressiveCodeHookTestDataProviderAttribute.PrimaryMetaStoryName);


        // Assert
        // ======
        generatedMetaStory.Should().NotBeNull();

        string hookGeneratedEvents = hookGeneratedStory.Events.GetEnumerable().Select(e => e?.ToString() ?? "").BulletPointList().Trim();
        string rerunEvents = rerunStory.Events.GetEnumerable().Select(e => e?.ToString() ?? "").BulletPointList().Trim();

        DiffAssert.DiffIfNotEqual(hookGeneratedEvents, rerunEvents);

        Debug.WriteLine("");
        Debug.WriteLine("Final serialised events :");
        Debug.WriteLine(rerunEvents);

        await Verify(rerunEvents);
    }
}
