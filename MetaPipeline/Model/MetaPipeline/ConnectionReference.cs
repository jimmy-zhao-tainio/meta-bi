#nullable enable

namespace MetaPipeline
{
    public sealed class ConnectionReference
    {
        public string Id { get; set; } = string.Empty;

        public string EnvironmentVariableName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Pipeline Pipeline { get; set; } = null!;

    }
}
