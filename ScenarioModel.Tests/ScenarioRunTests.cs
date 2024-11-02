using FluentAssertions;
using ScenarioModel.Execution.Dialog;
using ScenarioModel.Execution.Events;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Interpolation;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.Objects.ScenarioObjects.DataClasses;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using System.Data;

namespace ScenarioModel.Tests;

[TestClass]
public class ScenarioRunTests
{
    private string _scenarioText = """
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
                    Text "I am now Alice ! "
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
    public void ScenarioWithChooseAndIfTest()
    {
        // Arrange
        // =======
        Context context =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .LoadContext<HumanReadableSerialiser>(_scenarioText)
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
                (DialogNode dialogNode) =>
                {
                    // Do nothing
                },
                (IfNode ifNode) =>
                {
                    // Do nothing
                },
                (JumpNode jumpNode) =>
                {
                    // Do nothing
                },
                (StateTransitionNode transitionNode) =>
                {
                    // Do nothing
                }
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
}
