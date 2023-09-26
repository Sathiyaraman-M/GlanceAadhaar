namespace GlanceAadhaar.Exceptions;

internal static class ExceptionHelpers
{
    public static void ThrowIfNull<T>(T argument, string argName) where T : class
    {
        if (argument == null)
            throw new ArgumentNullException(argName);
    }
    
    public static void ThrowIfNullOrEmptyString(string argument, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException(RequiredNonEmptyString, argumentName);
    }
}