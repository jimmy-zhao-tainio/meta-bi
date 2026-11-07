using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaConvert.TransformScriptConversion;
using MetaDataType;
using MetaDataTypeConversion;
using MetaSchema;
using MetaTransformScript;

internal sealed partial class MetaTransformScriptCommandHandlers
{
    public async Task RunApplyConversionsAsync(MetaCliInvocation invocation, MetaCliWorkspaces workspaces)
    {
        try
        {
            var output = MetaCliWorkspace.OptionalOutputLocation(invocation)
                ?? throw new InvalidOperationException("Choose a new output workspace.");
            TransformScriptConversionWorkspaceService.EnsureSeparateOutput(output, invocation.Required("workspace"),
                invocation.Required("source-schema"), invocation.Required("data-type-workspace"), invocation.Required("data-type-conversion-workspace"));
            using (var activity = CliActivityLine.Start("Converting"))
            {
                var result = new TransformScriptConversionWorkspaceService().Convert(
                    await workspaces.RequiredAsync<MetaTransformScriptModel>("workspace"),
                    await workspaces.RequiredAsync<MetaDataTypeModel>("data-type-workspace"),
                    await workspaces.RequiredAsync<MetaDataTypeConversionModel>("data-type-conversion-workspace"),
                    await workspaces.RequiredAsync<MetaSchemaModel>("source-schema"));
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
