using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.SystemObjects;

namespace ScenarioModel.Expressions.Common;

public class EqualityFunctions
{
    public static T EqualityTypeCases<T>(object left, object right, System system, Func<State, string, T> compareStateAndString, Func<bool, bool, T> compareBools, Func<string, string, T> compareStrings, Func<Entity, Entity, T> compareEntities)
    {
        // Null checks
        if (left == null)
            throw new Exception("Left side of the expression is null");

        if (right == null)
            throw new Exception("Right side of the expression is null");

        // Unequal type comparisons
        if (left is State state1 && right is string str1)
            return compareStateAndString(state1, str1);
        else if (right is State state2 && left is string str2)
            return compareStateAndString(state2, str2);

        if (left.GetType() != right.GetType())
            throw new Exception($"Cannot compare different these differing types ({left.GetType().Name}, {right.GetType().Name})");

        // Equal type comparisons
        if (left.GetType() == typeof(bool))
            return compareBools((bool)left, (bool)right);

        if (left.GetType() == typeof(string))
            return compareStrings((string)left, (string)right);

        if (left is Entity leftEntity &&
            right is Entity rightEntity)
            return compareEntities(leftEntity, rightEntity);

        if (left is CompositeValue leftValue &&
            right is CompositeValue rightValue)
        {
            // Resolve values
            var leftResolvedObject = system.ResolveValue(leftValue);
            var rightResolvedObject = system.ResolveValue(rightValue);

            return EqualityTypeCases(
                leftResolvedObject,
                rightResolvedObject,
                system,
                compareStateAndString,
                compareBools,
                compareStrings,
                compareEntities
            );
        }

        throw new Exception($"Unsupported type for expression : {left.GetType().Name}");
    }

    public static void CheckEqualityTypeCases(Type? left, Type? right, System system)
    {
        // Null checks
        if (left == null)
            throw new Exception("Left side of the expression is null");

        if (right == null)
            throw new Exception("Right side of the expression is null");

        // Unequal type comparisons
        if (left == typeof(State) && right == typeof(string))
            return;
        else if (right == typeof(State) && left == typeof(string))
            return;

        if (left != right)
            throw new Exception($"Cannot compare different these differing types ({left.Name}, {right.Name})");

        // Equal type comparisons
        if (left == typeof(bool))
            return;

        if (left == typeof(string))
            return;

        if (left == typeof(Entity) &&
            right == typeof(Entity))
            return;

        //if (left == typeof(CompositeValue) &&
        //    right == typeof(CompositeValue))
        //{
        //    // Resolve values
        //    var leftResolvedObject = system.ResolveValue(leftValue);
        //    var rightResolvedObject = system.ResolveValue(rightValue);

        //    return EqualityTypeCases(
        //        leftResolvedObject,
        //        rightResolvedObject,
        //        system,
        //        compareStateAndString,
        //        compareBools,
        //        compareStrings,
        //        compareEntities
        //    );
        //}

        throw new Exception($"Unsupported type for expression : {left.Name}");
    }

}
