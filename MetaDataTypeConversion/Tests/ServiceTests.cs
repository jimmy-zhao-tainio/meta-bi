using MetaDataTypeConversion.Core;

namespace MetaDataTypeConversion.Tests;

public sealed class ServiceTests
{
    [Fact]
    public void CreateWorkspace_CreatesSanctionedTypedWorkspace()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaDataTypeConversion-service-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var result = new MetaDataTypeConversionService().CreateWorkspace(workspacePath);

            Assert.Equal(Path.GetFullPath(workspacePath), result.WorkspacePath);
            Assert.Equal("MetaDataTypeConversion", result.ModelName);
            Assert.True(result.ConversionImplementationCount > 0);
            Assert.True(result.DataTypeMappingCount > 0);
            Assert.True(File.Exists(Path.Combine(workspacePath, "workspace.xml")));
            Assert.True(File.Exists(Path.Combine(workspacePath, "model.xml")));

            var model = MetaDataTypeConversionModel.LoadFromXmlWorkspace(workspacePath);
            Assert.Equal(result.ConversionImplementationCount, model.ConversionImplementationList.Count);
            Assert.Equal(result.DataTypeMappingCount, model.DataTypeMappingList.Count);
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

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

    [Theory]
    [InlineData("sqlserver:type:int", "sqlserver:type:nvarchar")]
    public void ResolveCompatibility_AllowsSanctionedPath(string sourceDataTypeId, string targetDataTypeId)
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

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
