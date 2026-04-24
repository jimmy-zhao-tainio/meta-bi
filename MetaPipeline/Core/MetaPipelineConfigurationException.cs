namespace MetaPipeline;

public sealed class MetaPipelineConfigurationException : Exception
{
    public MetaPipelineConfigurationException(string message)
        : base(message)
    {
    }

    public MetaPipelineConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
