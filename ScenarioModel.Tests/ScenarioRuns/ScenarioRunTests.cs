using ScenarioModel.Execution;
using ScenarioModel.Execution.Dialog;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Interpolation;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using System.Data;

namespace ScenarioModel.Tests.ScenarioRuns;

[TestClass]
[UsesVerify]
public partial class ScenarioRunTests
{
    [TestMethod]
    [TestCategory("ScenarioRuns")]
    public async Task ScenarioWithChooseJumpAndIfTest()
    {
        // Arrange
        // =======
        string scenario = """
            Entity Actor {
                State Bob
                CharacterStyle "Red"
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
                   .LoadContext(scenario)
                   .Initialise();
        ExpressionEvalator evalator = new(context.System);
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);
        ScenarioTestRunner runner = new(executor, dependencies, choicesByNodeName);


        // Act
        // ===
        ScenarioRun scenarioRun = runner.Run("First");


        // Assert
        // ======
        string target = scenarioRun.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();
        await Verify(target);

    }

    [TestMethod]
    [TestCategory("ScenarioRuns")]
    public async Task ScenarioWithLoopTest()
    {
        // Arrange
        // =======
        string scenario = """
            Entity Actor {
                State "Amy Stake"
                CharacterStyle "Red"
            }

            SM Name {
                "Amy Stake"-> "Brock Lee" : ChangeName
                "Brock Lee"-> "Clara Nett" : ChangeName
                "Clara Nett"-> "Dee Zaster" : ChangeName
            }

            Scenario NameSwappingPuns {
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
                   .LoadContext(scenario)
                   .Initialise();
        ExpressionEvalator evalator = new(context.System);
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);
        ScenarioTestRunner runner = new(executor, dependencies);

        // Act
        // ===
        ScenarioRun scenarioRun = runner.Run("NameSwappingPuns");


        // Assert
        // ======
        string target = scenarioRun.Events.Select(e => e?.ToString() ?? "").BulletPointList().Trim();
        await Verify(target);

    }
}
