using ScenarioModelling.CoreObjects.Expressions.Evaluation;
using ScenarioModelling.CoreObjects.Interpolation;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.TestDataAndTools;
using System.Data;

namespace ScenarioModelling.Execution.Tests;

[TestClass]
[UsesVerify]
public partial class StoryRunTests
{
    [Ignore]
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

        TestContainer container = new();

        container.Context
                 .UseSerialiser<ContextSerialiser>()
                 .LoadContext(MetaStory)
                 .Initialise();

        ExpressionEvalator evalator = container.GetService<ExpressionEvalator>();
        DialogExecutor executor = container.GetService<DialogExecutor>();
        StringInterpolator interpolator = container.GetService<StringInterpolator>();
        EventGenerationDependencies dependencies = container.GetService<EventGenerationDependencies>();
        StoryTestRunner runner = container.GetService<StoryTestRunner>();


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

        TestContainer container = new();

        container.Context
                 .UseSerialiser<ContextSerialiser>()
                 .LoadContext(MetaStory)
                 .Initialise();

        ExpressionEvalator evalator = container.GetService<ExpressionEvalator>();
        DialogExecutor executor = container.GetService<DialogExecutor>();
        StringInterpolator interpolator = container.GetService<StringInterpolator>();
        EventGenerationDependencies dependencies = container.GetService<EventGenerationDependencies>();
        StoryTestRunner runner = container.GetService<StoryTestRunner>();

        // Act
        // ===
        Story story = runner.Run("NameSwappingPuns");


        // Assert
        // ======
        string serialisedStory = story.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();
        await Verify(serialisedStory);

    }
}
