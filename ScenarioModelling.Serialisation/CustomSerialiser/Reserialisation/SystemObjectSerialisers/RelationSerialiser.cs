using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers.Interfaces;
using System.Text;

using Relation = ScenarioModelling.CoreObjects.SystemObjects.Relation;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers;

[SystemObjectLike<IObjectSerialiser, Relation>]
public class RelationSerialiser : IObjectSerialiser<Relation>
{
    public void WriteObject(StringBuilder sb, MetaState metaState, Relation obj, string currentIndent)
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

