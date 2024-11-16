namespace ScenarioModel.Exhaustiveness.Exceptions;

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