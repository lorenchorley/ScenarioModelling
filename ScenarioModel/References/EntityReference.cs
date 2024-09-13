using LanguageExt;
using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.Relations;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.References;

public class EntityReference : IReference<Entity>, IRelatableObjectReference, IStatefulObjectReference
{
    public string EntityName { get; set; } = "";

    public Option<Entity> ResolveReference(System system)
    {
        throw new NotImplementedException();
    }

    Option<IRelatable> IReference<IRelatable>.ResolveReference(System system)
        => ResolveReference(system).Map(x => (IRelatable)x);

    Option<IStateful> IReference<IStateful>.ResolveReference(System system)
        => ResolveReference(system).Map(x => (IStateful)x);
}
