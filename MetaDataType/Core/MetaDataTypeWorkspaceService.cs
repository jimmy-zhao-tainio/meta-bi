using MetaDataType.Instance;

namespace MetaDataType.Core;

public sealed class MetaDataTypeWorkspaceService
{
    public CreateMetaDataTypeWorkspaceResult CreateWorkspace(string workspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);

        var fullPath = Path.GetFullPath(workspacePath);
        Directory.CreateDirectory(fullPath);

        var model = MetaDataTypeInstance.Default;
        model.SaveToXmlWorkspace(fullPath);

        return new CreateMetaDataTypeWorkspaceResult(
            fullPath,
            "MetaDataType",
            model.DataTypeSystemList.Count,
            model.DataTypeList.Count);
    }
}

public sealed record CreateMetaDataTypeWorkspaceResult(
    string WorkspacePath,
    string ModelName,
    int DataTypeSystemCount,
    int DataTypeCount);
