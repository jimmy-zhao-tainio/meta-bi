using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;
using System.Globalization;
using Meta.Integration;
using MetaDataType;
using MetaTransformScript.Sql;

internal sealed class MetaDataTypeConversionCommandHandlers
{
    private readonly ConsolePresenter presenter;
    private readonly IMetaDataTypeConversionService service;
    private readonly string appName;

    public MetaDataTypeConversionCommandHandlers(
        ConsolePresenter presenter,
        IMetaDataTypeConversionService service,
        string appName)
    {
        this.presenter = presenter;
        this.service = service;
        this.appName = appName;
    }

    public async Task RunCreate(
        MetaCliInvocation invocation,
        MetaCliWorkspaces workspaces)
    {
        var model = service.CreateWorkspace();
        await workspaces.CreateAsync("output", model).ConfigureAwait(false);

        presenter.WriteOk(
            "MetaDataTypeConversion workspace created",
            ("Path", MetaCliWorkspace.OutputLocation(invocation)),
            ("Model", "MetaDataTypeConversion"),
            ("ConversionImplementations", model.ConversionImplementationList.Count.ToString()),
            ("DataTypeMappings", model.DataTypeMappingList.Count.ToString()));
    }

    public void RunCheck(MetaCliInvocation invocation, MetaDataTypeConversionModel model)
    {
        var result = service.Check(model);
        if (result.HasErrors)
        {
            Fail(
                "Cannot check data-type conversions.",
                "fix the sanctioned mappings and rerun check.",
                2,
                result.Errors.Select(error => $"  - {error}"));
        }

        presenter.WriteOk(
            "MetaDataTypeConversion check",
            ("ConversionImplementations", result.ImplementationCount.ToString()),
            ("DataTypeMappings", result.MappingCount.ToString()),
            ("Errors", "0"));
    }

    public void RunResolve(MetaCliInvocation invocation, MetaDataTypeConversionModel model)
    {
        var sourceDataTypeId = invocation.Required("source-data-type");
        var targetDataTypeSystemName = invocation.Optional("target-data-type-system");

        try
        {
            var resolution = string.IsNullOrWhiteSpace(targetDataTypeSystemName)
                ? service.Resolve(model, sourceDataTypeId)
                : service.Resolve(model, sourceDataTypeId, targetDataTypeSystemName);
            var details = new List<(string Key, string Value)>
            {
                ("SourceDataTypeId", resolution.SourceDataTypeId),
                ("TargetDataTypeId", resolution.TargetDataTypeId),
                ("TargetDataTypeSystem", resolution.TargetDataTypeSystemName),
                ("ConversionImplementation", resolution.ConversionImplementationName)
            };

            if (!string.IsNullOrWhiteSpace(resolution.Notes))
            {
                details.Add(("Notes", resolution.Notes));
            }

            presenter.WriteOk("MetaDataTypeConversion resolve", details.ToArray());
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            Fail(
                "Cannot resolve data-type conversion.",
                HelpCommand("resolve"),
                4,
                [$"  {ex.Message}"]);
        }
    }

    public void RunSqlExpression(MetaCliInvocation invocation, MetaDataTypeConversionModel model)
    {
        try
        {
            int? ReadInteger(string name)
            {
                var text = invocation.Optional(name);
                if (text is null) return null;
                return int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var number)
                    ? number : throw new ArgumentException($"--{name} requires a nonnegative integer.");
            }
            var lengthText = invocation.Optional("length");
            var isMax = string.Equals(lengthText, "max", StringComparison.OrdinalIgnoreCase);
            var facets = new SqlServerTypeFacets(isMax ? null : ReadInteger("length"), isMax, ReadInteger("precision"), ReadInteger("scale"));
            var catalogPath = invocation.Optional("data-types-workspace");
            var catalog = catalogPath is null ? MetaDataTypeInstance.BuiltIn
                : TypedWorkspaceModelMapper.Load<MetaDataTypeModel>(Path.GetFullPath(catalogPath), searchUpward: false);
            var result = service.GenerateSqlExpression(model, catalog, new(
                invocation.Required("source-data-type"), invocation.Required("target-data-type-system"),
                invocation.Required("input-expression"), facets));
            presenter.WriteKeyValueBlock("MetaDataTypeConversion SQL expression", new[]
            {
                ("Mapping", result.Mapping.MappingId),
                ("TargetDataTypeId", result.Mapping.TargetDataTypeId),
                ("ConversionImplementation", result.Mapping.ConversionImplementationName),
                ("Sql", new MetaTransformScriptSqlService().ExportScalarExpression(result.Expression))
            });
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            Fail("Cannot generate SQL value conversion.", HelpCommand("sql-expression"), 4, [$"  {ex.Message}"]);
        }
    }

    private string HelpCommand(string commandName) => $"{appName} help {commandName}";

    private void Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details.Where(static detail => !string.IsNullOrWhiteSpace(detail)));
        }

        renderedDetails.Add($"Next: {next}");
        presenter.WriteFailure(message, renderedDetails);
        throw new MetaCliExitException(exitCode);
    }
}
