using Meta.Integration;
using Meta.Operations.Domain;
using MetaPipeline;
using MetaSchema;
using MetaSchema.Core;
using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql;

[Trait("Category", "Integration")]
public sealed class ExplicitWorkspaceBindingIntegrationTests
{
    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task PersistedContractsAndScript_BindAndResolveForPipelineWithExactGraphs(string representation)
    {
        var root = Path.Combine(Path.GetTempPath(), "explicit-bi-workspaces", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var source = Schema("SourceRows");
            var target = Schema("TargetRows");
            var schemaContract = MetaSchemaModels.CreateMetaSchemaModel();
            var schemaBinding = new MetaSchemaWorkspaceBinding();
            var sourceState = schemaBinding.Write(source, schemaContract);
            var targetState = schemaBinding.Write(target, schemaContract);
            var sourcePath = Path.Combine(root, "source");
            var targetPath = Path.Combine(root, "target");
            await WorkspaceSurface.CreateAsync(sourceState, sourcePath, representation);
            await WorkspaceSurface.CreateAsync(targetState, targetPath, representation);

            var sql = new MetaTransformScriptSqlService();
            var script = sql.ImportSqlCode(MetaTransformScriptModel.CreateEmpty(),
                "SELECT s.Value AS Value FROM TestSystem.dbo.SourceRows AS s;", "dbo.TargetRows", "CopyText").Model;
            var scriptPath = Path.Combine(root, "script");
            var scriptBinding = new MetaTransformScriptWorkspaceBinding();
            var scriptContract = MetaTransformScriptModels.LoadContract();
            await TypedWorkspaceModelMapper.CreateAsync(script, scriptContract, scriptBinding, scriptPath, representation);

            var restored = await MetaTransformScriptInstance.LoadFromWorkspaceAsync(scriptPath);
            Assert.Same(restored.TSqlStatementList.Single(), restored.TransformScriptStatementLinkList.Single().TSqlStatement);
            Assert.Same(restored.TransformScriptList.Single(), restored.TransformScriptStatementLinkList.Single().TransformScript);
            Assert.Equal(sql.ExportToSqlCode(script), sql.ExportToSqlCode(restored));
            restored.TransformScriptList.Single().Name = "Åsa äter räksmörgås.";
            await MetaTransformScriptInstance.SaveToWorkspaceAsync(restored, scriptPath);
            var saved = await MetaTransformScriptInstance.LoadFromWorkspaceAsync(scriptPath);
            Assert.Equal("Åsa äter räksmörgås.", saved.TransformScriptList.Single().Name);
            Assert.Null(InMemoryWorkspaceComparer.FindDifference(await TypedWorkspaceModelMapper.LoadStateAsync(scriptPath),
                scriptBinding.Write(saved, scriptContract)));
            foreach (var (path, expected) in new[] { (sourcePath, sourceState), (targetPath, targetState) })
            {
                var actualState = await TypedWorkspaceModelMapper.LoadStateAsync(path);
                var actual = schemaBinding.Read(actualState);
                Assert.Same(actual.SchemaObjectList.Single(), actual.FieldList.Single().SchemaObject);
                Assert.Same(actual.FieldList.Single(), actual.FieldDataTypeDetailList.Single().Field);
                Assert.Equal("Length", actual.FieldDataTypeDetailList.Single().Name);
                Assert.Equal("50", actual.FieldDataTypeDetailList.Single().Value);
                Assert.Equal("true", actual.FieldList.Single().IsNullable);
                Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, schemaBinding.Write(actual, actualState.Model)));
            }

            var bindingPath = Path.Combine(root, "binding");
            var result = new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(scriptPath, [sourcePath],
                targetPath, "TestSystem", "dbo", bindingPath);
            Assert.Equal(0, result.ErrorCount);
            Assert.Single(result.Model.ValidationTargetColumnTypeExactList);
            var bindingState = await TypedWorkspaceModelMapper.LoadStateAsync(bindingPath);
            var projection = new MetaTransformBindingWorkspaceBinding();
            var bound = projection.Read(bindingState);
            Assert.Null(InMemoryWorkspaceComparer.FindDifference(bindingState, projection.Write(bound, bindingState.Model)));
            if (representation == "csharp")
            {
                bindingPath = Path.Combine(root, "binding-csharp");
                await WorkspaceSurface.CreateAsync(bindingState, bindingPath, representation);
            }

            var selected = new MetaPipelineTransformSelectionResolver().Resolve(scriptPath, bindingPath, saved.TransformScriptList.Single().Name);
            var execution = new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(scriptPath, bindingPath,
                selected.TransformScriptId, selected.TransformBindingId);
            Assert.Equal(sql.ExportToSqlCode(saved), execution.SourceSql);
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    private static MetaSchemaModel Schema(string tableName)
    {
        var model = MetaSchemaModel.CreateEmpty();
        var system = new MetaSchema.System { Id = "system", Name = "TestSystem" };
        var schema = new Schema { Id = "schema", Name = "dbo", System = system };
        var table = new SchemaObject { Id = "object", Name = tableName, Schema = schema };
        model.SystemList.Add(system);
        model.SchemaList.Add(schema);
        model.SchemaObjectList.Add(table);
        model.TableList.Add(new Table { Id = "table", SchemaObject = table });
        var field = new Field { Id = "field", Name = "Value", Ordinal = "0", SchemaObject = table,
            MetaDataTypeId = "sqlserver:type:nvarchar", IsNullable = "true" };
        model.FieldList.Add(field);
        model.FieldDataTypeDetailList.Add(new FieldDataTypeDetail { Id = "length", Name = "Length", Value = "50", Field = field });
        return model;
    }
}
