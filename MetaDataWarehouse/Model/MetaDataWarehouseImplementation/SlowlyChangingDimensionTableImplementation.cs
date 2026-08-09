#nullable enable

namespace MetaDataWarehouseImplementation
{
    public sealed class SlowlyChangingDimensionTableImplementation
    {
        public string Id { get; set; } = string.Empty;

        public string CurrentFlagColumnName { get; set; } = string.Empty;

        public string CurrentFlagDataTypeId { get; set; } = string.Empty;

        public string EffectiveFromColumnName { get; set; } = string.Empty;

        public string EffectiveFromDataTypeId { get; set; } = string.Empty;

        public string EffectiveToColumnName { get; set; } = string.Empty;

        public string EffectiveToDataTypeId { get; set; } = string.Empty;

        public string? HashDiffColumnName { get; set; }

        public string? HashDiffDataTypeId { get; set; }

    }
}
