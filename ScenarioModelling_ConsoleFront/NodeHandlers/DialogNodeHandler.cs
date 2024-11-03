using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModelling_ConsoleFront.NodeHandlers.BaseClasses;
using Spectre.Console;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

[NodeLike<INodeHandler, DialogNode>]
internal class DialogNodeHandler : NodeHandler<DialogNode, DialogEvent>
{
    public override void Handle(DialogNode node, DialogEvent e)
    {
        if (Dependencies.Executor.IsLastEventOfType<DialogEvent>())
        {
            Console.ReadKey(); // Put a pause between dialogs
        }

        if (!string.IsNullOrEmpty(node.Character))
        {
            var characterEntity = Dependencies.Context.System.Entities.Where(e => e.Name.IsEqv(node.Character)).FirstOrDefault();

            if (characterEntity == null)
            {
                throw new Exception($"Character {node.Character} not found on dialog {node.Name}{node.LineInformation}");
            }

            AnsiConsole.MarkupLine($"[{characterEntity.CharacterStyle}]{node.Character}[/]");
        }

        AnsiConsole.Markup($"[green]{e.Text}[/]\n");

    }
}
