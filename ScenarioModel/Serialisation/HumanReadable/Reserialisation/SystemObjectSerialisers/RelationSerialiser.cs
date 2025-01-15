using LanguageExt;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

using Relation = ScenarioModelling.Objects.SystemObjects.Relation;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers;

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
            sb.AppendLine($@"{currentIndent}{leftRelatable.Name.AddQuotes()} -> {rightRelatable.Name.AddQuotes()}");
        else
            sb.AppendLine($@"{currentIndent}{leftRelatable.Name.AddQuotes()} -> {rightRelatable.Name.AddQuotes()} : {obj.Name.AddQuotes()}");
    }
}

