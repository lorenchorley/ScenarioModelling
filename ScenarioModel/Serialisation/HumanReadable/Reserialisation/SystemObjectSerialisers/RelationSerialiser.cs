using LanguageExt;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

using Relation = ScenarioModel.Objects.SystemObjects.Relation;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.ObjectDeserialisers.Interfaces;

[ObjectLike<IObjectSerialiser, Relation>]
public class RelationSerialiser(string IndentSegment) : IObjectSerialiser<Relation>
{
    public void WriteObject(StringBuilder sb, System system, Relation obj, string currentIndent)
    {
        Option<IRelatable> left = obj.LeftEntity?.ResolveReference() ?? Option<IRelatable>.None;
        Option<IRelatable> right = obj.RightEntity?.ResolveReference() ?? Option<IRelatable>.None;

        IRelatable leftRelatable = left.Match(Some: r => r, None: () => throw new Exception($"The left hand object of relation {obj.Name} is unresolvable"));
        IRelatable rightRelatable = right.Match(Some: r => r, None: () => throw new Exception($"The right hand object of relation {obj.Name} is unresolvable"));

        if (string.IsNullOrEmpty(obj.Name))
            sb.AppendLine($@"{currentIndent}{ContextSerialiser.AddQuotes(leftRelatable.Name)} -> {ContextSerialiser.AddQuotes(rightRelatable.Name)}");
        else
            sb.AppendLine($@"{currentIndent}{ContextSerialiser.AddQuotes(leftRelatable.Name)} -> {ContextSerialiser.AddQuotes(rightRelatable.Name)} : {ContextSerialiser.AddQuotes(obj.Name)}");
    }
}

