using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;

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

    public void RunNewWorkspace(MetaCliInvocation invocation)
    {
        var targetValidation = CliNewWorkspaceTargetValidator.Validate(invocation.Required("path"));
        if (!targetValidation.Ok)
        {
            Fail(
                targetValidation.ErrorMessage,
                "choose a new folder or empty the target directory and retry.",
                4,
                targetValidation.Details);
        }

        var result = service.CreateWorkspace(targetValidation.FullPath);

        presenter.WriteOk(
            "MetaDataTypeConversion workspace created",
            ("Path", result.WorkspacePath),
            ("Model", result.ModelName),
            ("ConversionImplementations", result.ConversionImplementationCount.ToString()),
            ("DataTypeMappings", result.DataTypeMappingCount.ToString()));
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
