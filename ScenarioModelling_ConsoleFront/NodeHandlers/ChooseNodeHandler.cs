using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.Scenario;
using Spectre.Console;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

internal class ChooseNodeHandler : NodeHandler<ChooseNode, ChoiceSelectedEvent>
{
    public override void Handle(ChooseNode node, ChoiceSelectedEvent e)
    {
        AnsiConsole.Markup($"[blue]Options : {node.TargetNodeNames.CommaSeparatedList()}[/]\n");

        // Prompt for input
        var promptResult = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select an option")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
            .AddChoices(node.Choices.Select(n => n.Text))
        );

        e.Choice = node.Choices.Where(n => n.Text.IsEqv(promptResult)).Select(s => s.NodeName).First();
        AnsiConsole.Markup($"[blue]{e.Choice}[/]\n");
    }
}
