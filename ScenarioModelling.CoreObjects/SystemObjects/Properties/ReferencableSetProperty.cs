using Newtonsoft.Json;
using OneOf;
using ScenarioModelling.CoreObjects.References.Interfaces;

namespace ScenarioModelling.CoreObjects.SystemObjects.Properties;

public abstract class ReferencableSetProperty<TValue, TReference> : IEnumerable<TValue>
    where TValue : class, IIdentifiable
    where TReference : class, IReference
{
    private HashSet<OneOf<TValue, TReference>> _set;
    protected readonly MetaState _system;

    protected ReferencableSetProperty(MetaState System, IEqualityComparer<OneOf<TValue, TReference>>? customEqualityComparer = null)
    {
        _system = System;
        _set = new(customEqualityComparer ?? new OneOfValOrRefEquivalanceComparer<TValue, TReference>());
    }

    //public bool Contains(INameful nameful)
    //{
    //    return _list.Any(s => s.Match(
    //        v => v.Name.IsEqv(nameful.Name), 
    //        r => r.Name.IsEqv(nameful.Name))
    //    );
    //}

    [JsonIgnore]
    public bool HasValues => _set.Count > 0;

    public void TryAddValue(TValue transition)
    {
        _set.Add(OneOf<TValue, TReference>.FromT0(transition));
    }

    public void TryAddReference(TReference reference)
    {
        _set.Add(OneOf<TValue, TReference>.FromT1(reference));
    }

    public void TryAddReferenceRange(IEnumerable<TReference> references)
    {
        foreach (var item in references)
        {
            TryAddReference(item);
        }
    }

    [JsonIgnore]
    public List<TReference> AllReferencesOnly
    {
        get => _set.Select(s => s.Match(entityType => null, reference => reference))
                   .Where(s => s != null)
                   .Cast<TReference>()
                   .ToList();
    }

    [JsonIgnore]
    public IEnumerable<TValue> AllResolvedValues
        => _set.SelectMany(s => s.Match(
               value => [value],
               reference => reference.Resolve<TValue>()
           )).Distinct();

    public int Count => _set.Count;

    public IEnumerator<TValue> GetEnumerator()
        => AllResolvedValues.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => AllResolvedValues.GetEnumerator();

}

public class OneOfValOrRefEquivalanceComparer<TValue, TReference> : IEqualityComparer<OneOf<TValue, TReference>>
    where TValue : class, IIdentifiable
    where TReference : class, IReference
{
    public bool Equals(OneOf<TValue, TReference> x, OneOf<TValue, TReference> y)
        => x.Match(
            vx => y.Match(vy => AreEqual(vx, vy), ry => false),
            rx => y.Match(vy => false, ry => AreEqual(rx, ry))
        );

    protected virtual bool AreEqual(TValue vx, TValue vy)
        => vx.IsEqv(vy);

    protected virtual bool AreEqual(TReference rx, TReference ry)
        => rx.IsEqv(ry);

    public int GetHashCode(/*[DisallowNull] */OneOf<TValue, TReference> obj)
        => obj.GetHashCode();
}
