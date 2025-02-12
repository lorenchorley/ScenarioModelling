public static class ArgumentNullExceptionStandard
{
    public static void ThrowIfNull(object? obj)
    {
        if (ReferenceEquals(obj, null))
            throw new ArgumentNullException("Value was null");
    }
}
