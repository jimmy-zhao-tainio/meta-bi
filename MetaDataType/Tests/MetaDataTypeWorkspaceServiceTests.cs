using MetaDataType.Core;

namespace MetaDataType.Tests;

public sealed class MetaDataTypeWorkspaceServiceTests
{
    [Fact]
    public void CreateWorkspace_ReturnsSanctionedModel()
    {
        var model = new MetaDataTypeWorkspaceService().CreateWorkspace();

        Assert.NotEmpty(model.DataTypeSystemList);
        Assert.NotEmpty(model.DataTypeList);
    }
}
