#nullable enable

namespace MetaTransformScript
{
    public sealed class AtTimeZoneCallTimeZoneLink
    {
        public string Id { get; set; } = string.Empty;

        public AtTimeZoneCall AtTimeZoneCall { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
