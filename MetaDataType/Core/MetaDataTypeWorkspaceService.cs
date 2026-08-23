namespace MetaDataType.Core;

public sealed class MetaDataTypeWorkspaceService
{
    public MetaDataTypeModel CreateWorkspace() => MetaDataTypeInstance.BuiltIn;
}
