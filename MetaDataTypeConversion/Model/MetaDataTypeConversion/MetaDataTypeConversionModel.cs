#nullable enable

using System.Collections.Generic;

namespace MetaDataTypeConversion
{
    public sealed partial class MetaDataTypeConversionModel
    {
        public static MetaDataTypeConversionModel CreateEmpty() => new();

        public List<ConversionImplementation> ConversionImplementationList { get; set; } = new();
        public List<DataTypeMapping> DataTypeMappingList { get; set; } = new();
    }
}
