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
            throw new InvalidOperationException(targetValidation.ErrorMessage);
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
}
