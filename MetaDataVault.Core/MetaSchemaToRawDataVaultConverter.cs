using Meta.Core.Domain;

namespace MetaDataVault.Core;

public sealed class MetaSchemaToRawDataVaultConverter
{
    private const string MetaSchemaModelName = "MetaSchema";
    private const string MetaBusinessKeyModelName = "MetaBusinessKey";
    private const string MetaDataVaultImplementationModelName = "MetaDataVaultImplementation";

    public Workspace Convert(
        Workspace metaSchemaWorkspace,
        Workspace businessKeyWorkspace,
        Workspace implementationWorkspace,
        string newWorkspacePath)
    {
        ArgumentNullException.ThrowIfNull(metaSchemaWorkspace);
        ArgumentNullException.ThrowIfNull(businessKeyWorkspace);
        ArgumentNullException.ThrowIfNull(implementationWorkspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        EnsureModel(metaSchemaWorkspace, MetaSchemaModelName, nameof(metaSchemaWorkspace));
        EnsureModel(businessKeyWorkspace, MetaBusinessKeyModelName, nameof(businessKeyWorkspace));
        EnsureModel(implementationWorkspace, MetaDataVaultImplementationModelName, nameof(implementationWorkspace));

        throw new InvalidOperationException(
            "MetaSchema -> MetaRawDataVault conversion requires explicit weave bindings between MetaSchema, MetaBusinessKey, and MetaDataVaultImplementation. Heuristic business-key inference was removed, and the weave-driven materialization path is not implemented yet.");
    }

    private static void EnsureModel(Workspace workspace, string expectedModelName, string parameterName)
    {
        if (!string.Equals(workspace.Model.Name, expectedModelName, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Expected sanctioned model '{expectedModelName}' but found '{workspace.Model.Name}'.",
                parameterName);
        }
    }
}
