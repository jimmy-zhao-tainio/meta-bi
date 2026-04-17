using MetaDataTypeConversion;
using MetaDataTypeConversion.Instance;

namespace MetaDataTypeConversion.Tests;

public sealed class InstanceTests
{
    [Fact]
    public void Default_ProvidesSanctionedInMemoryInstance()
    {
        var model = MetaDataTypeConversionInstance.Default;

        Assert.NotEmpty(model.ConversionImplementationList);
        Assert.NotEmpty(model.DataTypeMappingList);
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "meta:type:String", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "sqlserver:type:nvarchar", StringComparison.Ordinal) &&
                   string.Equals(row.ConversionImplementationId, "MetaDataTypeConversion:implementation:direct", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:numeric", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "meta:type:Decimal", StringComparison.Ordinal) &&
                   string.Equals(row.ConversionImplementationId, "MetaDataTypeConversion:implementation:direct", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:real", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "meta:type:Single", StringComparison.Ordinal) &&
                   string.Equals(row.ConversionImplementationId, "MetaDataTypeConversion:implementation:direct", StringComparison.Ordinal));
    }
}
