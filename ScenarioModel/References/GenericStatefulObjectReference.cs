using LanguageExt;
using ScenarioModel.Objects.SystemObjects.States;

namespace ScenarioModel.References;

public class GenericStatefulObjectReference(string ObjectName) : IStatefulObjectReference
{
    public Option<IStateful> ResolveReference(System system)
    {
        var entity = system.Entities.Where(e => e.Name.IsEqv(ObjectName)).FirstOrDefault();

        if (entity is null)
            return Option<IStateful>.None;

        return entity;
    }
}
