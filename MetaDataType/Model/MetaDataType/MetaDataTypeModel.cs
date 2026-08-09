#nullable enable

using System.Collections.Generic;

namespace MetaDataType
{
    public sealed partial class MetaDataTypeModel
    {
        public static MetaDataTypeModel CreateEmpty() => new();

        public List<DataType> DataTypeList { get; set; } = new();

        public List<DataTypeSystem> DataTypeSystemList { get; set; } = new();
    }
}
