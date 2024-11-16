using ScenarioModel.Exhaustiveness.Exceptions;

namespace ScenarioModel.Exhaustiveness.Common;

public class TypeExhaustivityFunctions
{
    private readonly Type[] _allTypes;
    private readonly Type _likeAttributeType;

    public TypeExhaustivityFunctions(Type[] allTypes, Type likeAttributeType)
    {
        _allTypes = allTypes;
        _likeAttributeType = likeAttributeType;
    }

    public void AssertTypeExhaustivelyImplemented<TBaseType>()
    {
        Type[] implementations = GetAllImplmentationsOfType<TBaseType>();
        var taggedTypes = GetAllTaggedTypesWithBaseClass<TBaseType>();

        var allAnnotatedImplementations = taggedTypes.Select(t => t.GenericTargetType).ToArray();

        var unimplementedTypes = allAnnotatedImplementations.Except(_allTypes).ToArray();
        if (unimplementedTypes.Any())
        {
            throw new ExhaustivenessException("These types are not implemented", unimplementedTypes);
        }

        var untaggedTargetTypes = _allTypes.Except(allAnnotatedImplementations).ToArray();
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

    private Type[] GetAllImplmentationsOfType<TBaseType>()
    {
        var baseType = typeof(TBaseType);
        return baseType.Assembly
                       .GetTypes()
                       .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
                       .ToArray();
    }

    private (Type Type, Attribute LikeAttribute, Type GenericBaseType, Type GenericTargetType)[] GetAllTaggedTypesWithBaseClass<TBaseClass>()
    {
        var baseType = typeof(TBaseClass);
        var taggedTypes = baseType.Assembly
                       .GetTypes()
                       .Select(t => (Type: t, LikeAttribute: (Attribute?)t.GetCustomAttributes(_likeAttributeType, false).FirstOrDefault()))
                       .Select(t => (t.Type, t.LikeAttribute, GenericBaseType: t.LikeAttribute?.GetType().GetGenericArguments()[0], GenericTargetType: t.LikeAttribute?.GetType().GetGenericArguments()[1]))
                       .Where(t => t.GenericBaseType == baseType)
                       .ToArray();

        var erroneouslyTags1 = taggedTypes.Where(a => a.LikeAttribute == null);
        foreach (var erroneouslyTag in erroneouslyTags1)
        {
            throw new ExhaustivenessException($"Type {erroneouslyTag.Type.Name} is missing the {_likeAttributeType.Name} attribute", erroneouslyTags1.Select(s => s.Type)); // TODO better
        }

        var erroneouslyTags2 = taggedTypes.Where(a => a.GenericBaseType == null);
        foreach (var erroneouslyTag in erroneouslyTags2)
        {
            throw new ExhaustivenessException($"Type {erroneouslyTag.Type.Name} is missing the {_likeAttributeType.Name} attribute", erroneouslyTags2.Select(s => s.Type)); // TODO better
        }

        var erroneouslyTags3 = taggedTypes.Where(a => a.GenericTargetType == null);
        foreach (var erroneouslyTag in erroneouslyTags3)
        {
            throw new ExhaustivenessException($"Type {erroneouslyTag.Type.Name} is missing the {_likeAttributeType.Name} attribute", erroneouslyTags3.Select(s => s.Type)); // TODO better
        }

        return taggedTypes.Select(t => (t.Type, LikeAttribute: t.LikeAttribute!, GenericBaseType: t.GenericBaseType!, GenericTargetType: t.GenericTargetType!))
                          .ToArray();
    }
}
