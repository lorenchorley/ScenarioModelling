using ScenarioModel.Objects.ScenarioObjects;

namespace ScenarioModel.Exhaustiveness;

public static class NodeExhaustiveness
{
    public static Type[] AllNodeTypes =>
    [
        typeof(ChooseNode),
        typeof(DialogNode),
        typeof(IfNode),
        typeof(JumpNode),
        typeof(StateTransitionNode)
    ];

    public static void DoForEachNodeType(
        Action ChooseNode,
        Action DialogNode,
        Action IfNode,
        Action JumpNode,
        Action StateTransitionNode)
    {
        ChooseNode();
        DialogNode();
        IfNode();
        JumpNode();
        StateTransitionNode();
    }

    public static void AssertExhaustivelyImplemented<TBaseClass>(Type[]? replacementCompleteTypeList = null)
    {
        Type[] completeList = replacementCompleteTypeList ?? AllNodeTypes;
        Type[] implementations = GetAllImplmentationsOfType<TBaseClass>();
        var taggedTypes = GetAllTaggedTypesWithBaseClass<TBaseClass>();

        var allAnnotatedImplementations = taggedTypes.Select(t => t.GenericTargetType).ToArray();

        var unimplementedTypes = allAnnotatedImplementations.Except<Type>(completeList).ToArray();
        if (unimplementedTypes.Any())
        {
            throw new ExhaustivenessException("These types are not implemented", unimplementedTypes);
        }

        var untaggedTargetTypes = completeList.Except(allAnnotatedImplementations).ToArray();
        if (untaggedTargetTypes.Any())
        {
            throw new ExhaustivenessException("These target types are not tagged", untaggedTargetTypes);
        }

        var groupedByTargetType = taggedTypes.GroupBy(a => a.GenericTargetType);
        foreach (var group in groupedByTargetType)
        {
            if (group.Count() > 1)
            {
                throw new ExhaustivenessException($"Type {group.Key.Name} is tagged as implemented {group.Count()} times", group.Select(g => g.Type));
            }
        }
    }

    private static Type[] GetAllImplmentationsOfType<TBaseClass>()
    {
        var baseType = typeof(TBaseClass);
        return baseType.Assembly
                       .GetTypes()
                       .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
                       .ToArray();
    }

    private static (Type Type, Attribute NodeLikeAttribute, Type GenericBaseType, Type GenericTargetType)[] GetAllTaggedTypesWithBaseClass<TBaseClass>()
    {
        var baseType = typeof(TBaseClass);
        var taggedTypes = baseType.Assembly
                       .GetTypes()
                       .Select(t => (Type: t, NodeLikeAttribute: (Attribute?)t.GetCustomAttributes(typeof(NodeLikeAttribute<,>), false).FirstOrDefault()))
                       .Select(t => (Type: t.Type, NodeLikeAttribute: t.NodeLikeAttribute, GenericBaseType: t.NodeLikeAttribute?.GetType().GetGenericArguments()[0], GenericTargetType: t.NodeLikeAttribute?.GetType().GetGenericArguments()[1]))
                       .Where(t => t.GenericBaseType == baseType)
                       .ToArray();

        var erroneouslyTags1 = taggedTypes.Where(a => a.NodeLikeAttribute == null);
        foreach (var erroneouslyTag in erroneouslyTags1)
        {
            throw new ExhaustivenessException($"Type {erroneouslyTag.Type.Name} is missing the NodeLike attribute", erroneouslyTags1.Select(s => s.Type)); // TODO better
        }

        var erroneouslyTags2 = taggedTypes.Where(a => a.GenericBaseType == null);
        foreach (var erroneouslyTag in erroneouslyTags2)
        {
            throw new ExhaustivenessException($"Type {erroneouslyTag.Type.Name} is missing the NodeLike attribute", erroneouslyTags2.Select(s => s.Type)); // TODO better
        }

        var erroneouslyTags3 = taggedTypes.Where(a => a.GenericTargetType == null);
        foreach (var erroneouslyTag in erroneouslyTags3)
        {
            throw new ExhaustivenessException($"Type {erroneouslyTag.Type.Name} is missing the NodeLike attribute", erroneouslyTags3.Select(s => s.Type)); // TODO better
        }

        return taggedTypes.Select(t => (Type: t.Type, NodeLikeAttribute: t.NodeLikeAttribute!, GenericBaseType: t.GenericBaseType!, GenericTargetType: t.GenericTargetType!))
                          .ToArray();
    }
}

public class ExhaustivenessException : Exception
{
    public Type[] Types { get; }

    public ExhaustivenessException(string message, IEnumerable<Type> types) : base(message)
    {
        Types = types.ToArray();
    }

    public override string ToString()
    {
        return $"{Message} : {string.Join(", ", Types.Select(t => t.Name))}";
    }
}