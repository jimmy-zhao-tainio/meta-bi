using MSS = global::MetaSqlScript;
using MTS = global::MetaTransformScript;

namespace MetaTransformScript.Sql;

public sealed partial class MetaTransformScriptSqlService
{
    public ImportToWorkspaceResult ImportSqlScriptWorkspace(
        MTS.MetaTransformScriptModel model,
        MSS.MetaSqlScriptModel source)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(source);

        var requests = new List<SqlCodeImportRequest>(source.SqlScriptList.Count);
        foreach (var script in source.SqlScriptList.OrderBy(
                     static script => script.Id,
                     StringComparer.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(script.SqlText))
            {
                throw new MetaTransformScriptSqlImportException(
                    MetaTransformScriptSqlImportFailureKind.InvalidSqlInput,
                    $"SQL script '{script.Id}' does not contain SQL text.");
            }

            requests.Add(new SqlCodeImportRequest(
                script.SqlText,
                TargetSqlIdentifier: null,
                script.Name));
        }

        return ImportSqlCodeBatch(model, requests);
    }
}
