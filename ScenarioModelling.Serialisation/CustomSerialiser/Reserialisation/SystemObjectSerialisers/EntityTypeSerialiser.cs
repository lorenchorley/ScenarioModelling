﻿using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers;

[MetaStateObjectLike<IObjectSerialiser, EntityType>]
public class EntityTypeSerialiser : IObjectSerialiser<EntityType>
{
    public void WriteObject(StringBuilder sb, MetaState metaState, EntityType obj, string currentIndent)
    {
        if (!obj.ShouldReserialise)
            return;

        string subIndent = currentIndent + CustomContextSerialiser.IndentSegment;

        sb.AppendLine($"{currentIndent}EntityType {obj.Name.AddQuotes()} {{");

        if (obj.StateMachine.ResolvedValue != null)
            sb.AppendLine($"{subIndent}StateMachine {obj.StateMachine.Name?.AddQuotes() ?? ""}");

        sb.AppendLine($"{currentIndent}}}");
        sb.AppendLine($"");
    }
}

