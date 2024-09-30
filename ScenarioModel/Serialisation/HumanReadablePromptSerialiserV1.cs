using LanguageExt;
using ScenarioModel.SystemObjects.Entities;
using System.Text;

namespace ScenarioModel.Serialisation;

public class HumanReadablePromptSerialiserV1 : ISerialiser
{
    public string SerialiseScenario(Scenario scenario)
    {
        StringBuilder sb = new();

        sb.AppendLine($"Scenario: {scenario.Name} (System: {scenario.SystemName}) {{");

        foreach (var step in scenario.Steps)
        {
            foreach (var target in step.TargetNodeNames)
            {
                sb.AppendLine($"  {step.Name} -> {target}");
            }
        }

        sb.AppendLine($"}}");

        return sb.ToString();
    }

    const string _indent = "  ";
    public string SerialiseSystem(System system)
    {
        StringBuilder sb = new();
        string indent = "";

        sb.AppendLine($"{indent}System: {system.Name} {{");

        foreach (var entityType in system.EntityTypes)
        {
            WriteEntityType(sb, indent + _indent, entityType);
        }

        foreach (var entity in system.Entities)
        {
            WriteEntity(sb, indent + _indent, entity);
        }

        sb.AppendLine($"{indent}}}");

        return sb.ToString();
    }

    private static void WriteEntity(StringBuilder sb, string indent, Entity entity)
    {
        sb.AppendLine($"{_indent}Entity: {entity.Name} {{");

        foreach (var aspectType in entity.Aspects)
        {
            WriteAspectType(sb, indent + _indent, aspectType);
        }

        foreach (var relation in entity.Relations)
        {
            sb.AppendLine($"{_indent}Relation: {relation.Name} {{");

            sb.AppendLine($"{_indent}{relation.LeftEntity} -> {relation.RightEntity}");
            sb.AppendLine($"{_indent}}}");
        }

        sb.AppendLine($"{_indent}}}");
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

    public Option<Scenario> DeserialiseScenario(string text, Context context)
    {
        return Option<Scenario>.None;
    }

    public Option<System> DeserialiseSystem(string text, Context context)
    {
        return Option<System>.None;
    }
}
