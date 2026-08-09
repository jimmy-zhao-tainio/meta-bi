#nullable enable

namespace MetaTransformBinding
{
    public sealed class InsertValuesWrite
    {
        public string Id { get; set; } = string.Empty;

        public string MetaTransformScriptRowValueId { get; set; } = string.Empty;

        public Write Write { get; set; } = null!;

    }
}
