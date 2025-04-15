using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Exhaustiveness;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers;

public static class NodePropertySerialisation
{
    public static void SerialiseAnnotatedProperties<TNode>(this TNode node, StringBuilder sb, string indent) where TNode : IStoryNode
    {
        MetaStoryNodeExhaustivity.DoForEachNodeProperty(node, (attr, prop, value) =>
        {
            if (value is bool boolValue && attr.OptionalBool != OptionalBoolSetting.NotOptional)
            {
                if ((attr.OptionalBool == OptionalBoolSetting.TrueAsDefault && boolValue == false) ||
                    (attr.OptionalBool == OptionalBoolSetting.FalseAsDefault && boolValue == true))
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

