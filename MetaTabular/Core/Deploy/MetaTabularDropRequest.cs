namespace MetaTabular.Core.Deploy;

public sealed class MetaTabularDropRequest
{
    public string Server { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
}
