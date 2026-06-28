using MetaDataType.Core;

namespace MetaDataType.Tests;

public sealed class MetaDataTypeWorkspaceServiceTests
{
    [Fact]
    public void CreateWorkspace_CreatesSanctionedTypedWorkspace()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaDataType-service-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var result = new MetaDataTypeWorkspaceService().CreateWorkspace(workspacePath);

            Assert.Equal(Path.GetFullPath(workspacePath), result.WorkspacePath);
            Assert.Equal("MetaDataType", result.ModelName);
            Assert.True(result.DataTypeSystemCount > 0);
            Assert.True(result.DataTypeCount > 0);
            Assert.True(File.Exists(Path.Combine(workspacePath, "workspace.xml")));
            Assert.True(File.Exists(Path.Combine(workspacePath, "model.xml")));

            var model = MetaDataTypeModel.LoadFromXmlWorkspace(workspacePath);
            Assert.Equal(result.DataTypeSystemCount, model.DataTypeSystemList.Count);
            Assert.Equal(result.DataTypeCount, model.DataTypeList.Count);
        }
        finally
        {
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
