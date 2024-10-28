using ScenarioModel;
using ScenarioModel.Execution.Dialog;
using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.Scenario;
using Spectre.Console;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

internal class DialogNodeHandler : NodeHandler<DialogNode, DialogEvent>
{
    public override void Handle(DialogNode node, DialogEvent e)
    {
        if (DialogFactory.IsLastEventOfType<DialogEvent>())
        {
            Console.ReadKey(); // Put a pause between dialogs
        }

        string text = node.TextTemplate;
        text = Interpolator.ReplaceInterpolations(text);

        if (!string.IsNullOrEmpty(node.Character))
        {
            var characterEntity = Context.System.Entities.Where(e => e.Name.IsEqv(node.Character)).FirstOrDefault();

            if (characterEntity == null)
            {
                throw new Exception($"Character {node.Character} not found on of dialog {node.Name}");
            }

            AnsiConsole.MarkupLine($"[{characterEntity.CharacterStyle}]{node.Character}[/]");
        }

        AnsiConsole.Markup($"[green]{text}[/]\n");

        e.Text = text;
    }
}
