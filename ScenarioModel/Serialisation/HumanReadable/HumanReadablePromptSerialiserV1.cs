using LanguageExt.Common;
using ScenarioModel.Serialisation.HumanReadable.Interpreter;
using ScenarioModel.SystemObjects.Entities;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable;

public class HumanReadablePromptSerialiserV1 : ISerialiser
{
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

        return context;
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
        sb.AppendLine($"Scenario: {scenario.Name} {{");

        foreach (var step in scenario.Steps)
        {
            if (step is not ITransitionNode transitionNode) continue;

            foreach (var target in transitionNode.TargetNodeNames)
            {
                sb.AppendLine($"  {step.Name} -> {target}");
            }
        }

        sb.AppendLine($"}}");
    }

    const string _indent = "  ";
    public void SerialiseSystem(StringBuilder sb, System system)
    {
        foreach (var entityType in system.EntityTypes)
        {
            WriteEntityType(sb, "", entityType);
        }

        foreach (var entity in system.Entities)
        {
            WriteEntity(sb, "", entity);
        }
    }

    private static void WriteEntity(StringBuilder sb, string indent, Entity entity)
    {
        sb.AppendLine($"{indent}Entity: {entity.Name} {{");

        foreach (var aspectType in entity.Aspects)
        {
            WriteAspectType(sb, indent + _indent, aspectType);
        }

        foreach (var relation in entity.Relations)
        {
            WriteRelation(sb, indent + _indent, relation);
        }

        sb.AppendLine($"{indent}}}");
    }

    private static void WriteRelation(StringBuilder sb, string indent, SystemObjects.Relations.Relation relation)
    {
        sb.AppendLine($"{indent}Relation: {relation.Name} {{");

        sb.AppendLine($"{indent}{relation.LeftEntity} -> {relation.RightEntity}");
        sb.AppendLine($"{indent}}}");
    }

    private static void WriteAspectType(StringBuilder sb, string indent, Aspect aspectType)
    {
        sb.AppendLine($"{indent}AspectType: {aspectType.Name} {{");

        sb.AppendLine($"{indent}}}");
    }

    private static void WriteEntityType(StringBuilder sb, string indent, EntityType entityType)
    {
        sb.AppendLine($"{indent}EntityType: {entityType.Name} {{");

        foreach (var aspectType in entityType.AspectTypes)
        {
            WriteAspect(sb, indent + _indent, aspectType);
        }

        sb.AppendLine($"{indent}}}");
    }

    private static void WriteAspect(StringBuilder sb, string indent, AspectType aspectType)
    {
        sb.AppendLine($"{indent}AspectType: {aspectType.Name} {{");

        sb.AppendLine($"{indent}}}");
    }

}
