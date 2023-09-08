namespace GlanceAadhaar.Exceptions;

internal static class ExceptionHelpers
{
    public static T ThrowIfNull<T>(T argument, string argName) where T : class
    {
        if (argument == null)
            throw new ArgumentNullException(argName);

        return argument;
    }
    
    public static string ThrowIfNullOrEmptyString(string argument, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException(RequiredNonEmptyString, argumentName);

        return argument;
    }
}