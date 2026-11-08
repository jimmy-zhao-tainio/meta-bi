using Meta.Integration;
using MetaBi.Tests.Common;
using MetaConvert.TransformScriptConversion;
using MetaDataType;
using MetaDataTypeConversion;
using MetaSchema;
using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql;
using MetaPipeline;
using Microsoft.Data.SqlClient;
using System.Data;

[Trait("Category", "Integration")]
public sealed class TransformScriptConversionIntegrationTests
{
    [Fact]
    public void Cli_ResolvesReadsFromRepeatedSchemaInputs_WithWorkspaceLocalIdentities()
    {
        using var f = new Fixture(true, onlyFirst: true,
            firstSql: "SELECT a.Id, a.TextValue AS Converted FROM TestSystem.dbo.SourceRows a JOIN OtherSystem.dbo.SourceRows b ON a.TextValue = b.TextValue;");
        var other = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(f.Source), MetaSchemaModel.CreateEmpty);
        other.SystemList.Single().Name = "OtherSystem";
        var otherPath = f.PathOf("OtherSource");
        TypedWorkspaceModelMapper.Create(other, otherPath, "csharp");
        var before = f.Snapshot();
        var result = CliTestRunner.RunStandardCli("meta-transform-script",
            $"apply-conversions --workspace \"{f.Input}\" --source-schema \"{f.SourcePath}\" --source-schema \"{otherPath}\" --data-type-workspace \"{f.PathOf("Catalog")}\" --data-type-conversion-workspace \"{f.PathOf("Conversions")}\" --output-xml \"{f.PathOf("MultiSystem")}\"");
        Assert.True(result.ExitCode == 0, result.Output);
        var model = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(f.PathOf("MultiSystem"), searchUpward: false);
        Assert.Equal(3, model.CastCallList.Count);
        var sql = f.Syntax.ExportToSqlCode(model);
        Assert.Contains("TestSystem.dbo.SourceRows", sql);
        Assert.Contains("OtherSystem.dbo.SourceRows", sql);
        Assert.Equal(before, f.Snapshot());
        Assert.Equal("OtherSystem", TypedWorkspaceModelMapper.Load<MetaSchemaModel>(otherPath, searchUpward: false).SystemList.Single().Name);
    }

    [Theory]
    [InlineData("xml", false)]
    [InlineData("csharp", true)]
    public void Cli_RewritesEverySourceReadAcrossScriptCollection_AndPreservesInputs(string surface, bool arbitraryIds)
    {
        using var f = new Fixture(arbitraryIds, complex: true);
        var before = f.Snapshot();
        var output = f.RunCli("Converted", surface);
        var model = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(output, searchUpward: false);
        Assert.Equal(new[] { "First", "Second" }, model.TransformScriptList.Select(script => script.Name));
        Assert.Equal(new[] { "dbo.TargetOne", "dbo.TargetTwo" }, model.TransformScriptList.Select(script => model.ScriptObjectViewList.Single(view => view.TransformScript.Id == script.Id).TargetSqlIdentifier));
        Assert.True(model.CastCallList.Count == 8, string.Join("\n", model.TransformScriptList.Select(script => f.Syntax.ExportToSqlCode(model, script.Name))));
        var sql = f.Syntax.ExportToSqlCode(model, "First");
        Assert.Contains("JOIN dbo.LookupRows", sql);
        Assert.Contains("LEN(", sql);
        Assert.DoesNotContain("CAST((LEN(", sql);
        Assert.Contains("N's.TextValue' AS Literal", sql);
        Assert.Contains("WHERE", sql);
        Assert.Contains("ORDER BY", sql);
        var second = f.Syntax.ExportToSqlCode(model, "Second");
        Assert.Contains("AS [TextValue]", second);
        Assert.Contains("ORDER BY TextValue", second);
        Assert.Equal(before, f.Snapshot());
    }

    [Fact]
    public void WorkspaceApi_IsSourceDriven_AndSnippetChangesAlterTheProducedSql()
    {
        using var f = new Fixture(true);
        var before = f.Snapshot();
        var first = new TransformScriptConversionWorkspaceService().Apply(f.Request("FirstResult"));
        Assert.Equal(2, first.ScriptCount);
        Assert.Equal(3, first.ConvertedReferenceCount);
        Assert.Equal(before, f.Snapshot());
        f.AssertBound(first.WorkspacePath);
        f.Conversions.SqlConversionSnippetList.Single().TransformScript = "CONVERT($( destination ), COALESCE($( source ), N'0'))";
        f.SaveConversions();
        before = f.Snapshot();
        var changed = new TransformScriptConversionWorkspaceService().Apply(f.Request("Changed"));
        var sql = f.Syntax.ExportToSqlCode(changed.WorkspacePath, "First");
        Assert.Contains("CONVERT(int, COALESCE", sql);
        Assert.NotEqual(f.Syntax.ExportToSqlCode(first.WorkspacePath, "First"), sql);
        f.AssertBound(changed.WorkspacePath);
        Assert.Equal(before, f.Snapshot());
    }

    [Fact]
    public void TypesWithoutRulesAndWorkspacesWithoutApplicableRules_AreUnchanged()
    {
        using var f = new Fixture(true);
        f.Conversions.DataTypeMappingList.Clear();
        f.SaveConversions();
        var result = new TransformScriptConversionWorkspaceService().Apply(f.Request("NoRules"));
        Assert.Equal(0, result.ConvertedReferenceCount);
        foreach (var name in new[] { "First", "Second" })
            Assert.Equal(f.Syntax.ExportToSqlCode(f.Input, name), f.Syntax.ExportToSqlCode(result.WorkspacePath, name));
    }

    [Fact]
    public void StructuralSubstitution_PreservesExpressionPrecedence_AndDoesNotRewriteInsertedReferences()
    {
        using var f = new Fixture(true, firstSql: "SELECT s.Id * 2 AS Converted FROM dbo.SourceRows s;", onlyFirst: true);
        f.Conversions.DataTypeMappingList.Clear();
        var implementation = f.Conversions.ConversionImplementationList.Single();
        f.Conversions.DataTypeMappingList.Add(new DataTypeMapping { Id = "increment", SourceDataTypeId = f.TypeId("int"), TargetDataTypeId = f.TypeId("int"), ConversionImplementation = implementation });
        f.Conversions.SqlConversionSnippetList.Single().TransformScript = "$(source) + 1";
        f.Conversions.SqlConversionParameterBindingList.RemoveAll(binding => binding.Name == "destination");
        f.SaveConversions();
        var result = new TransformScriptConversionWorkspaceService().Apply(f.Request("Precedence"));
        Assert.Equal(1, result.ConvertedReferenceCount);
        var sql = f.Syntax.ExportToSqlCode(result.WorkspacePath);
        Assert.Contains("((s.Id) + 1) * 2 AS Converted", sql);
    }

    [Fact]
    public void ExistingOuterCastIsPreserved_AndFinalBindingRemainsIndependent()
    {
        using var f = new Fixture(true, firstSql: "SELECT s.Id, CAST(s.TextValue AS nvarchar(50)) AS Converted FROM dbo.SourceRows s;", onlyFirst: true);
        var output = f.RunCli("InnerConversion");
        var model = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(output, searchUpward: false);
        Assert.Equal(2, model.CastCallList.Count);
        Assert.Contains("CAST((CAST((s.TextValue) AS int)) AS nvarchar(50))", f.Syntax.ExportToSqlCode(model));
        var error = Assert.Throws<TransformBindingValidationException>(() => f.AssertBound(output));
        Assert.Equal("TargetColumnTypeConformanceMismatch", error.Code);
    }

    [Fact]
    public void EscapedSnippetTextAndSourceLiterals_AreNotRescanned()
    {
        using var f = new Fixture(true, destination: "nvarchar", firstSql: "SELECT s.TextValue AS Converted, N'$(source)' AS Literal FROM dbo.SourceRows s;", onlyFirst: true);
        f.Conversions.SqlConversionSnippetList.Single().TransformScript = "CAST(COALESCE($(source), N'$$(fallback)') AS $(destination)(25))";
        f.SaveConversions();
        var output = f.RunCli("Escaped");
        var sql = f.Syntax.ExportToSqlCode(output);
        Assert.Contains("N'$(fallback)'", sql);
        Assert.Contains("N'$(source)' AS Literal", sql);
    }

    [Theory]
    [InlineData("ambiguous", "ambiguous")]
    [InlineData("missing-snippet", "no authored SQL conversion snippet")]
    [InlineData("placeholder", "no valid binding")]
    [InlineData("binding", "ambiguous bindings")]
    [InlineData("statement", "Expected EndOfFile")]
    [InlineData("foreign-target", "does not belong")]
    [InlineData("unknown-target", "selected MetaDataType catalog")]
    [InlineData("unknown-source", "selected MetaDataType catalog")]
    public void Cli_FailuresLeaveNoOutputAndPreserveInputs(string failure, string diagnostic)
    {
        using var f = new Fixture(true);
        switch (failure)
        {
            case "ambiguous":
                f.Conversions.DataTypeMappingList.Add(new DataTypeMapping { Id = "competing", SourceDataTypeId = f.TypeId("nvarchar"), TargetDataTypeId = f.TypeId("decimal"), ConversionImplementation = f.Conversions.ConversionImplementationList.Single() }); break;
            case "missing-snippet": f.Conversions.ConversionImplementationList.Single().SqlConversionSnippet = null; break;
            case "placeholder": f.Conversions.SqlConversionSnippetList.Single().TransformScript = "CAST($(missing) AS $(destination))"; break;
            case "binding":
                var binding = f.Conversions.SqlConversionParameterBindingList.First();
                f.Conversions.SqlConversionParameterBindingList.Add(new SqlConversionParameterBinding { Id = "duplicate", Name = binding.Name, SqlConversionSnippet = binding.SqlConversionSnippet, SqlConversionParameter = binding.SqlConversionParameter }); break;
            case "statement": f.Conversions.SqlConversionSnippetList.Single().TransformScript = "CAST($(source) AS $(destination)); SELECT 1"; break;
            case "foreign-target": f.Conversions.DataTypeMappingList.Single().TargetDataTypeId = "meta:type:String"; break;
            case "unknown-target": f.Conversions.DataTypeMappingList.Single().TargetDataTypeId = "sqlserver:type:not-cataloged"; break;
            case "unknown-source":
                f.Source.FieldList.First(field => field.Name == "TextValue").MetaDataTypeId = "sqlserver:type:not-cataloged";
                TypedWorkspaceModelMapper.Create(f.Source, f.SourcePath, "xml"); break;
        }
        f.SaveConversions();
        var before = f.Snapshot();
        var result = f.Cli("Failed");
        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains(diagnostic, result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.False(Directory.Exists(Path.Combine(f.Root, "Failed")));
        Assert.Equal(before, f.Snapshot());
    }

    [Theory]
    [InlineData("SELECT x.TextValue FROM dbo.SourceRows s;", "Cannot resolve source reads")]
    [InlineData("SELECT s.Missing FROM dbo.SourceRows s;", "Cannot resolve source reads")]
    [InlineData("SELECT s.Id FROM dbo.SourceRows s JOIN dbo.LookupRows l ON s.TextValue = x.TextValue;", "Cannot resolve source reads")]
    [InlineData("SELECT s.TextValue.value(N'.', N'nvarchar(50)') AS Converted FROM dbo.SourceRows s;", "conversion of method call targets is unsupported")]
    public void UnsupportedOrUnresolvedReads_FailBeforeOutput(string sql, string diagnostic)
    {
        using var f = new Fixture(true, firstSql: sql, onlyFirst: true);
        var result = f.Cli("Unsupported");
        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains(diagnostic, result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.False(Directory.Exists(Path.Combine(f.Root, "Unsupported")));
    }

    [Theory]
    [InlineData("nvarchar", "CAST($(source) AS $(destination)(max))")]
    [InlineData("float", "CAST($(source) AS $(destination)(24))")]
    [InlineData("datetime2", "CAST($(source) AS $(destination)(3))")]
    public void DeclaredUnsupportedDestinationFacets_RemainExplicitFailures(string destination, string snippet)
    {
        using var f = new Fixture(true, destination: destination);
        f.Conversions.SqlConversionSnippetList.Single().TransformScript = snippet;
        f.SaveConversions();
        var result = f.Cli("UnsupportedFacet");
        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("unsupported in this slice", result.Output);
        Assert.False(Directory.Exists(Path.Combine(f.Root, "UnsupportedFacet")));
    }

    [Fact]
    public void CliHelpAndOutputGuards_ExposeOnlySourceDrivenInputs()
    {
        var help = CliTestRunner.RunStandardCli("meta-transform-script", "apply-conversions --help");
        Assert.Equal(0, help.ExitCode);
        Assert.Contains("--source-schema", help.Output);
        Assert.Contains("--data-type-conversion-workspace", help.Output);
        Assert.DoesNotContain("requirements", help.Output);
        Assert.DoesNotContain("target-schema", help.Output);
        using var f = new Fixture(true);
        f.RunCli("Existing");
        Assert.NotEqual(0, f.Cli("Existing").ExitCode);
        Assert.NotEqual(0, f.Cli(Path.Combine("Input", "Nested")).ExitCode);
        Assert.False(Directory.Exists(Path.Combine(f.Input, "Nested")));
    }

    [Theory]
    [InlineData("int", false)]
    [InlineData("nvarchar", false)]
    [InlineData("int", true)]
    [InlineData("nvarchar", true)]
    public async Task CliCollectionThenIndependentBindingAndPipeline_ExecutesExactSourceDrivenSemantics(string destination, bool arbitraryIds)
    {
        using var f = new Fixture(arbitraryIds, destination: destination);
        if (destination == "int")
        {
            var error = Assert.Throws<TransformBindingValidationException>(() => f.AssertBound(f.Input));
            Assert.Equal("TargetColumnTypeConformanceMismatch", error.Code);
        }
        var output = f.RunCli("Executable");
        f.AssertBound(output);
        var scripts = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(output, searchUpward: false);
        var bindings = TypedWorkspaceModelMapper.Load<MetaTransformBindingModel>(output + "-Binding", searchUpward: false);
        var database = "MetaBiSourceConversion_" + Guid.NewGuid().ToString("N");
        await using var master = new SqlConnection("Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false;Pooling=false");
        await master.OpenAsync();
        await using (var create = new SqlCommand($"CREATE DATABASE [{database}]", master)) await create.ExecuteNonQueryAsync();
        try
        {
            var cs = new SqlConnectionStringBuilder(master.ConnectionString) { InitialCatalog = database }.ConnectionString;
            await using var db = new SqlConnection(cs);
            await db.OpenAsync();
            var targetSql = destination == "int" ? "int" : "nvarchar(25)";
            await using (var setup = new SqlCommand($"CREATE TABLE dbo.SourceRows(Id int NOT NULL, TextValue nvarchar(50) NULL); CREATE TABLE dbo.TargetOne(Id int NOT NULL, Converted {targetSql} NULL); CREATE TABLE dbo.TargetTwo(Id int NOT NULL, Converted {targetSql} NULL);", db)) await setup.ExecuteNonQueryAsync();
            string?[] values = destination == "int" ? ["2", "10", null] : ["ordinary text", "\u00c5sa \u00e4ter r\u00e4ksm\u00f6rg\u00e5s.", null, ""];
            for (var i = 0; i < values.Length; i++)
            {
                await using var insert = new SqlCommand("INSERT dbo.SourceRows VALUES(@id,@value)", db);
                insert.Parameters.Add("@id", SqlDbType.Int).Value = i + 1;
                insert.Parameters.Add("@value", SqlDbType.NVarChar, 50).Value = (object?)values[i] ?? DBNull.Value;
                await insert.ExecuteNonQueryAsync();
            }
            foreach (var script in scripts.TransformScriptList)
            {
                var binding = bindings.TransformBindingList.Single(row => row.MetaTransformScriptTransformScriptId == script.Id);
                var selected = script.Name == "Second" && destination == "int" ? new[] { 0, 1 } : Enumerable.Range(0, values.Length).ToArray();
                if (arbitraryIds)
                {
                    var env = "METABI_CONVERSION_" + Guid.NewGuid().ToString("N");
                    Environment.SetEnvironmentVariable(env, cs);
                    try
                    {
                        var cli = CliTestRunner.RunStandardCli("meta-pipeline",
                            $"execute-sqlserver --transform-workspace \"{output}\" --binding-workspace \"{output}-Binding\" --execution-connection-env {env} --target-connection-env {env} --script {script.Name} --binding {binding.Id} --batch-size 2 --data-type-workspace \"{Path.Combine(f.Root, "Catalog")}\"");
                        Assert.True(cli.ExitCode == 0, cli.Output);
                    }
                    finally { Environment.SetEnvironmentVariable(env, null); }
                }
                else
                {
                    var execution = await new MetaPipelineSqlServerExecutionService().ExecuteAsync(new(output, output + "-Binding", cs, cs, script.Id, binding.Id, BatchSize: 2));
                    Assert.True(execution.Succeeded, execution.FailureMessage);
                    Assert.Equal(selected.Length, execution.RowCount);
                }
                var target = scripts.ScriptObjectViewList.Single(view => view.TransformScript.Id == script.Id).TargetSqlIdentifier;
                await using var verify = new SqlCommand($"SELECT Id,Converted FROM {target} ORDER BY Id", db);
                await using var reader = await verify.ExecuteReaderAsync();
                foreach (var index in selected)
                {
                    Assert.True(await reader.ReadAsync());
                    Assert.Equal(index + 1, reader.GetInt32(0));
                    object? expected = values[index] is null ? null : destination == "int" ? int.Parse(values[index]!) : values[index];
                    Assert.Equal(expected, reader.IsDBNull(1) ? null : reader.GetValue(1));
                }
                Assert.False(await reader.ReadAsync());
            }
        }
        finally
        {
            await using var drop = new SqlCommand($"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{database}]", master);
            await drop.ExecuteNonQueryAsync();
            await using var check = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name=@name", master);
            check.Parameters.AddWithValue("@name", database);
            Assert.Equal(0, (int)(await check.ExecuteScalarAsync())!);
        }
    }

    [Theory]
    [InlineData("SELECT s.* FROM dbo.SourceRows s ORDER BY s.TextValue;")]
    [InlineData("WITH c AS (SELECT s.* FROM dbo.SourceRows s) SELECT c.TextValue FROM c;")]
    [InlineData("SELECT s.TextValue FROM dbo.SourceRows s WHERE EXISTS (SELECT 1 FROM dbo.LookupRows l WHERE l.TextValue=s.TextValue);")]
    [InlineData("SELECT s.TextValue FROM dbo.SourceRows s UNION ALL SELECT l.TextValue FROM dbo.LookupRows l;")]
    [InlineData("SELECT s.TextValue, COUNT(*) AS C FROM dbo.SourceRows s GROUP BY s.TextValue HAVING COUNT(s.TextValue)>0;")]
    [InlineData("SELECT s.Id, ROW_NUMBER() OVER (PARTITION BY s.TextValue ORDER BY s.TextValue) AS Position FROM dbo.SourceRows s;")]
    public void Cli_UsesExistingQueryScopesAndExpandsImplicitReads(string sql)
    {
        using var f = new Fixture(true, firstSql: sql, onlyFirst: true);
        var before = f.Snapshot();
        var output = f.RunCli("QueryScopes", "csharp");
        var model = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(output, searchUpward: false);
        Assert.NotEmpty(model.CastCallList);
        Assert.Empty(model.SelectStarExpressionList);
        Assert.Equal(before, f.Snapshot());
        Assert.Contains("CAST(", f.Syntax.ExportToSqlCode(model));
    }

    [Fact]
    public void Cli_PreservesFunctionEnvelopeAndAuthoredMetadata()
    {
        using var f = new Fixture(true, firstSql: "CREATE FUNCTION dbo.ReadRows() RETURNS TABLE AS RETURN (SELECT s.Id, s.TextValue FROM dbo.SourceRows s);", onlyFirst: true, statement: true);
        var original = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(f.Input, searchUpward: false);
        var output = f.RunCli("Function");
        var model = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(output, searchUpward: false);
        Assert.Equal(original.TransformScriptList.Single().Id, model.TransformScriptList.Single().Id);
        Assert.Equal(original.ScriptObjectTVFList.Single().Id, model.ScriptObjectTVFList.Single().Id);
        Assert.Single(model.CastCallList);
        Assert.Contains("CAST(", f.Syntax.ExportModuleDefinitions(model).Single().DefinitionSql);
    }

    [Theory]
    [InlineData("insert")]
    [InlineData("cte-insert")]
    [InlineData("update")]
    [InlineData("merge")]
    [InlineData("delete")]
    [InlineData("truncate")]
    [InlineData("values")]
    public async Task Cli_ConvertsMutationReads_AndBindingAndPipelineExecuteThem(string operation)
    {
        var sql = operation switch
        {
            "insert" => "INSERT dbo.TargetOne (Id, Converted) SELECT s.Id, s.TextValue FROM dbo.SourceRows s;",
            "cte-insert" => "WITH c AS (SELECT s.* FROM dbo.SourceRows s) INSERT dbo.TargetOne (Id, Converted) SELECT c.Id,c.TextValue FROM c WHERE c.TextValue>=N'2';",
            "update" => "UPDATE t SET Converted=s.TextValue FROM dbo.TargetOne t INNER JOIN dbo.SourceRows s ON s.Id=t.Id;",
            "merge" => "MERGE dbo.TargetOne AS t USING (SELECT s.Id,s.TextValue FROM dbo.SourceRows s) AS src ON t.Id=src.Id WHEN MATCHED THEN UPDATE SET Converted=src.TextValue WHEN NOT MATCHED THEN INSERT (Id,Converted) VALUES(src.Id,src.TextValue);",
            "delete" => "DELETE t FROM dbo.TargetOne t WHERE EXISTS (SELECT 1 FROM dbo.SourceRows s WHERE s.Id=t.Id AND s.TextValue>=N'2');",
            "truncate" => "TRUNCATE TABLE dbo.TargetOne;",
            "values" => "INSERT dbo.TargetOne(Id,Converted) VALUES(4,12);",
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };
        using var f = new Fixture(true, firstSql: sql, onlyFirst: true, statement: true);
        var before = f.Snapshot();
        var output = f.RunCli("Mutation");
        Assert.Equal(before, f.Snapshot());
        var model = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(output, searchUpward: false);
        var bound = f.Bind(output);
        Assert.Equal(0, bound.ErrorCount);
        if (operation is "values" or "truncate") Assert.Empty(model.CastCallList);
        else Assert.NotEmpty(model.CastCallList);
        var database = "MetaBiConvertedMutation_" + Guid.NewGuid().ToString("N");
        await using var master = new SqlConnection("Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false;Pooling=false");
        await master.OpenAsync();
        await using (var create = new SqlCommand($"CREATE DATABASE [{database}]", master)) await create.ExecuteNonQueryAsync();
        try
        {
            var cs = new SqlConnectionStringBuilder(master.ConnectionString) { InitialCatalog = database }.ConnectionString;
            await using var db = new SqlConnection(cs);
            await db.OpenAsync();
            await using (var setup = new SqlCommand("CREATE TABLE dbo.SourceRows(Id int NOT NULL, TextValue nvarchar(50) NULL); INSERT dbo.SourceRows VALUES(1,N'2'),(2,N'10'),(3,NULL); CREATE TABLE dbo.TargetOne(Id int NOT NULL, Converted int NULL);", db)) await setup.ExecuteNonQueryAsync();
            if (operation is "update" or "merge" or "delete" or "truncate")
                await using (var seed = new SqlCommand("INSERT dbo.TargetOne VALUES(1,0),(2,0),(3,0),(9,99)", db)) await seed.ExecuteNonQueryAsync();
            var script = model.TransformScriptList.Single();
            var execution = await new MetaPipelineSqlServerExecutionService().ExecuteAsync(new(output, output + "-Binding", cs, null,
                script.Id, bound.Model.TransformBindingList.Single().Id, TargetWriteModelName: "None"));
            Assert.True(execution.Succeeded, execution.FailureMessage);
            (int Id, int? Value)[] expected = operation switch
            {
                "insert" => [(1,2),(2,10),(3,null)],
                "cte-insert" => [(1,2),(2,10)],
                "update" or "merge" => [(1,2),(2,10),(3,null),(9,99)],
                "delete" => [(3,0),(9,99)],
                "values" => [(4,12)],
                _ => []
            };
            await using var verify = new SqlCommand("SELECT Id,Converted FROM dbo.TargetOne ORDER BY Id", db);
            await using var reader = await verify.ExecuteReaderAsync();
            foreach (var row in expected)
            {
                Assert.True(await reader.ReadAsync());
                Assert.Equal(row.Id, reader.GetInt32(0));
                Assert.Equal(row.Value, reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1));
            }
            Assert.False(await reader.ReadAsync());
        }
        finally
        {
            await using var drop = new SqlCommand($"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{database}]", master);
            await drop.ExecuteNonQueryAsync();
            await using var check = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name=@name", master);
            check.Parameters.AddWithValue("@name", database);
            Assert.Equal(0, (int)(await check.ExecuteScalarAsync())!);
        }
    }

    [Fact]
    public void Cli_PreservesStoredProcedureContractOnOpaqueInvocation()
    {
        using var f = new Fixture(true, firstSql: "CREATE PROCEDURE dbo.ReadStored AS BEGIN SELECT 1 AS Value; END;", onlyFirst: true, statement: true);
        var original = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(f.Input, searchUpward: false);
        var script = original.TransformScriptList.Single();
        script.SourcePath = "authored-procedure.sql";
        f.Syntax.AddStoredProcedureContract(original, script.Name, new StoredProcedureContractDeclaration(
            [new StoredProcedureContractOperationDeclaration(10, "Call", "dbo.ReadStored")], [], "Keep authored contract"));
        TypedWorkspaceModelMapper.Save(original, f.Input);
        var before = f.Snapshot();
        var output = f.RunCli("Procedure", "csharp");
        var model = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(output, searchUpward: false);
        Assert.Equal(before, f.Snapshot());
        Assert.Equal(script.Id, model.TransformScriptList.Single().Id);
        Assert.Equal(script.SourcePath, model.TransformScriptList.Single().SourcePath);
        Assert.Equal(original.StoredProcedureContractList.Single().Id, model.StoredProcedureContractList.Single().Id);
        Assert.Equal("Keep authored contract", model.StoredProcedureContractList.Single().Notes);
        Assert.Equal("dbo.ReadStored", model.StoredProcedureContractOperationList.Single().SqlIdentifier);
        Assert.Equal(f.Syntax.ExportToSqlCode(original), f.Syntax.ExportToSqlCode(model));
    }

    private sealed class Fixture : IDisposable
    {
        public string Root { get; } = Path.Combine(Path.GetTempPath(), "SourceConversion", Guid.NewGuid().ToString("N"));
        public string Input => Path.Combine(Root, "Input");
        public string SourcePath => Path.Combine(Root, "Source");
        public MetaTransformScriptSqlService Syntax { get; } = new();
        public MetaDataTypeModel Catalog { get; } = MetaDataTypeInstance.CreateBuiltIn();
        public MetaDataTypeConversionModel Conversions { get; } = MetaDataTypeConversionModel.CreateEmpty();
        public MetaSchemaModel Source { get; }
        public string PathOf(string name) => Path.Combine(Root, name);
        public string TypeId(string name) => Catalog.DataTypeList.Single(type => type.Name == name && type.DataTypeSystem.Name == "SqlServer").Id;
        public Fixture(bool arbitraryIds, bool complex = false, string destination = "int", string? firstSql = null, bool onlyFirst = false, bool statement = false)
        {
            if (arbitraryIds)
            {
                foreach (var type in Catalog.DataTypeList.Where(type => type.DataTypeSystem.Name == "SqlServer")) type.Id = "user-" + type.Name;
                Catalog.DataTypeSystemList.Single(system => system.Name == "SqlServer").Id = "arbitrary-system";
            }
            Source = Schema(("SourceRows", "TextValue", "nvarchar", 50), ("LookupRows", "TextValue", "nvarchar", 50), ("TargetOne", "Converted", destination, destination == "nvarchar" ? 25 : null));
            TypedWorkspaceModelMapper.Create(Source, SourcePath, "xml");
            TypedWorkspaceModelMapper.Create(Schema(("TargetOne", "Converted", destination, destination == "nvarchar" ? 25 : null), ("TargetTwo", "Converted", destination, destination == "nvarchar" ? 25 : null)), PathOf("Target"), "xml");
            TypedWorkspaceModelMapper.Create(Catalog, PathOf("Catalog"), "xml");
            firstSql ??= complex ? "SELECT s.TextValue AS Converted, s.Id, LEN(s.TextValue) AS Size, N's.TextValue' AS Literal FROM dbo.SourceRows s JOIN dbo.LookupRows l ON s.TextValue = l.TextValue WHERE s.TextValue IS NULL OR s.TextValue > N'0' ORDER BY s.TextValue;" : "SELECT s.Id, s.TextValue AS Converted FROM dbo.SourceRows s WHERE s.Id > 0;";
            Syntax.ImportFromSqlCodeToXmlWorkspaceAsync(firstSql, statement ? null : "dbo.TargetOne", Input, "First").GetAwaiter().GetResult();
            if (!onlyFirst)
            {
                var second = complex ? "SELECT s.TextValue, s.Id FROM dbo.SourceRows s ORDER BY TextValue;" : destination == "int" ? "SELECT s.Id, s.TextValue AS Converted FROM dbo.SourceRows s WHERE s.Id > 0 AND s.TextValue >= N'2';" : "SELECT s.Id, s.TextValue AS Converted FROM dbo.SourceRows s WHERE s.Id > 0;";
                Syntax.AddSqlCodeToWorkspaceAsync(second, "dbo.TargetTwo", Input, "Second").GetAwaiter().GetResult();
            }
            var snippet = new SqlConversionSnippet { Id = "snippet", TransformScript = destination == "nvarchar" ? "CAST($(source) AS $(destination)(25))" : "CAST($(source) AS $(destination))" };
            var implementation = new ConversionImplementation { Id = "implementation", Name = "Authored rule", SqlConversionSnippet = snippet };
            Conversions.SqlConversionSnippetList.Add(snippet); Conversions.ConversionImplementationList.Add(implementation);
            Conversions.DataTypeMappingList.Add(new DataTypeMapping { Id = "mapping", SourceDataTypeId = TypeId("nvarchar"), TargetDataTypeId = TypeId(destination), ConversionImplementation = implementation });
            foreach (var pair in new[] { ("source", "SourceExpression"), ("destination", "DestinationType") })
            {
                var parameter = new SqlConversionParameter { Id = pair.Item1, Name = pair.Item2 };
                Conversions.SqlConversionParameterList.Add(parameter);
                Conversions.SqlConversionParameterBindingList.Add(new SqlConversionParameterBinding { Id = pair.Item1, Name = pair.Item1, SqlConversionParameter = parameter, SqlConversionSnippet = snippet });
            }
            SaveConversions();
        }
        public void SaveConversions() => TypedWorkspaceModelMapper.Create(Conversions, PathOf("Conversions"), "xml");
        public TransformScriptConversionRequest Request(string output) => new(Input, PathOf("Catalog"), PathOf("Conversions"), [SourcePath], PathOf(output));
        public (int ExitCode, string Output) Cli(string output, string surface = "xml") => CliTestRunner.RunStandardCli("meta-transform-script",
            $"apply-conversions --workspace \"{Input}\" --source-schema \"{SourcePath}\" --data-type-workspace \"{PathOf("Catalog")}\" --data-type-conversion-workspace \"{PathOf("Conversions")}\" --output-{surface} \"{PathOf(output)}\"");
        public string RunCli(string output, string surface = "xml")
        {
            var result = Cli(output, surface);
            Assert.True(result.ExitCode == 0, result.Output);
            Assert.Contains("Ok", result.Output);
            return PathOf(output);
        }
        public void AssertBound(string output)
        {
            var result = Bind(output);
            Assert.Equal(0, result.ErrorCount);
            Assert.Equal(4, result.Model.ValidationTargetColumnTypeExactList.Count);
        }
        public BindToWorkspaceResult Bind(string output) => new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(
            output, [SourcePath], PathOf("Target"), "TestSystem", "dbo", output + "-Binding", dataTypeWorkspacePath: PathOf("Catalog"));
        public string Snapshot() => string.Join("\n", new[] { "Input", "Source", "Target", "Catalog", "Conversions" }.SelectMany(name => Directory.GetFiles(PathOf(name), "*", SearchOption.AllDirectories)).OrderBy(path => path).Select(path => Path.GetRelativePath(Root, path) + ":" + System.Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path)))));
        public void Dispose() { if (Directory.Exists(Root)) Directory.Delete(Root, true); }
        private MetaSchemaModel Schema(params (string Table, string Column, string Type, int? Length)[] tables)
        {
            var model = MetaSchemaModel.CreateEmpty();
            var system = new MetaSchema.System { Id = "system", Name = "TestSystem" };
            var schema = new Schema { Id = "schema", Name = "dbo", System = system };
            model.SystemList.Add(system); model.SchemaList.Add(schema);
            foreach (var table in tables)
            {
                var obj = new SchemaObject { Id = table.Table, Name = table.Table, Schema = schema };
                model.SchemaObjectList.Add(obj); model.TableList.Add(new Table { Id = table.Table, SchemaObject = obj });
                model.FieldList.Add(new Field { Id = table.Table + ":Id", Name = "Id", Ordinal = "1", IsNullable = "false", MetaDataTypeId = TypeId("int"), SchemaObject = obj });
                var value = new Field { Id = table.Table + ":Value", Name = table.Column, Ordinal = "2", IsNullable = "true", MetaDataTypeId = TypeId(table.Type), SchemaObject = obj };
                model.FieldList.Add(value);
                if (table.Length.HasValue) model.FieldDataTypeDetailList.Add(new FieldDataTypeDetail { Id = table.Table + ":Length", Name = "Length", Value = table.Length.Value.ToString(), Field = value });
            }
            return model;
        }
    }
}
