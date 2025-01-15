using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Expressions.SemanticTree;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.References.GeneralisedReferences;

public class CompositeValueObjectReference(System System) : IReference<IIdentifiable>
{
    public CompositeValue Identifier { get; set; } = null!;

    public string Name
    {
        get => ResolveReference().Match(
            Some: x => x.Name,
            None: () => throw new Exception($"{Identifier.ValueList.DotSeparatedList()} is not a valid relatable object reference")
        );
        set => throw new Exception();
    }

    [JsonIgnore]
    public Type Type => ResolveReference().Match(
            Some: x => x.Type,
            None: () => throw new Exception($"{Identifier.ValueList.DotSeparatedList()} is not a valid relatable object reference")
        );

    public bool IsResolvable() => ResolveReference().IsSome;

    public Option<IIdentifiable> ResolveReference()
    {
        if (Identifier.ValueList.Count == 0)
        {
            throw new ArgumentException();
        }

        return FirstLevel();
    }

    /// <summary>
    /// The first position has to be an entity name
    /// </summary>
    /// <param name="system"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Option<IIdentifiable> FirstLevel()
    {
        var value = Identifier.ValueList[0];

        var entity = System.Entities.FirstOrDefault(e => e.Name.IsEqv(value));

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
            return EntityAccessor(entity);
        }
    }

    /// <summary>
    /// The second level when an entity is the first, can only be an aspect
    /// </summary>
    /// <param name="system"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Option<IIdentifiable> EntityAccessor(Entity entity)
    {
        var accessor = Identifier.ValueList[1];

        if (accessor.IsEqv("State"))
        {
            return entity.State.ResolvedValue;
        }

        var aspect = entity.Aspects.FirstOrDefault(f => f.Name.IsEqv(accessor));
        if (aspect != null)
        {
            if (Identifier.ValueList.Count == 2)
            {
                return aspect;
            }
            else
            {
                return AspectAccessor(aspect);
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
    private Option<IIdentifiable> AspectAccessor(Aspect aspect)
    {
        var accessor = Identifier.ValueList[2];

        if (accessor.IsEqv("State"))
        {
            return aspect.State.ResolvedValue;
        }

        return null;
    }

}
