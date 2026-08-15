namespace MetaDataType.Tests;

public sealed class InstanceTests
{
    [Fact]
    public void BuiltIn_ProvidesSanctionedWorkspaceInstance()
    {
        var model = MetaDataTypeInstance.BuiltIn;

        Assert.NotEmpty(model.DataTypeSystemList);
        Assert.NotEmpty(model.DataTypeList);
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "sqlserver:type:nvarchar", StringComparison.Ordinal) &&
                   string.Equals(row.DataTypeSystem.Name, "SqlServer", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "meta:type:String", StringComparison.Ordinal) &&
                   string.Equals(row.DataTypeSystem.Name, "Meta", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "sqlserver:type:numeric", StringComparison.Ordinal) &&
                   string.Equals(row.DataTypeSystem.Name, "SqlServer", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "sqlserver:type:real", StringComparison.Ordinal) &&
                   string.Equals(row.DataTypeSystem.Name, "SqlServer", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "sqlserver:type:Flag", StringComparison.Ordinal) &&
                   string.Equals(row.Category, "Logical", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "sqlserver:type:Name", StringComparison.Ordinal) &&
                   string.Equals(row.Category, "Text", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "sqlserver:type:NameStyle", StringComparison.Ordinal) &&
                   string.Equals(row.Category, "Logical", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "sqlserver:type:AccountNumber", StringComparison.Ordinal) &&
                   string.Equals(row.Category, "Text", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "sqlserver:type:OrderNumber", StringComparison.Ordinal) &&
                   string.Equals(row.Category, "Text", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "sqlserver:type:Phone", StringComparison.Ordinal) &&
                   string.Equals(row.Category, "Text", StringComparison.Ordinal));
        Assert.Contains(
            model.DataTypeList,
            row => string.Equals(row.Id, "sqlserver:type:sysname", StringComparison.Ordinal) &&
                   string.Equals(row.Category, "Text", StringComparison.Ordinal));
    }
}
