using System.Globalization;
using Meta.Core.Domain;
using Meta.Core.Serialization;
using MetaSql;
using MetaTransformScript.Sql;

namespace MetaConvert.TransformScriptToSql;

public static class TransformScriptToSqlConverter
{
    public static async Task<InMemoryWorkspace> ConvertAsync(
        string transformScriptWorkspacePath,
        string pathToNewMetaSqlWorkspace,
        string databaseName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformScriptWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(pathToNewMetaSqlWorkspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var modules = new MetaTransformScriptSqlService().ExportModuleDefinitions(transformScriptWorkspacePath);
        var metaSql = ConvertToMetaSql(modules, databaseName);
        Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(metaSql, pathToNewMetaSqlWorkspace);

        var outputWorkspace = await XmlWorkspaceReader
            .OpenAsync(pathToNewMetaSqlWorkspace, cancellationToken)
            .ConfigureAwait(false);
        return outputWorkspace.State;
    }

    public static MetaSqlModel ConvertToMetaSql(
        IReadOnlyList<MetaTransformScriptSqlModuleDefinition> modules,
        string databaseName)
    {
        ArgumentNullException.ThrowIfNull(modules);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var context = new ConversionContext(databaseName.Trim());
        foreach (var module in modules
                     .OrderBy(static item => item.DeployOrdinal)
                     .ThenBy(static item => item.SchemaName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static item => item.ObjectName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static item => item.TransformScriptId, StringComparer.Ordinal))
        {
            context.AddModule(module);
        }

        return context.MetaSql;
    }

    private sealed class ConversionContext
    {
        private readonly Dictionary<string, Schema> schemasByName = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> moduleKeys = new(StringComparer.OrdinalIgnoreCase);

        public ConversionContext(string databaseName)
        {
            MetaSql = MetaSqlModel.CreateEmpty();
            Database = new Database
            {
                Id = databaseName,
                Name = databaseName,
            };
            MetaSql.DatabaseList.Add(Database);
        }

        public MetaSqlModel MetaSql { get; }
        public Database Database { get; }

        public void AddModule(MetaTransformScriptSqlModuleDefinition module)
        {
            var schema = GetOrAddSchema(module.SchemaName);
            var moduleKey = $"{schema.Name}.{module.ObjectName}";
            if (!moduleKeys.Add(moduleKey))
            {
                throw new InvalidOperationException(
                    $"Duplicate transform-script SQL module '{moduleKey}' cannot be lowered to MetaSql.");
            }

            var deployOrdinal = module.DeployOrdinal.ToString(CultureInfo.InvariantCulture);
            switch (module.ModuleKind)
            {
                case MetaTransformScriptSqlModuleKind.View:
                    MetaSql.ViewList.Add(new View
                    {
                        Id = $"{Database.Id}.{schema.Name}.{module.ObjectName}",
                        Schema = schema,
                        Name = module.ObjectName,
                        DefinitionSql = module.DefinitionSql,
                        DeployOrdinal = deployOrdinal,
                    });
                    break;

                case MetaTransformScriptSqlModuleKind.InlineTableValuedFunction:
                case MetaTransformScriptSqlModuleKind.ScalarFunction:
                    MetaSql.FunctionList.Add(new Function
                    {
                        Id = $"{Database.Id}.{schema.Name}.{module.ObjectName}",
                        Schema = schema,
                        Name = module.ObjectName,
                        FunctionKind = module.ModuleKind.ToString(),
                        DefinitionSql = module.DefinitionSql,
                        DeployOrdinal = deployOrdinal,
                    });
                    break;

                case MetaTransformScriptSqlModuleKind.StoredProcedure:
                    MetaSql.StoredProcedureList.Add(new StoredProcedure
                    {
                        Id = $"{Database.Id}.{schema.Name}.{module.ObjectName}",
                        Schema = schema,
                        Name = module.ObjectName,
                        DefinitionSql = module.DefinitionSql,
                        DeployOrdinal = deployOrdinal,
                    });
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported transform-script SQL module kind '{module.ModuleKind}'.");
            }
        }

        private Schema GetOrAddSchema(string schemaName)
        {
            if (schemasByName.TryGetValue(schemaName, out var schema))
            {
                return schema;
            }

            schema = new Schema
            {
                Id = $"{Database.Id}.{schemaName}",
                Database = Database,
                Name = schemaName,
            };
            schemasByName.Add(schemaName, schema);
            MetaSql.SchemaList.Add(schema);
            return schema;
        }
    }
}
