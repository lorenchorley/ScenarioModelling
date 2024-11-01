using LanguageExt;
using ScenarioModel.Objects.SystemObjects.Entities;
using ScenarioModel.Objects.SystemObjects.Relations;
using ScenarioModel.Objects.SystemObjects.States;

namespace ScenarioModel.References;

public record EntityReference : IReference<Entity>, IRelatableObjectReference, IStatefulObjectReference
{
    public string EntityName { get; set; } = "";

    public Option<Entity> ResolveReference(System system)
    {
        return system.Entities.Find(x => x.Name.IsEqv(EntityName));
    }

    Option<IRelatable> IReference<IRelatable>.ResolveReference(System system)
        => ResolveReference(system).Map(x => (IRelatable)x);

    Option<IStateful> IReference<IStateful>.ResolveReference(System system)
        => ResolveReference(system).Map(x => (IStateful)x);

    override public string ToString() => $"{EntityName}";
}
