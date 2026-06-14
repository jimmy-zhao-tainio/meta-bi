using MetaDataTypeConversion.Core;

namespace MetaDataTypeConversion.Tests;

public sealed class ServiceTests
{
    [Fact]
    public void DefaultWorkspace_Check_AllowsOneSourcePerTargetSystem()
    {
        var result = new MetaDataTypeConversionService()
            .Check(MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace());

        Assert.False(result.HasErrors);
    }

    [Fact]
    public void Resolve_WithTargetDataTypeSystem_SelectsRequestedSystem()
    {
        var resolution = new MetaDataTypeConversionService().Resolve(
            MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace(),
            "meta:type:String",
            "SqlServer");

        Assert.Equal("meta:type:String", resolution.SourceDataTypeId);
        Assert.Equal("sqlserver:type:nvarchar", resolution.TargetDataTypeId);
        Assert.Equal("sqlserver", resolution.TargetDataTypeSystemName);
    }

    [Fact]
    public void ResolveCompatibility_AllowsSanctionedPath()
    {
        var resolution = new MetaDataTypeConversionService().ResolveCompatibility(
            MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace(),
            "sqlserver:type:int",
            "sqlserver:type:nvarchar");

        Assert.Equal("sqlserver:type:int", resolution.SourceDataTypeId);
        Assert.Equal("sqlserver:type:nvarchar", resolution.TargetDataTypeId);
        Assert.False(resolution.IsExact);
        Assert.NotEmpty(resolution.Path);
    }

    [Theory]
    [InlineData("sqlserver:type:Flag", "sqlserver:type:bit")]
    [InlineData("sqlserver:type:NameStyle", "sqlserver:type:bit")]
    [InlineData("sqlserver:type:AccountNumber", "sqlserver:type:nvarchar")]
    [InlineData("sqlserver:type:Name", "sqlserver:type:nvarchar")]
    [InlineData("sqlserver:type:OrderNumber", "sqlserver:type:nvarchar")]
    [InlineData("sqlserver:type:Phone", "sqlserver:type:nvarchar")]
    [InlineData("sqlserver:type:sysname", "sqlserver:type:nvarchar")]
    public void ResolveCompatibility_AllowsAdventureWorksAliasTypes(string sourceDataTypeId, string targetDataTypeId)
    {
        var resolution = new MetaDataTypeConversionService().ResolveCompatibility(
            MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace(),
            sourceDataTypeId,
            targetDataTypeId);

        Assert.Equal(sourceDataTypeId, resolution.SourceDataTypeId);
        Assert.Equal(targetDataTypeId, resolution.TargetDataTypeId);
        Assert.False(resolution.IsExact);
        Assert.NotEmpty(resolution.Path);
    }

    [Theory]
    [InlineData("sqlserver:type:Flag", "sqlserver:type:bit")]
    [InlineData("sqlserver:type:NameStyle", "sqlserver:type:bit")]
    [InlineData("sqlserver:type:AccountNumber", "sqlserver:type:nvarchar")]
    [InlineData("sqlserver:type:Name", "sqlserver:type:nvarchar")]
    [InlineData("sqlserver:type:OrderNumber", "sqlserver:type:nvarchar")]
    [InlineData("sqlserver:type:Phone", "sqlserver:type:nvarchar")]
    [InlineData("sqlserver:type:sysname", "sqlserver:type:nvarchar")]
    public void Resolve_WithSqlServerTargetSystem_ResolvesAliasTypesToBaseTypes(string sourceDataTypeId, string expectedTargetDataTypeId)
    {
        var resolution = new MetaDataTypeConversionService().Resolve(
            MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace(),
            sourceDataTypeId,
            "SqlServer");

        Assert.Equal(sourceDataTypeId, resolution.SourceDataTypeId);
        Assert.Equal(expectedTargetDataTypeId, resolution.TargetDataTypeId);
    }
}
