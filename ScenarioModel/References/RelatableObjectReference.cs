using LanguageExt;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.Relations;

namespace ScenarioModel.References;

public class RelatableObjectReference : IRelatableObjectReference, IReference
{
    public ValueComposite Identifier { get; set; }

    public Option<IRelatable> ResolveReference(System system)
    {
        if (Identifier.ValueList.Count == 0)
        {
            throw new ArgumentException();
        }

        return FirstLevel(system);
    }

    /// <summary>
    /// The first position has to be an entity name
    /// </summary>
    /// <param name="system"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Option<IRelatable> FirstLevel(System system)
    {
        var value = Identifier.ValueList[0];

        var entity = system.Entities.Find(e => e.Name.IsEqv(value));

        if (entity == null)
        {
            return null;
        }

        if (Identifier.ValueList.Count == 1)
        {
            return entity;
        }
        else
        {
            return EntityAccessor(system, entity);
        }
    }

    /// <summary>
    /// The second level when an entity is the first, can only be an aspect
    /// </summary>
    /// <param name="system"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Option<IRelatable> EntityAccessor(System system, Entity entity)
    {
        var accessor = Identifier.ValueList[1];

        var aspect = entity.Aspects.Find(f => f.Name.IsEqv(accessor));
        if (aspect != null)
        {
            return aspect;
        }

        return null;
    }
}
