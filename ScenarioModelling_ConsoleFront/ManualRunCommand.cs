using ScenarioModel;
using ScenarioModel.Execution;
using ScenarioModel.Execution.Dialog;
using ScenarioModel.Execution.Events;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Interpolation;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModel.Objects.ScenarioObjects.DataClasses;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling_ConsoleFront.NodeHandlers;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

// colors
// https://spectreconsole.net/appendix/colors

public class ManualRunCommand : Command<ManualRunCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[file]")]
        public string? File { get; set; }

        [CommandArgument(1, "[scenarioname]")]
        public string? ScenarioName { get; set; }

    }

    public override int Execute([NotNull] CommandContext commandContext, [NotNull] Settings settings)
    {
        string scenarioText = ReadScenarioTest(settings);
        string filePath = DetermineFilePath(settings);

        // Create the context from the scenario text
        Context context =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .SetResourceFolder(filePath)
                   .LoadContext<HumanReadableSerialiser>(scenarioText)
                   .Initialise();

        DialogExecutor executor = new(context);
        StringInterpolator interpolator = new(context.System);
        ExpressionEvalator evalator = new(context.System);
        EventGenerationDependencies dependencies = new EventGenerationDependencies(interpolator, evalator, executor, context);

        DialogNodeHandler dialogNodeHandler = new() { Dependencies = dependencies };
        StateTransitionNodeHandler stateTransitionNodeHandler = new() { Dependencies = dependencies };
        JumpNodeHandler jumpNodeHandler = new() { Dependencies = dependencies };
        IfNodeHandler ifNodeHandler = new() { Dependencies = dependencies };
        ChooseNodeHandler chooseNodeHandler = new() { Dependencies = dependencies };

        // Initialize the scenario
        var scenarioRun = executor.StartScenario(settings.ScenarioName ?? "");

        // Generate first node
        var node = executor.NextNode();

        while (node != null)
        {
            if (!executor.IsLastEventOfType<JumpEvent>())
            {
                // Custom style for non-jump events
                AnsiConsole.Write(new Rule { Style = "grey dim" });
            }

            switch (node)
            {
                case DialogNode dialogNode:
                    dialogNodeHandler.Manage(dialogNode);
                    break;
                case ChooseNode chooseNode:
                    chooseNodeHandler.Manage(chooseNode);
                    break;
                case StateTransitionNode transitionNode:
                    stateTransitionNodeHandler.Manage(transitionNode);
                    break;
                case JumpNode jumpNode:
                    jumpNodeHandler.Manage(jumpNode);
                    break;
                case IfNode ifNode:
                    ifNodeHandler.Manage(ifNode);
                    break;
                default:
                    throw new Exception($"Unknown node type : {node.GetType().Name}");
            }

            // Generate the next node from the previous state
            node = executor.NextNode();
        }

        return 0;
    }

    private static string DetermineFilePath(Settings settings)
    {
        string? filePath = Path.GetDirectoryName(Path.GetFullPath(settings.File ?? ""));

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new Exception($"Could not determine path to given file : '{settings.File}'");
        }

        return filePath;
    }

    private static string ReadScenarioTest(Settings settings)
    {
        if (!File.Exists(settings.File))
        {
            throw new FileNotFoundException($"File not found: {settings.File}");
        }

        return File.ReadAllText(settings.File);
    }
}
