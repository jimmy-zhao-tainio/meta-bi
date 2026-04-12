namespace MetaTransform.Binding;

public sealed class TransformBindingValidationException : InvalidOperationException
{
    public TransformBindingValidationException(
        string code,
        string message)
        : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
