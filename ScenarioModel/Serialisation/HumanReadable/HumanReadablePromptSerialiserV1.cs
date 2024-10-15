using LanguageExt;
using LanguageExt.Common;
using ScenarioModel.ScenarioObjects;
using ScenarioModel.Serialisation.HumanReadable.Interpreter;
using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.States;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable;

public class HumanReadablePromptSerialiserV1 : ISerialiser
{
    const string _indent = "  ";

    public Result<Context> DeserialiseContext(string text)
    {
        HumanReadableInterpreter interpreter = new();
        var result = interpreter.Parse(text);

        if (result.HasErrors)
        {
            return new Result<Context>(new Exception(string.Join('\n', result.Errors)));
        }

        SemanticContextBuilder contextBuilder = new();

        return contextBuilder.Build(result.Tree);
    }

    public Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context)
    {
        var newContext = DeserialiseContext(text);

        return newContext.Match(Succ: c => context.Incorporate(c), Fail: e => new Result<Context>(e));
    }

    public Result<string> SerialiseContext(Context context)
    {
        StringBuilder sb = new();

        SerialiseSystem(sb, context.System);

        foreach (var scenario in context.Scenarios)
        {
            SerialiseScenario(sb, scenario);
        }

        return sb.ToString();
    }

    public void SerialiseScenario(StringBuilder sb, Scenario scenario)
    {
        sb.AppendLine($"Scenario {scenario.Name} {{");

        foreach (var step in scenario.Steps)
        {
            if (step is ITransitionNode transitionNode)
            {
                foreach (var target in transitionNode.TargetNodeNames)
                {
                    sb.AppendLine($"{_indent}{step.Name} -> {target}");
                }

                continue;
            }

            if (step is ChooseNode chooseNode)
            {
                WriteChooseNode(sb, chooseNode, _indent);
                continue;
            }

            if (step is DialogNode dialogNode)
            {
                WriteDialogNode(sb, dialogNode, _indent);
                continue;
            }

            if (step is JumpNode jumpNode)
            {
                WriteJumpNode(sb, jumpNode, _indent);
                continue;
            }
        }

        sb.AppendLine($"}}");
        sb.AppendLine($"");
    }

    private static void WriteChooseNode(StringBuilder sb, ChooseNode node, string indent)
    {
        sb.AppendLine($"{indent}Choose {node.Name} {{");

        foreach (var option in node.TargetNodeNames)
        {
            sb.AppendLine($"{indent}{_indent}{option}");
        }

        sb.AppendLine($"{indent}}}");
        sb.AppendLine($"");
    }
    
    private static void WriteDialogNode(StringBuilder sb, DialogNode node, string indent)
    {
        sb.AppendLine($"{indent}Dialog {node.Name} {{");

        sb.AppendLine($"{indent}{_indent}Text {node.TextTemplate}");
        
        if (node.Character != null)
        {
            sb.AppendLine($"{indent}{_indent}Character {node.Character}");
        }

        sb.AppendLine($"{indent}}}");
        sb.AppendLine($"");
    }
    
    private static void WriteJumpNode(StringBuilder sb, JumpNode node, string indent)
    {
        sb.AppendLine($"{indent}Jump {node.Name} {{");

        sb.AppendLine($"{indent}{_indent}{node.Target}");

        sb.AppendLine($"{indent}}}");
        sb.AppendLine($"");
    }

    public void SerialiseSystem(StringBuilder sb, System system)
    {
        foreach (var entity in system.Entities)
        {
            WriteEntity(sb, system, "", entity);
        }

        foreach (var entityType in system.EntityTypes)
        {
            WriteEntityType(sb, "", entityType);
        }

        foreach (var stateMachine in system.StateMachines)
        {
            WriteStateMachine(sb, "", stateMachine);
        }
    }

    private static void WriteEntity(StringBuilder sb, System system, string indent, Entity entity)
    {
        sb.AppendLine($"{indent}Entity {AddQuotes(entity.Name)} {{");

        if (entity.EntityType != null)
        {
            sb.AppendLine($"{indent}{_indent}EntityType {entity.EntityType.Name}");
        }

        if (entity.State != null)
        {
            WriteSMState(sb, indent + _indent, entity.State);
        }

        foreach (var aspectType in entity.Aspects)
        {
            WriteAspect(sb, indent + _indent, aspectType);
        }

        foreach (var relation in entity.Relations)
        {
            WriteRelation(sb, indent + _indent, relation);
        }

        sb.AppendLine($"{indent}}}");
        sb.AppendLine($"");
    }

    private static void WriteRelation(StringBuilder sb, string indent, SystemObjects.Relations.Relation relation)
    {
        sb.AppendLine($"{indent}{relation.LeftEntity} -> {relation.RightEntity}");
    }

    private static void WriteAspect(StringBuilder sb, string indent, Aspect aspect)
    {
        sb.AppendLine($"{indent}Aspect {AddQuotes(aspect.Name)} {{");

        sb.AppendLine($"{indent}}}");
        sb.AppendLine($"");
    }

    private static void WriteEntityType(StringBuilder sb, string indent, EntityType entityType)
    {
        sb.AppendLine($"{indent}EntityType {AddQuotes(entityType.Name)} {{");

        if (entityType.StateType != null)
        {
            sb.AppendLine($"{indent}{_indent}SM {AddQuotes(entityType.StateType.Name)}");
        }

        sb.AppendLine($"{indent}}}");
        sb.AppendLine($"");
    }

    private static void WriteStateMachine(StringBuilder sb, string indent, StateMachine stateType)
    {
        sb.AppendLine($"{indent}SM {AddQuotes(stateType.Name)} {{");

        foreach (var state in stateType.States)
        {
            WriteSMState(sb, indent + _indent, state);
        }

        foreach (var state in stateType.States)
        {
            foreach (var transition in state.Transitions)
            {
                WriteSMTransition(sb, indent + _indent, stateType.States, state, transition);
            }
        }

        sb.AppendLine($"{indent}}}");
        sb.AppendLine($"");
    }

    private static void WriteSMState(StringBuilder sb, string indent, State state)
    {
        //sb.AppendLine($"{indent}State {state.Name} {{ SM {state.StateMachine.Name} }}");
        sb.AppendLine($"{indent}State {AddQuotes(state.Name)}");
    }

    private static void WriteSMTransition(StringBuilder sb, string indent, List<State> states, State state, Transition transition)
    {
        sb.AppendLine($"{indent}{transition.SourceState} -> {transition.DestinationState} : {transition.Name}");
    }

    //private static void WriteAspect(StringBuilder sb, string indent, AspectType aspectType)
    //{
    //    sb.AppendLine($"{indent}AspectType {AddQuotes(aspectType.Name)} {{");

    //    sb.AppendLine($"{indent}}}");
    //    sb.AppendLine($"");
    //}

    private static string AddQuotes(string str)
    {
        if (str.Contains(' '))
        {
            return $"\"{str}\"";
        }

        return str;
    }
}
