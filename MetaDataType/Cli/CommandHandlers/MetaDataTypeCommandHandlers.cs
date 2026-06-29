using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataType.Core;

internal sealed class MetaDataTypeCommandHandlers
{
    private readonly ConsolePresenter presenter;
    private readonly MetaDataTypeWorkspaceService service;

    public MetaDataTypeCommandHandlers(
        ConsolePresenter presenter,
        MetaDataTypeWorkspaceService service)
    {
        this.presenter = presenter;
        this.service = service;
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

        presenter.WriteKeyValueBlock(
            "MetaDataType workspace created",
            new[]
            {
                ("Path", result.WorkspacePath),
                ("Model", result.ModelName),
                ("DataTypeSystems", result.DataTypeSystemCount.ToString()),
                ("DataTypes", result.DataTypeCount.ToString())
            });
    }

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
