﻿using ScenarioModel;
using ScenarioModel.Execution.Dialog;
using ScenarioModel.Execution.Events;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Interpolation;
using ScenarioModel.References;
using ScenarioModel.ScenarioObjects;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModel.SystemObjects.States;
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
        if (!File.Exists(settings.File))
        {
            throw new FileNotFoundException($"File not found: {settings.File}");
        }

        var scenarioText = File.ReadAllText(settings.File);

        Context context =
            Context.New()
                   .UseSerialiser<HumanReadablePromptSerialiser>()
                   .LoadContext<HumanReadablePromptSerialiser>(scenarioText)
                   .Initialise();

        StringInterpolator interpolator = new(context.System);

        AnsiConsole.Markup($"[blue]{context.Serialise<HumanReadablePromptSerialiser>()}[/]");

        DialogFactory dialogFactory = new(context);
        var scenario = dialogFactory.StartScenario(settings.ScenarioName ?? "");

        var node = dialogFactory.NextNode();

        while (true)
        {
            if (!dialogFactory.IsLastEventOfType<JumpEvent>())
            {
                AnsiConsole.Write(new Rule { Style = "grey dim" });
            }

            if (node is DialogNode dialogNode)
            {
                if (dialogFactory.IsLastEventOfType<DialogEvent>())
                {
                    Console.ReadKey(); // Put a pause between dialogs
                }

                var e = dialogNode.GenerateEvent();

                string text = dialogNode.TextTemplate;

                text = interpolator.ReplaceInterpolations(text);

                if (!string.IsNullOrEmpty(dialogNode.Character))
                {
                    var characterEntity = context.System.Entities.Where(e => e.Name.IsEqv(dialogNode.Character)).FirstOrDefault();

                    if (characterEntity == null)
                    {
                        throw new Exception($"Character {dialogNode.Character} not found on of dialog {dialogNode.Name}");
                    }

                    AnsiConsole.MarkupLine($"[{characterEntity.CharacterStyle}]{dialogNode.Character}[/]");
                }

                AnsiConsole.Markup($"[green]{text}[/]\n");

                e.Text = text;

                dialogFactory.RegisterEvent(e);
            }
            else if (node is ChooseNode chooseNode)
            {
                AnsiConsole.Markup($"[blue]Options : {chooseNode.TargetNodeNames.CommaSeparatedList()}[/]\n");

                var e = chooseNode.GenerateEvent();

                // Prompt for input
                var promptResult = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("Select an option")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(chooseNode.Choices.Select(n => n.Text))
                );

                e.Choice = chooseNode.Choices.Where(n => n.Text.IsEqv(promptResult)).Select(s => s.NodeName).First();
                AnsiConsole.Markup($"[blue]{e.Choice}[/]\n");

                dialogFactory.RegisterEvent(e);
            }
            else if (node is StateTransitionNode transitionNode)
            {
                var e = transitionNode.GenerateEvent();

                IStateful statefulObject =
                    transitionNode.StatefulObject
                                  ?.ResolveReference(context.System)
                                  .Match(
                                        Some: obj => obj,
                                        None: () => throw new Exception("Stateful object not found")
                                    )
                                  ?? throw new Exception("StatefulObject was null");

                if (statefulObject.State == null)
                {
                    if (statefulObject is INameful nameful)
                    {
                        throw new Exception($"Attempted state transition {transitionNode.TransitionName} on {nameful.Name} but no state set initially");
                    }
                    else
                    {
                        throw new Exception($"Attempted state transition {transitionNode.TransitionName} on object but no state set initially");
                    }
                }

                e.InitialState = new StateReference() { StateName = statefulObject.State.Name };

                if (!statefulObject.State.TryTransition(transitionNode.TransitionName, statefulObject))
                {
                    throw new Exception($"State transition failed, no such transition {transitionNode.TransitionName} on state {statefulObject.State.Name} of type {statefulObject.State.StateMachine.Name}");
                }

                e.FinalState = new StateReference() { StateName = statefulObject.State.Name };

                AnsiConsole.Markup($"[white]{e.StatefulObject} : {e.InitialState} -> {e.FinalState} (Via transition : {e.TransitionName})[/]\n");

                dialogFactory.RegisterEvent(e);
            }
            else if (node is JumpNode jumpNode)
            {
                var e = jumpNode.GenerateEvent();

                dialogFactory.RegisterEvent(e);
            }
            else if (node is IfNode ifNode)
            {
                var e = ifNode.GenerateEvent();

                ExpressionEvalatorVisitor visitor = new(context.System);

                var result = ifNode.Expression.Accept(visitor);
                e.IfBlockRun = true;

                dialogFactory.RegisterEvent(e);
            }
            else
            {
                throw new Exception($"Unknown node type : {node.GetType().Name}");
            }

            //AnsiConsole.Markup($"\n[darkgreen]Event: {Markup.Escape(dialogFactory.GetLastEvent().ToString())}[/]\n");
            node = dialogFactory.NextNode();
            if (node == null)
            {
                // End
                break;
            }

        }

        return 0;
    }
}
