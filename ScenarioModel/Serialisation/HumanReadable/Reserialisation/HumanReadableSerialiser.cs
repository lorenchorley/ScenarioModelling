using LanguageExt.Common;
using ScenarioModel.Expressions.Validation;
using ScenarioModel.Objects.Scenario;
using ScenarioModel.Objects.System.Entities;
using ScenarioModel.Objects.System.Relations;
using ScenarioModel.Objects.System.States;
using ScenarioModel.Serialisation.HumanReadable.Interpreter;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation;

public class HumanReadableSerialiser : ISerialiser
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

        return contextBuilder.Build(result.ParsedObject);
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
            WriteScenarioStep(sb, scenario, step, _indent);
        }

        sb.AppendLine($"}}");
        sb.AppendLine($"");
    }

    private void WriteScenarioStep(StringBuilder sb, Scenario scenario, IScenarioNode step, string indent)
    {
        if (step is ITransitionNode transitionNode)
        {
            foreach (var target in transitionNode.TargetNodeNames)
            {
                sb.AppendLine($"{indent}{step.Name} -> {target}");
            }

            return;
        }

        if (step is ChooseNode chooseNode)
        {
            WriteChooseNode(sb, chooseNode, indent);
            return;
        }

        if (step is DialogNode dialogNode)
        {
            WriteDialogNode(sb, dialogNode, indent);
            return;
        }

        if (step is JumpNode jumpNode)
        {
            WriteJumpNode(sb, jumpNode, indent);
            return;
        }

        if (step is StateTransitionNode stateTransitionNode)
        {
            WriteStateTransitionNode(sb, scenario, stateTransitionNode, indent);
            return;
        }

        if (step is IfNode ifNode)
        {
            WriteIfNode(sb, scenario, ifNode, indent);
            return;
        }

        throw new NotImplementedException($"Unhandle scenario node type : {step.GetType().Name}");
    }

    private void WriteIfNode(StringBuilder sb, Scenario scenario, IfNode node, string indent)
    {
        ExpressionSerialiser visitor = new(scenario.System);
        var result = (string)node.Expression.Accept(visitor);    

        sb.AppendLine($"{indent}If <{result}> {{"); // TODO Serialise the expression correctly

        foreach (var step in node.Steps)
        {
            WriteScenarioStep(sb, scenario, step, indent + _indent);
        }

        sb.AppendLine($"{indent}}}");
        sb.AppendLine($"");
    }

    private static void WriteStateTransitionNode(StringBuilder sb, Scenario scenario, StateTransitionNode stateTransitionNode, string indent)
    {
        if (stateTransitionNode.StatefulObject == null)
        {
            throw new Exception($"Stateful object not set on transition: {stateTransitionNode.Name}");
        }

        var stateful = stateTransitionNode.StatefulObject.ResolveReference(scenario.System);

        var obj = stateful.Match(
            Some: s => s,
            None: () => throw new Exception($"Stateful object not found: {stateTransitionNode.StatefulObject}"));

        sb.AppendLine($"{_indent}Transition {{");

        sb.AppendLine($"{_indent}{indent}{obj.Name} : {stateTransitionNode.TransitionName}");

        sb.AppendLine($"{_indent}}}");
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

    private static void WriteRelation(StringBuilder sb, string indent, Relation relation)
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

        if (entityType.StateMachine != null)
        {
            sb.AppendLine($"{indent}{_indent}SM {AddQuotes(entityType.StateMachine.Name)}");
        }

        sb.AppendLine($"{indent}}}");
        sb.AppendLine($"");
    }

    private static void WriteStateMachine(StringBuilder sb, string indent, StateMachine stateMachine)
    {
        sb.AppendLine($"{indent}SM {AddQuotes(stateMachine.Name)} {{");

        foreach (var state in stateMachine.States)
        {
            WriteSMState(sb, indent + _indent, state);
        }

        foreach (var state in stateMachine.States)
        {
            foreach (var transition in state.Transitions)
            {
                WriteSMTransition(sb, indent + _indent, stateMachine.States, state, transition);
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
        if (string.IsNullOrEmpty(transition.Name))
        {
            sb.AppendLine($"{indent}{transition.SourceState} -> {transition.DestinationState}");
        }
        else
        {
            sb.AppendLine($"{indent}{transition.SourceState} -> {transition.DestinationState} : {transition.Name}");
        }
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
