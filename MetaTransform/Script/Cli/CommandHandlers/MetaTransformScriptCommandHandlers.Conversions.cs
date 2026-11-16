using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaConvert.TransformScriptConversion;
using MetaDataType;
using MetaDataTypeConversion;
using MetaSchema;
using MetaTransformScript;
using MetaTransformScript.Sql;
using Meta.Integration;

internal sealed partial class MetaTransformScriptCommandHandlers
{
    public async Task RunApplyConversionsAsync(MetaCliInvocation invocation, MetaCliWorkspaces workspaces)
    {
        try
        {
            var output = MetaCliWorkspace.OptionalOutputLocation(invocation)
                ?? throw new InvalidOperationException("Choose a new output workspace.");
            var sources = invocation.Values("source-schema");
            TransformScriptConversionWorkspaceService.EnsureSeparateOutput(output, [invocation.Required("workspace"),
                invocation.Required("data-type-workspace"), invocation.Required("data-type-conversion-workspace"), .. sources]);
            using (var activity = CliActivityLine.Start("Converting"))
            {
                var result = new TransformScriptConversionWorkspaceService().Convert(
                    await workspaces.RequiredAsync("workspace", new MetaTransformScriptWorkspaceBinding()),
                    await workspaces.RequiredAsync("data-type-workspace", new MetaDataTypeWorkspaceBinding()),
                    await workspaces.RequiredAsync("data-type-conversion-workspace", new MetaDataTypeConversionWorkspaceBinding()),
                    sources.Select(path => TypedWorkspaceModelMapper.Load(path, new MetaSchemaWorkspaceBinding())).ToArray());
                await workspaces.CreateAsync("output", result.Model, MetaTransformScriptModels.LoadContract(), new MetaTransformScriptWorkspaceBinding());
            }
            presenter.WriteOk();
        }
        catch (Exception ex)
        {
            Fail("Cannot apply type conversions.", "check the source schema and authored conversion rules, then retry.", 4, [ex.Message]);
        }
    }
}
