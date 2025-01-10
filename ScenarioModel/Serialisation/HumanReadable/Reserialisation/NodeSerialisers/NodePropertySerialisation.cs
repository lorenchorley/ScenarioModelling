using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers;

public static class NodePropertySerialisation
{
    public static void SerialiseAnnotatedProperties<TNode>(this TNode node, StringBuilder sb, string indent) where TNode : IScenarioNode
    {
        ScenarioNodeExhaustivity.DoForEachNodeProperty(node, (attr, prop, value) =>
        {
            if (value is bool boolValue && attr.OptionalBool != Exhaustiveness.Attributes.OptionalBoolSetting.NotOptional)
            {
                if ((attr.OptionalBool == Exhaustiveness.Attributes.OptionalBoolSetting.TrueAsDefault && boolValue == false) ||
                    (attr.OptionalBool == Exhaustiveness.Attributes.OptionalBoolSetting.FalseAsDefault && boolValue == true))
                {
                    sb.AppendLine($"{indent}{prop}");
                }
            }
            else
            {
                sb.AppendLine($"{indent}{prop} {value?.ToString()?.AddQuotes() ?? ""}");
            }
        });
    }
}

