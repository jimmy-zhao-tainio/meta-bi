using Meta.Integration;
using MetaSchema;
using MetaTransform.Binding;
using MetaTransformScript;
using MetaTransformScript.Sql;

public sealed class TransformScriptNavigatorIntegrationTests
{
    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task PersistedQueries_KeepExpressionOrderAndCteOwnershipDuringExportAndBinding(string representation)
    {
        var root = NewRoot();
        try
        {
            var service = new MetaTransformScriptSqlService();
            var model = service.ImportFromSqlCode(
                """
                CREATE VIEW dbo.FirstProjection AS
                WITH stage (Code, Fallback, Other) AS
                (SELECT Code, Fallback, Other FROM src.SourceOne)
                SELECT LEFT(s.Code, 2) AS Prefix,
                       RIGHT(s.Fallback, 3) AS Suffix,
                       COALESCE(s.Code, s.Fallback, N'2', N'3', N'4', N'5', N'6', N'7', N'8', N'9', N'10', N'11') AS Chosen,
                       REPLACE(s.Other, N'x', N'y') AS Replaced
                FROM stage AS s;
                GO
                CREATE VIEW dbo.SecondProjection AS
                WITH stage (Code, Fallback, Other) AS
                (SELECT Code, Fallback, Other FROM src.SourceTwo)
                SELECT LEFT(s.Other, 5) AS Prefix,
                       RIGHT(s.Code, 6) AS Suffix,
                       COALESCE(s.Other, N'z') AS Chosen,
                       REPLACE(s.Fallback, N'a', N'b') AS Replaced
                FROM stage AS s;
                """);
            var path = Path.Combine(root, "script");
            await TypedWorkspaceModelMapper.CreateAsync(model, MetaTransformScriptModels.LoadContract(), new MetaTransformScriptWorkspaceBinding(), path, representation);
            var restored = await MetaTransformScriptInstance.LoadFromWorkspaceAsync(path);
            var navigator = new TransformScriptNavigator(restored);

            AssertParameters("dbo.FirstProjection", ["s.Code", "2"], ["s.Fallback", "3"],
                ["s.Code", "s.Fallback", "N'2'", "N'3'", "N'4'", "N'5'", "N'6'", "N'7'", "N'8'", "N'9'", "N'10'", "N'11'"],
                ["s.Other", "N'x'", "N'y'"]);
            AssertParameters("dbo.SecondProjection", ["s.Other", "5"], ["s.Code", "6"],
                ["s.Other", "N'z'"], ["s.Fallback", "N'a'", "N'b'"]);

            var bound = new TransformBindingService().BindTransforms(restored, restored.TransformScriptList, SourceSchema());
            Assert.Equal(2, bound.Count);
            foreach (var result in bound)
            {
                Assert.False(result.HasErrors, string.Join(" | ", result.Issues.Select(issue => issue.Message)));
                Assert.NotNull(result.TopLevelRowset);
                Assert.Equal(new[] { "Prefix", "Suffix", "Chosen", "Replaced" }, result.TopLevelRowset.Columns.Select(column => column.Name));
                var sourceName = result.TransformScriptName == "dbo.FirstProjection" ? "SourceOne" : "SourceTwo";
                var otherSourceName = sourceName == "SourceOne" ? "SourceTwo" : "SourceOne";
                Assert.Contains(result.TableSources, source => source.SqlIdentifier.EndsWith(sourceName, StringComparison.Ordinal));
                Assert.DoesNotContain(result.TableSources, source => source.SqlIdentifier.EndsWith(otherSourceName, StringComparison.Ordinal));
                Assert.Equal(new[] { "Code", "Fallback", "Other" }, result.ColumnReferences
                    .Select(reference => reference.ResolvedColumn.Name).Distinct().OrderBy(name => name, StringComparer.Ordinal));

                var exported = service.ExportToSqlCode(restored, result.TransformScriptName);
                Assert.Equal(service.ExportToSqlCode(model, result.TransformScriptName), exported);
                Assert.Equal(exported, service.ExportToSqlCode(service.ImportFromSqlCode(exported, "roundtrip")));
            }

            void AssertParameters(string scriptName, string[] left, string[] right, string[] coalesce, string[] replace)
            {
                var script = restored.TransformScriptList.Single(row => row.Name == scriptName);
                var cte = Assert.Single(navigator.GetCommonTableExpressions(script));
                Assert.Equal("stage", navigator.TryGetCommonTableExpressionName(cte));
                Assert.Equal(new[] { "Code", "Fallback", "Other" }, navigator.GetCommonTableExpressionColumnAliases(cte));
                var statement = Assert.IsType<SelectStatement>(navigator.TryGetSelectStatement(script));
                var query = Assert.IsType<QuerySpecification>(navigator.TryGetQuerySpecification(statement));
                var expressions = navigator.GetSelectElements(query)
                    .Select(element => Assert.IsType<SelectScalarExpression>(navigator.TryGetSelectScalarExpression(element)))
                    .ToDictionary(projection => navigator.TryGetSelectScalarExpressionAlias(projection)!,
                        projection => Assert.IsType<ScalarExpression>(navigator.TryGetSelectScalarExpressionBody(projection)));
                Assert.Equal(left, Render(navigator.GetLeftFunctionCallParameters(Assert.IsType<LeftFunctionCall>(navigator.TryGetLeftFunctionCall(expressions["Prefix"])))));
                Assert.Equal(right, Render(navigator.GetRightFunctionCallParameters(Assert.IsType<RightFunctionCall>(navigator.TryGetRightFunctionCall(expressions["Suffix"])))));
                Assert.Equal(coalesce, Render(navigator.GetCoalesceExpressions(Assert.IsType<CoalesceExpression>(navigator.TryGetCoalesceExpression(expressions["Chosen"])))));
                Assert.Equal(replace, Render(navigator.GetFunctionCallParameters(Assert.IsType<FunctionCall>(navigator.TryGetFunctionCall(expressions["Replaced"])))));
            }

            string[] Render(IReadOnlyList<ScalarExpression> expressions) => expressions
                .Select(expression => service.ExportScalarExpression(new SqlScalarExpression(restored, expression))).ToArray();
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task PersistedProcedures_KeepContractOperationAndResultOwnersIsolated(string representation)
    {
        var root = NewRoot();
        try
        {
            var service = new MetaTransformScriptSqlService();
            var model = service.ImportFromSqlCode(
                """
                CREATE PROCEDURE dbo.FirstProcedure AS BEGIN SELECT 1 AS Ignored; END
                GO
                CREATE PROCEDURE dbo.SecondProcedure AS BEGIN SELECT 2 AS Ignored; END
                """);
            foreach (var prefix in new[] { "First", "Second" })
                service.AddStoredProcedureContract(model, $"dbo.{prefix}Procedure", new StoredProcedureContractDeclaration(
                    [new StoredProcedureContractOperationDeclaration(20, "Read", $"src.{prefix}Source"),
                        new StoredProcedureContractOperationDeclaration(10, "Call", $"audit.{prefix}Audit")],
                    [new StoredProcedureResultRowsetDeclaration(prefix + "Rows",
                        [new StoredProcedureResultColumnDeclaration(prefix + "Code", "sqlserver:type:int", false),
                            new StoredProcedureResultColumnDeclaration(prefix + "Name", "sqlserver:type:nvarchar", true)])]));

            var path = Path.Combine(root, "script");
            await TypedWorkspaceModelMapper.CreateAsync(model, MetaTransformScriptModels.LoadContract(), new MetaTransformScriptWorkspaceBinding(), path, representation);
            var restored = await MetaTransformScriptInstance.LoadFromWorkspaceAsync(path);
            var navigator = new TransformScriptNavigator(restored);
            foreach (var prefix in new[] { "First", "Second" })
            {
                var script = restored.TransformScriptList.Single(row => row.Name == $"dbo.{prefix}Procedure");
                Assert.Single(navigator.GetStoredProcedureContracts(script));
                Assert.Equal(new[] { $"audit.{prefix}Audit", $"src.{prefix}Source" },
                    navigator.GetStoredProcedureOperations(script).Select(operation => operation.SqlIdentifier));
                var rowset = Assert.Single(navigator.GetStoredProcedureResultRowsets(script));
                Assert.Equal(prefix + "Rows", rowset.Name);
                Assert.Equal(new[] { prefix + "Code", prefix + "Name" },
                    navigator.GetStoredProcedureResultColumns(rowset).Select(column => column.Name));
                var bound = new TransformBindingService().BindTransform(restored, script);
                Assert.False(bound.HasErrors, string.Join(" | ", bound.Issues.Select(issue => issue.Message)));
                Assert.Contains(bound.Rowsets, row => row.SqlIdentifier == $"src.{prefix}Source");
            }
            Assert.Equal(service.ExportModuleDefinitions(model).Select(module => module.DefinitionSql),
                service.ExportModuleDefinitions(restored).Select(module => module.DefinitionSql));
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    private static MetaSchemaModel SourceSchema()
    {
        var model = MetaSchemaModel.CreateEmpty();
        var system = new MetaSchema.System { Id = "system", Name = "Db" };
        var schema = new Schema { Id = "schema", Name = "src", System = system };
        model.SystemList.Add(system);
        model.SchemaList.Add(schema);
        foreach (var name in new[] { "SourceOne", "SourceTwo" })
        {
            var table = new SchemaObject { Id = name, Name = name, Schema = schema };
            model.SchemaObjectList.Add(table);
            model.TableList.Add(new Table { Id = name, SchemaObject = table });
            foreach (var (fieldName, ordinal) in new[] { ("Code", "0"), ("Fallback", "1"), ("Other", "2") })
                model.FieldList.Add(new Field
                {
                    Id = name + ":" + fieldName, Name = fieldName, Ordinal = ordinal,
                    MetaDataTypeId = "sqlserver:type:nvarchar", IsNullable = "true", SchemaObject = table
                });
        }
        return model;
    }

    private static string NewRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "navigator-direct-access", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
