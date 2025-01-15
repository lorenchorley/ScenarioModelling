using ScenarioModelling.References.Interfaces;

public static class SystemObjectExtensions
{
    public static IEnumerable<TValue> Resolve<TValue>(this IReference reference)
    {
        if (reference is IReference<TValue> r1)
        {
            var result = r1.ResolveReference();

            return result.Match<TValue[]>(
                Some: v => [v],
                None: () => Array.Empty<TValue>()
            );
        }
        else if (reference is IReferences<TValue> r2)
        {
            return r2.ResolveReferences();
        }

        throw new Exception($"{typeof(TValue).Name} reference '{reference}' could not be resolved.");
    }

}
