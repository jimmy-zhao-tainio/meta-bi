using MetaDataVaultImplementation;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

internal sealed class ConversionContext
{
    public required string PathToNewMetaSqlWorkspace { get; init; }
    public required string DatabaseName { get; init; }
    public required MetaDataVaultImplementationModel ImplementationModel { get; init; }
    public SqlServerBusinessTypeLowering? BusinessTypeLowering { get; init; }
    public required MetaSqlModel MetaSql { get; init; }
    public required Database Database { get; init; }
    public required Dictionary<string, Schema> SchemasByName { get; init; }
}
