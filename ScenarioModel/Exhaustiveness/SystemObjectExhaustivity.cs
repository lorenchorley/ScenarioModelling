using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Exhaustiveness.Common;
using ScenarioModel.Objects.SystemObjects;

namespace ScenarioModel.Exhaustiveness;

public static class SystemObjectExhaustivity
{
    public static Type[] AllObjectTypes =>
    [
        typeof(Entity),
        typeof(EntityType),
        typeof(Aspect),
        //typeof(AspectType),
        typeof(Relation),
        typeof(State),
        typeof(StateMachine),
        typeof(Transition),
        typeof(Constraint),
    ];

    public static void AssertIsObjectType<T>()
    {
        if (!AllObjectTypes.Contains(typeof(T)))
            throw new ArgumentException($"Type {typeof(T).Name} is not a valid object type.");
    }

    public static void DoForEachObjectType(
        Action entity,
        Action entityType,
        Action aspect,
        //Action aspectType,
        Action relation,
        Action state,
        Action stateMachine,
        Action transition,
        Action constraint)
    {
        entity();
        entityType();
        aspect();
        //aspectType();
        relation();
        state();
        stateMachine();
        transition();
        constraint();
    }

    private static readonly PropertyExhaustivityFunctions<ObjectLikePropertyAttribute> _propertyExhaustivityFunctions = new();
    private static readonly TypeExhaustivityFunctions _typeExhaustivityFunctions = new(AllObjectTypes, typeof(ObjectLikeAttribute<,>), "object");

    public static void DoForEachNodeProperty<TNode>(TNode node, Action<string, object?> callback)
    {
        _propertyExhaustivityFunctions.DoForEachProperty<TNode>(node, callback);
    }

    public static void AssertInterfaceExhaustivelyImplemented<TInterfaceType>()
    {
        _typeExhaustivityFunctions.AssertTypeExhaustivelyImplemented<TInterfaceType>();
    }

}
