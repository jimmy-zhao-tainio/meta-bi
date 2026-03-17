using MetaDataVaultImplementation;
using MetaSql;

namespace MetaDataVault.ToMetaSql;

internal sealed class ConversionContext
{
    public required string PathToNewMetaSqlWorkspace { get; init; }
    public required string DatabaseName { get; init; }
    public required string DefaultSchemaName { get; init; }
    public required MetaDataVaultImplementationModel ImplementationModel { get; init; }
    public required MetaSqlModel MetaSql { get; init; }
    public required Database Database { get; init; }
    public required Schema DefaultSchema { get; init; }
}
