public static class ArgumentExceptionStandard
{
    public static void ThrowIfNullOrEmpty(string message)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("String was null or empty");
    }
}
