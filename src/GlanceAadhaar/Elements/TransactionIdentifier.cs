namespace GlanceAadhaar.Elements;

public class TransactionIdentifier
{
    public static readonly Func<string> DefaultGenerator = () => Guid.NewGuid().ToString("N");

    private static Func<string> _generator = DefaultGenerator;
    
    public static Func<string> Generator
    {
        get => _generator;
        set
        {
            ThrowIfNull(value, nameof(Generator));
            _generator = value;
        }
    }
    
    public string Value { get; set; } = Generator();
    
    public static implicit operator string(TransactionIdentifier transactionIdentifier) => transactionIdentifier.Value;
    
    public static implicit operator TransactionIdentifier(string transactionIdentifier) => new() { Value = transactionIdentifier };
    
    public override string ToString() => Value;
}