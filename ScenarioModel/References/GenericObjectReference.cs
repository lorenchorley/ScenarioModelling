using LanguageExt;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.SystemObjects.Entities;

namespace ScenarioModel.References;

public class GenericObjectReference : IReference<object>
{
    public ValueComposite Identifier { get; set; } = null!;

    public Option<object> ResolveReference(System system)
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
    private Option<object> FirstLevel(System system)
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
    private Option<object> EntityAccessor(System system, Entity entity)
    {
        var accessor = Identifier.ValueList[1];

        if (accessor.IsEqv("State"))
        {
            return entity.State.ResolvedValue;
        }

        var aspect = entity.Aspects.Find(f => f.Name.IsEqv(accessor));
        if (aspect != null)
        {
            if (Identifier.ValueList.Count == 2)
            {
                return aspect;
            }
            else
            {
                return AspectAccessor(system, aspect);
            }
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="system"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Option<object> AspectAccessor(System system, Aspect aspect)
    {
        var accessor = Identifier.ValueList[2];

        if (accessor.IsEqv("State"))
        {
            return aspect.State.ResolvedValue;
        }

        return null;
    }
}
