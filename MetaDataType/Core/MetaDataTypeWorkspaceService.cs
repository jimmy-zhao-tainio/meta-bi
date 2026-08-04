using MetaDataType.Instance;

namespace MetaDataType.Core;

public sealed class MetaDataTypeWorkspaceService
{
    public MetaDataTypeModel CreateWorkspace() => MetaDataTypeInstance.Default;
}
