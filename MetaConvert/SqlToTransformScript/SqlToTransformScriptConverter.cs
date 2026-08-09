using System.Globalization;
using Meta.Core.Domain;
using Meta.Core.Serialization;
using MetaSql;
using MetaTransformScript.Sql;
using MTS = global::MetaTransformScript;

namespace MetaConvert.SqlToTransformScript;

public static class SqlToTransformScriptConverter
{
    public static async Task<SqlToTransformScriptConversionResult> ConvertAsync(
        string metaSqlWorkspacePath,
        string pathToNewTransformScriptWorkspace,
        CancellationToken cancellationToken = default)
    {
        return await ConvertAsync(
                metaSqlWorkspacePath,
                pathToNewTransformScriptWorkspace,
                new SqlToTransformScriptConversionOptions(),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<SqlToTransformScriptConversionResult> ConvertAsync(
        string metaSqlWorkspacePath,
        string pathToNewTransformScriptWorkspace,
        SqlToTransformScriptConversionOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(metaSqlWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(pathToNewTransformScriptWorkspace);
        ArgumentNullException.ThrowIfNull(options);
        cancellationToken.ThrowIfCancellationRequested();

        var moduleKinds = options.ModuleKinds == SqlToTransformScriptModuleKinds.None
            ? SqlToTransformScriptModuleKinds.All
            : options.ModuleKinds;

        var metaSql = await Meta.Core.Serialization.TypedWorkspaceXmlSerializer.LoadAsync<MetaSqlModel>(
                Path.GetFullPath(metaSqlWorkspacePath),
                searchUpward: false,
                cancellationToken)
            .ConfigureAwait(false);

        var modules = GetConvertibleModules(metaSql, moduleKinds);
        var outputWorkspacePath = Path.GetFullPath(pathToNewTransformScriptWorkspace);
        if (modules.Count == 0)
        {
            if (options.AllowEmpty)
            {
                var emptyModel = MTS.MetaTransformScriptModel.CreateEmpty();
                await Meta.Core.Serialization.TypedWorkspaceXmlSerializer.SaveAsync(emptyModel, outputWorkspacePath, cancellationToken)
                    .ConfigureAwait(false);
                var emptyWorkspace = await XmlWorkspaceReader
                    .OpenAsync(outputWorkspacePath, cancellationToken)
                    .ConfigureAwait(false);

                return new SqlToTransformScriptConversionResult(emptyWorkspace.State, outputWorkspacePath, 0, 0, 0);
            }

            throw new InvalidOperationException(
                "MetaSql workspace does not contain selected SQL modules to convert to MetaTransformScript.");
        }

        var transformScriptSql = new MetaTransformScriptSqlService();
        for (var i = 0; i < modules.Count; i++)
        {
            var module = modules[i];
            if (i == 0)
            {
                await transformScriptSql
                    .ImportFromSqlCodeToWorkspaceAsync(
                        module.DefinitionSql,
                        targetSqlIdentifier: null,
                        outputWorkspacePath,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await transformScriptSql
                    .AddSqlCodeToWorkspaceAsync(
                        module.DefinitionSql,
                        targetSqlIdentifier: null,
                        outputWorkspacePath,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        var workspace = await XmlWorkspaceReader
            .OpenAsync(outputWorkspacePath, cancellationToken)
            .ConfigureAwait(false);

        return new SqlToTransformScriptConversionResult(
            workspace.State,
            outputWorkspacePath,
            modules.Count(static module => module.ModuleKind == SqlToTransformScriptModuleKind.View),
            modules.Count(static module => module.ModuleKind == SqlToTransformScriptModuleKind.Function),
            modules.Count(static module => module.ModuleKind == SqlToTransformScriptModuleKind.StoredProcedure));
    }

    public static IReadOnlyList<SqlToTransformScriptModuleDefinition> GetConvertibleModules(
        MetaSqlModel metaSql)
    {
        return GetConvertibleModules(metaSql, SqlToTransformScriptModuleKinds.All);
    }

    public static IReadOnlyList<SqlToTransformScriptModuleDefinition> GetConvertibleModules(
        MetaSqlModel metaSql,
        SqlToTransformScriptModuleKinds moduleKinds)
    {
        ArgumentNullException.ThrowIfNull(metaSql);

        if (moduleKinds == SqlToTransformScriptModuleKinds.None)
        {
            moduleKinds = SqlToTransformScriptModuleKinds.All;
        }

        var modules = new List<SqlToTransformScriptModuleDefinition>();
        if (moduleKinds.HasFlag(SqlToTransformScriptModuleKinds.Functions))
        {
            modules.AddRange(metaSql.FunctionList.Select(static function => new SqlToTransformScriptModuleDefinition(
                SqlToTransformScriptModuleKind.Function,
                function.Schema.Name,
                function.Name,
                function.DefinitionSql,
                ParseDeployOrdinal(function.DeployOrdinal))));
        }

        if (moduleKinds.HasFlag(SqlToTransformScriptModuleKinds.Views))
        {
            modules.AddRange(metaSql.ViewList.Select(static view => new SqlToTransformScriptModuleDefinition(
                SqlToTransformScriptModuleKind.View,
                view.Schema.Name,
                view.Name,
                view.DefinitionSql,
                ParseDeployOrdinal(view.DeployOrdinal))));
        }

        if (moduleKinds.HasFlag(SqlToTransformScriptModuleKinds.StoredProcedures))
        {
            modules.AddRange(metaSql.StoredProcedureList.Select(static procedure => new SqlToTransformScriptModuleDefinition(
                SqlToTransformScriptModuleKind.StoredProcedure,
                procedure.Schema.Name,
                procedure.Name,
                procedure.DefinitionSql,
                ParseDeployOrdinal(procedure.DeployOrdinal))));
        }

        return modules
            .OrderBy(static module => module.DeployOrdinal)
            .ThenBy(static module => module.SchemaName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static module => module.ObjectName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static module => module.ModuleKind)
            .ToArray();
    }

    private static int ParseDeployOrdinal(string? deployOrdinal)
    {
        return int.TryParse(deployOrdinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : int.MaxValue;
    }
}

public sealed record SqlToTransformScriptConversionResult(
    InMemoryWorkspace Workspace,
    string WorkspacePath,
    int ViewCount,
    int FunctionCount,
    int StoredProcedureCount);

public sealed class SqlToTransformScriptConversionOptions
{
    public SqlToTransformScriptModuleKinds ModuleKinds { get; set; } = SqlToTransformScriptModuleKinds.All;
    public bool AllowEmpty { get; set; }
}

[Flags]
public enum SqlToTransformScriptModuleKinds
{
    None = 0,
    Views = 1,
    Functions = 2,
    StoredProcedures = 4,
    All = Views | Functions | StoredProcedures
}

public enum SqlToTransformScriptModuleKind
{
    View,
    Function,
    StoredProcedure
}

public sealed record SqlToTransformScriptModuleDefinition(
    SqlToTransformScriptModuleKind ModuleKind,
    string SchemaName,
    string ObjectName,
    string DefinitionSql,
    int DeployOrdinal);
