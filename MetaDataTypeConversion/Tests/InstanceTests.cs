using MetaDataTypeConversion;

namespace MetaDataTypeConversion.Tests;

public sealed class InstanceTests
{
    [Fact]
    public void BuiltIn_ProvidesSanctionedWorkspaceInstance()
    {
        var model = MetaDataTypeConversionInstance.BuiltIn;

        Assert.NotEmpty(model.ConversionImplementationList);
        Assert.NotEmpty(model.DataTypeMappingList);
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "meta:type:String", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "sqlserver:type:nvarchar", StringComparison.Ordinal) &&
                   string.Equals(row.ConversionImplementation.Id, "MetaDataTypeConversion:implementation:direct", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:numeric", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "meta:type:Decimal", StringComparison.Ordinal) &&
                   string.Equals(row.ConversionImplementation.Id, "MetaDataTypeConversion:implementation:direct", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:real", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "meta:type:Single", StringComparison.Ordinal) &&
                   string.Equals(row.ConversionImplementation.Id, "MetaDataTypeConversion:implementation:direct", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:Flag", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "meta:type:Boolean", StringComparison.Ordinal) &&
                   string.Equals(row.ConversionImplementation.Id, "MetaDataTypeConversion:implementation:direct", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:Name", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "meta:type:String", StringComparison.Ordinal) &&
                   string.Equals(row.ConversionImplementation.Id, "MetaDataTypeConversion:implementation:direct", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:NameStyle", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "meta:type:Boolean", StringComparison.Ordinal) &&
                   string.Equals(row.ConversionImplementation.Id, "MetaDataTypeConversion:implementation:direct", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:Flag", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "sqlserver:type:bit", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:Name", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "sqlserver:type:nvarchar", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:NameStyle", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "sqlserver:type:bit", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:sysname", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "meta:type:String", StringComparison.Ordinal) &&
                   string.Equals(row.ConversionImplementation.Id, "MetaDataTypeConversion:implementation:direct", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeMappingList,
            row => string.Equals(row.SourceDataTypeId, "sqlserver:type:sysname", StringComparison.Ordinal) &&
                   string.Equals(row.TargetDataTypeId, "sqlserver:type:nvarchar", StringComparison.Ordinal));
    }
}
