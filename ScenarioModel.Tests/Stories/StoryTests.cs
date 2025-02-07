using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Exhaustiveness.Common;
using ScenarioModelling.Expressions.Evaluation;
using ScenarioModelling.Interpolation;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.Tests.Stories;
using System.Data;

namespace ScenarioModelling.Tests.storys;

[TestClass]
[UsesVerify]
public partial class StoryRunTests
{
    [TestMethod]
    [TestCategory("MetaStory -> Story")]
    public async Task ChooseJumpStoryReplayTest()
    {
        // Arrange
        // =======
        string MetaStory = """
            Entity Actor {
                State Bob
                CharacterStyle "Red"
            }

            StateMachine Name {
                Bob -> Alice : ChangeName
                Alice -> Bob : ChangeName
            }

            MetaStory First {
                Dialog {
                    Character Actor
                    Text "Hello"
                }
                Dialog SayName {
                    Character Actor
                    Text "My name is {Actor.State}"
                }
                Choose ChooseToRepeat {
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
                    SayName
                }
                Dialog GoodBye {
                    Text "Bubye (Actor called {Actor.State} in the end)"
                }
            }
        """;

        Dictionary<string, Queue<string>> choicesByNodeName = new()
        {
            { "ChooseToRepeat", new Queue<string>(["Change name and repeat", "Change name and repeat", "Change name and repeat", "Ciao"]) }
        };

        Context context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .LoadContext(MetaStory)
                   .Initialise();
        ExpressionEvalator evalator = new(context.System);
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);
        StoryTestRunner runner = new(executor, dependencies, choicesByNodeName);


        // Act
        // ===
        Story story = runner.Run("First");


        // Assert
        // ======
        string serialisedStory = story.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();
        await Verify(serialisedStory);

    }

    [TestMethod]
    [TestCategory("MetaStory -> Story")]
    public async Task LoopStoryReplyTest()
    {
        // Arrange
        // =======
        string MetaStory = """
            Entity Actor {
                State "Amy Stake"
                CharacterStyle "Red"
            }

            StateMachine Name {
                "Amy Stake"-> "Brock Lee" : ChangeName
                "Brock Lee"-> "Clara Nett" : ChangeName
                "Clara Nett"-> "Dee Zaster" : ChangeName
            }

            MetaStory NameSwappingPuns {
                Dialog SayName {
                    Character Actor
                    Text "Hi, this is {Actor.State}"
                }
                While <Actor.State != "Dee Zaster"> {
                    if <Actor.State == "Amy Stake"> {
                        Dialog {
                            Character Actor
                            Text "Amy's name was well chosen"
                        }
                    }
                    if <Actor.State == "Brock Lee"> {
                        Dialog {
                            Character Actor
                            Text "Brock didn't like his vegies"
                        }
                    }
                    if <Actor.State == "Clara Nett"> {
                        Dialog {
                            Character Actor
                            Text "Clara hated music"
                        }
                    }
                    Transition Change {
                        Actor: ChangeName
                    }
                }
                if <Actor.State == "Dee Zaster"> {
                    Dialog {
                        Character Actor
                        Text "Well, that went well !"
                    }
                }
            }
        """;

        Context context =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .LoadContext(MetaStory)
                   .Initialise();
        ExpressionEvalator evalator = new(context.System);
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);
        StoryTestRunner runner = new(executor, dependencies);

        // Act
        // ===
        Story story = runner.Run("NameSwappingPuns");


        // Assert
        // ======
        string serialisedStory = story.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();
        await Verify(serialisedStory);

    }
}
