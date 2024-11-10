using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.References.Interfaces;

public static class ReferenceExtensions
{
    public static bool IsEqv<T, S>(this T a, S b)
        where T : IIdentifiable
        where S : IIdentifiable
        => a.Type == b.Type && a.Name.IsEqv(b.Name);

    public static bool IsEqv<T, TRef>(this OptionalReferencableProperty<T, TRef> a, T b)
        where T : class, IIdentifiable
        where TRef : class, IReference<T>
        => a.Match(
            value: v => v.IsEqv(b),
            reference: r => r.IsEqv(b),
            isNull: () => false
        );

    public static bool IsEqv<T, TRef>(this T a, OptionalReferencableProperty<T, TRef> b)
        where T : class, IIdentifiable
        where TRef : class, IReference<T>
        => b.Match(
            value: v => v.IsEqv(a),
            reference: r => r.IsEqv(a),
            isNull: () => false
        );
}
