using MetaDataType.Instance;

namespace MetaDataType.Tests;

public sealed class InstanceTests
{
    [Fact]
    public void Default_ProvidesSanctionedInMemoryInstance()
    {
        var model = MetaDataTypeInstance.Default;

        Assert.NotEmpty(model.DataTypeSystemList);
        Assert.NotEmpty(model.DataTypeList);
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "sqlserver:type:nvarchar", StringComparison.Ordinal) &&
                   string.Equals(row.DataTypeSystemId, "sqlserver:type-system", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "meta:type:String", StringComparison.Ordinal) &&
                   string.Equals(row.DataTypeSystemId, "meta:type-system", StringComparison.Ordinal));
    }
}
