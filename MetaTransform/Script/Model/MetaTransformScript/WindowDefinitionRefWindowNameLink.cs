#nullable enable

namespace MetaTransformScript
{
    public sealed class WindowDefinitionRefWindowNameLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public WindowDefinition WindowDefinition { get; set; } = null!;

    }
}
