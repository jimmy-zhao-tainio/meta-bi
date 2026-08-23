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

    public async Task RunCreate(
        MetaCliInvocation invocation,
        MetaCliWorkspaces workspaces)
    {
        var model = service.CreateWorkspace();
        await workspaces.CreateAsync("output", model).ConfigureAwait(false);

        presenter.WriteKeyValueBlock(
            "MetaDataType workspace created",
            new[]
            {
                ("Path", MetaCliWorkspace.OutputLocation(invocation)),
                ("Model", "MetaDataType"),
                ("DataTypeSystems", model.DataTypeSystemList.Count.ToString()),
                ("DataTypes", model.DataTypeList.Count.ToString())
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
