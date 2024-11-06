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
        typeof(StateTransitionNode),
        typeof(WhileNode)
    ];

    public static void DoForEachNodeType(
        Action chooseNode,
        Action dialogNode,
        Action ifNode,
        Action jumpNode,
        Action stateTransitionNode,
        Action whileNode)
    {
        chooseNode();
        dialogNode();
        ifNode();
        jumpNode();
        stateTransitionNode();
        whileNode();
    }

    public static void DoForEachNodeProperty<TNode>(TNode node, Action<string, object?> callback)
    {
        var type = typeof(TNode);
        var properties =
            type.GetProperties()
                .Select(p => (Property: p, Attribute: (NodeLikePropertyAttribute?)p.GetCustomAttributes(typeof(NodeLikePropertyAttribute), false).FirstOrDefault()))
                .Where(a => a.Attribute != null)
                .Select(a => (a.Property, Attribute: a.Attribute!))
                .Where(a => a.Attribute.Serialise);

        foreach (var property in properties)
        {
            var propertyName = property.Attribute.SerialisedName ?? property.Property.Name;
            var propertyValue = property.Property.GetValue(node);

            if (property.Attribute.DoNotSerialiseIfNullOrEmpty)
            {
                if (propertyValue is null)
                    continue;

                if (propertyValue is string s && string.IsNullOrEmpty(s))
                    continue;
            }

            callback(propertyName, propertyValue);
        }
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

    public ExhaustivenessException(string message, IEnumerable<Type> types) : base(TransformMessage(message, types))
    {
        Types = types.ToArray();
    }

    public static string TransformMessage(string message, IEnumerable<Type> types)
    {
        return $"{message} : {string.Join(", ", types.Select(t => t.Name))}";
    }
}