using FluentAssertions;
using ScenarioModel.Execution.Dialog;
using ScenarioModel.Execution.Events;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Interpolation;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using System.Data;

namespace ScenarioModel.Tests;

[TestClass]
public class ScenarioRunTests
{
    private string _chooseJumpAndIfScenario = """
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
            Choose {
                Change "Change name and repeat"
                GoodBye "Ciao"
            }

            Transition Change {
                Actor : ChangeName
            }
            if <Actor.State != "Alice"> {
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

    [TestMethod]
    [TestCategory("ScenarioRuns")]
    public void ScenarioWithChooseJumpAndIfTest()
    {
        // Arrange
        // =======
        Context context =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .LoadContext(_chooseJumpAndIfScenario)
                   .Initialise();

        DialogExecutor executor = new(context);
        StringInterpolator interpolator = new(context.System);
        ExpressionEvalator evalator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);

        Queue<string> choices = new();
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Change name and repeat");
        choices.Enqueue("Ciao");


        // Act
        // ===

        // Initialize the scenario
        var scenarioRun = executor.StartScenario("First");

        // Generate first node
        IScenarioNode? node = null;

        while ((node = executor.NextNode()) != null)
        {
            IScenarioEvent e = node.GenerateUntypedEvent(dependencies);

            node.ToOneOf().Switch(
                (ChooseNode chooseNode) =>
                {
                    string selection = choices.Dequeue();
                    ((ChoiceSelectedEvent)e).Choice = chooseNode.Choices.Where(n => n.Text.IsEqv(selection)).Select(s => s.NodeName).First();
                },
                (DialogNode dialogNode) => { },
                (IfNode ifNode) => { },
                (JumpNode jumpNode) => { },
                (StateTransitionNode transitionNode) => { },
                (WhileNode whileNode) => { }
            );

            executor.RegisterEvent(e);
        }


        // Assert
        // ======
        scenarioRun.Events
                   .OfType<DialogEvent>()
                   .Select(d => d.Text.Trim())
                   .ToList()
                   .Should()
                   .BeEquivalentTo(["My name is Bob", "My name is Alice", "I am now Alice !", "My name is Bob", "My name is Alice", "Bubye (Actor called Alice in the end)"]);



    }
    private string _loopScenario = """
        Entity Actor {
            State "Amy Stake"
            CharacterStyle "Red"
        }

        SM Name {
            "Amy Stake" -> "Brock Lee" : ChangeName
            "Brock Lee" -> "Clara Nett" : ChangeName
            "Clara Nett" -> "Dee Zaster" : ChangeName
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
                    Actor : ChangeName
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

    [TestMethod]
    [TestCategory("ScenarioRuns")]
    public void ScenarioWithLoopTest()
    {
        // Arrange
        // =======
        Context context =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .LoadContext(_loopScenario)
                   .Initialise();

        DialogExecutor executor = new(context);
        StringInterpolator interpolator = new(context.System);
        ExpressionEvalator evalator = new(context.System);
        EventGenerationDependencies dependencies = new(interpolator, evalator, executor, context);


        // Act
        // ===

        // Initialize the scenario
        var scenarioRun = executor.StartScenario("NameSwappingPuns");

        // Generate first node
        IScenarioNode? node = null;

        while ((node = executor.NextNode()) != null)
        {
            IScenarioEvent e = node.GenerateUntypedEvent(dependencies);

            node.ToOneOf().Switch(
                (ChooseNode chooseNode) => { },
                (DialogNode dialogNode) => { },
                (IfNode ifNode) => { },
                (JumpNode jumpNode) => { },
                (StateTransitionNode transitionNode) => { },
                (WhileNode whileNode) => { }
            );

            executor.RegisterEvent(e);
        }


        // Assert
        // ======
        scenarioRun.Events
                   .OfType<DialogEvent>()
                   .Select(d => d.Text.Trim())
                   .ToList()
                   .Should()
                   .BeEquivalentTo(["Hi, this is Amy Stake", "Amy's name was well chosen", "Brock didn't like his vegies", "Clara hated music", "Well, that went well !"]);



    }
}
