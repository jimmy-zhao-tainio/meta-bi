namespace MetaDataType.Core;

public sealed class MetaDataTypeWorkspaceService
{
    public MetaDataTypeModel CreateWorkspace() => MetaDataTypeWorkspaceProvider.GetDefaultWorkspace();
}
