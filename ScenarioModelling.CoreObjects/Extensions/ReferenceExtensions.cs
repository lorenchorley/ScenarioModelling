﻿using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;
using ScenarioModelling.CoreObjects.References;

public static class ReferenceExtensions
{
    public static bool IsEqv<T, S>(this T a, S b)
        where T : IIdentifiable
        where S : IIdentifiable
        => a.Type == b.Type && a.Name.IsEqv(b.Name);

    public static bool IsEqv<TVal, TRef>(this OptionalReferencableProperty<TVal, TRef> a, TVal b)
        where TVal : class, IMetaStateObject<TRef>
        where TRef : class, IReference<TVal>
        => a.Match(
            value: v => v.IsEqv(b),
            reference: r => r.IsEqv(b),
            isNull: () => false
        );

    public static bool IsEqv<TVal, TRef>(this TVal a, OptionalReferencableProperty<TVal, TRef> b)
        where TVal : class, IMetaStateObject<TRef>
        where TRef : class, IReference<TVal>
        => b.Match(
            value: v => v.IsEqv(a),
            reference: r => r.IsEqv(a),
            isNull: () => false
        );
}
