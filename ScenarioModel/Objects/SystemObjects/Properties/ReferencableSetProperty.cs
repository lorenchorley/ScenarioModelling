using OneOf;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References.Interfaces;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ScenarioModel.Objects.SystemObjects.Properties;

public abstract class ReferencableSetProperty<TValue, TReference>(System System) : IEnumerable<TValue>
    where TValue : class, IIdentifiable
    where TReference : class, IReference<TValue>
{
    private HashSet<OneOf<TValue, TReference>> _set = new(new EquivalanceComparer());

    //public bool Contains(INameful nameful)
    //{
    //    return _list.Any(s => s.Match(
    //        v => v.Name.IsEqv(nameful.Name), 
    //        r => r.Name.IsEqv(nameful.Name))
    //    );
    //}

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

    public List<TReference> AllReferences
    {
        get => _set.Select(s => s.Match<TReference?>(entityType => null, reference => reference))
                   .Where(s => s != null)
                   .Cast<TReference>()
                   .ToList();
    }

    public IEnumerable<TValue> AllResolvedValues
    {
        get =>
            _set.Select(s => s.Match(
                value => value,
                reference => reference.ResolveReference().Match(
                    value => value,
                    () => throw new Exception($"{typeof(TValue).Name} reference '{reference}' could not be resolved.")
                )
            )).ToList();
    }

    public int Count => _set.Count;

    public IEnumerator<TValue> GetEnumerator()
        => AllResolvedValues.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => AllResolvedValues.GetEnumerator();

    private class EquivalanceComparer : IEqualityComparer<OneOf<TValue, TReference>>
    {
        public bool Equals(OneOf<TValue, TReference> x, OneOf<TValue, TReference> y)
            => x.Match(
                X => y.Match(Y => X.IsEqv(Y), R => false),
                r => y.Match(Y => false, R => r.IsEqv(R))
            );

        public int GetHashCode([DisallowNull] OneOf<TValue, TReference> obj)
            => obj.GetHashCode();
    }

}
