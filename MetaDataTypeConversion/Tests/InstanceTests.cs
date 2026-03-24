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
    }
}
