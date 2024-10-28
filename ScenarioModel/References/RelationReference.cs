using LanguageExt;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.System.States;

namespace ScenarioModel.References;

public record RelationReference : IReference<Objects.System.Relations.Relation>, IStatefulObjectReference
{
    public string? RelationName { get; set; }
    public ValueComposite? FirstRelatableName { get; set; }
    public ValueComposite? SecondRelatableName { get; set; }

    public Option<Objects.System.Relations.Relation> ResolveReference(System system)
    {
        if (FirstRelatableName == null || SecondRelatableName == null)
        {
            throw new NotImplementedException();
        }

        return system.AllRelations
                     .Find(x => x.Name.IsEqv(RelationName));
    }

    Option<IStateful> IReference<IStateful>.ResolveReference(System system)
        => ResolveReference(system).Map(x => (IStateful)x);

    override public string ToString() => $"{RelationName}";
}
