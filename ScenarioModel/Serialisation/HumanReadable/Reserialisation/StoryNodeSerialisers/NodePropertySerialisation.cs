using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.StoryNodeSerialisers;

public static class NodePropertySerialisation
{
    public static void SerialiseAnnotatedProperties<TNode>(this TNode node, StringBuilder sb, string indent) where TNode : IStoryNode
    {
        MetaStoryNodeExhaustivity.DoForEachNodeProperty(node, (attr, prop, value) =>
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

