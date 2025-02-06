using ScenarioModelling.Exhaustiveness.Attributes;

namespace ScenarioModelling.Exhaustiveness.Common;

public class PropertyExhaustivityFunctions<TPropertyAttribute>
        where TPropertyAttribute : IPropertyAttribute
{
    public void DoForEachProperty<TType>(TType obj, Action<TPropertyAttribute, string, object?> callback)
    {
        if (!ExhaustivityFunctions.Active)
            return;

        var type = typeof(TType);
        var properties =
            type.GetProperties()
                .Select(p => (Property: p, Attribute: (TPropertyAttribute?)p.GetCustomAttributes(typeof(TPropertyAttribute), false).FirstOrDefault()))
                .Where(a => a.Attribute != null)
                .Select(a => (a.Property, Attribute: a.Attribute!))
                .Where(a => a.Attribute.Serialise);

        foreach (var property in properties)
        {
            var propertyName = property.Attribute.SerialisedName ?? property.Property.Name;
            var propertyValue = property.Property.GetValue(obj);

            if (property.Attribute.DoNotSerialiseIfNullOrEmpty)
            {
                if (propertyValue is null)
                    continue;

                if (propertyValue is string s && string.IsNullOrEmpty(s))
                    continue;
            }

            callback(property.Attribute, propertyName, propertyValue);
        }
    }

}
