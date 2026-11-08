using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaConvert.TransformScriptConversion;
using MetaDataType;
using MetaDataTypeConversion;
using MetaSchema;
using MetaTransformScript;
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
                    await workspaces.RequiredAsync<MetaTransformScriptModel>("workspace"),
                    await workspaces.RequiredAsync<MetaDataTypeModel>("data-type-workspace"),
                    await workspaces.RequiredAsync<MetaDataTypeConversionModel>("data-type-conversion-workspace"),
                    sources.Select(path => TypedWorkspaceModelMapper.Load<MetaSchemaModel>(path, searchUpward: false)).ToArray());
                await workspaces.CreateAsync("output", result.Model);
            }
            presenter.WriteOk();
        }
        catch (Exception ex)
        {
            Fail("Cannot apply type conversions.", "check the source schema and authored conversion rules, then retry.", 4, [ex.Message]);
        }
    }
}
