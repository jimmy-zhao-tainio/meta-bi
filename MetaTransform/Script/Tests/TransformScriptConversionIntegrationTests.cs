using Meta.Integration;
using MetaConvert.TransformScriptConversion;
using MetaDataType;
using MetaDataTypeConversion;
using MetaSchema;
using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql;
using Microsoft.Data.SqlClient;
using System.Data;

[Trait("Category", "Integration")]
public sealed class TransformScriptConversionIntegrationTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PersistConvertReloadExportAndBind_UsesAuthoredSnippetAndSelectedCatalog(bool arbitraryIds)
    {
        using var fixture = new Fixture("int", arbitraryIds);
        var before = fixture.Snapshot();
        var output = fixture.Convert("First");
        var sql = fixture.Syntax.ExportToSqlCode(output);
        Assert.Contains("CAST((CAST(s.TextValue AS nvarchar(50))) AS int) AS Converted", sql);
        Assert.Contains("s.Id", sql);
        Assert.Contains("WHERE s.Id > 0", sql);
        fixture.AssertBound(output);

        fixture.Conversions.SqlConversionSnippetList.Single().TransformScript = "CONVERT($( destination ), $( source ))";
        fixture.SaveConversions();
        var changed = fixture.Convert("Changed");
        var changedSql = fixture.Syntax.ExportToSqlCode(changed);
        Assert.Contains("CONVERT(int, (CAST(s.TextValue AS nvarchar(50)))) AS Converted", changedSql);
        Assert.NotEqual(sql, changedSql);
        fixture.AssertBound(changed);
        Assert.Equal(before, fixture.Snapshot());
    }

    [Fact]
    public void ImplicitColumnNameAndOtherProjections_ArePreserved()
    {
        using var fixture = new Fixture("nvarchar", true, "s.TextValue");
        var output = fixture.Convert("Implicit");
        var reloaded = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(output, searchUpward: false);
        var analysis = new TransformBindingService().BindTransform(reloaded, reloaded.TransformScriptList.Single(), fixture.Source, fixture.Catalog);
        Assert.Equal(new[] { "Id", "TextValue" }, analysis.TopLevelRowset!.Columns.Select(column => column.Name));
        Assert.Equal(25, analysis.TopLevelRowset.Columns[1].DataType!.Length);
        fixture.AssertBound(output);
    }

    [Fact]
    public void InsertedSourceSql_IsNotRescannedForSnippetPlaceholders()
    {
        using var fixture = new Fixture("nvarchar", true, "N'$(destination)' AS Converted");
        var output = fixture.Convert("LiteralPlaceholder");
        Assert.Contains("N'$(destination)'", fixture.Syntax.ExportToSqlCode(output));
        fixture.AssertBound(output);
    }

    [Fact]
    public void MultipleProjectionRequirements_PreserveOrderAndAllInputArtifacts()
    {
        using var fixture = new Fixture("int", true);
        var intType = fixture.Catalog.DataTypeList.Single(type => type.Name == "int" && type.DataTypeSystem.Name == "SqlServer").Id;
        fixture.Conversions.DataTypeMappingList.Add(new DataTypeMapping { Id = "int-to-int", SourceDataTypeId = intType, TargetDataTypeId = intType,
            ConversionImplementation = fixture.Conversions.ConversionImplementationList.Single() });
        fixture.SaveConversions();
        var input = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(Path.Combine(fixture.Root, "Input"), searchUpward: false);
        var before = fixture.AllInputArtifacts();
        var output = Path.Combine(fixture.Root, "Multiple");
        var requirements = input.SelectScalarExpressionList.Select(projection => new ProjectionConversionRequirement(projection.Id, intType, new())).Reverse().ToArray();
        var request = new TransformScriptConversionRequest(Path.Combine(fixture.Root, "Input"), Path.Combine(fixture.Root, "Catalog"),
            Path.Combine(fixture.Root, "Conversions"), Path.Combine(fixture.Root, "Source"), requirements, output);
        Assert.Equal(2, new TransformScriptConversionWorkspaceService().Apply(request).ConvertedProjectionCount);
        fixture.AssertBound(output);
        Assert.Equal(before, fixture.AllInputArtifacts());
        Assert.Contains("already exists", Assert.Throws<InvalidOperationException>(() => new TransformScriptConversionWorkspaceService().Apply(request)).Message);
        Assert.Equal(before, fixture.AllInputArtifacts());
        var nested = request with { OutputWorkspace = Path.Combine(fixture.Root, "Input", "Nested") };
        Assert.Contains("separate", Assert.Throws<InvalidOperationException>(() => new TransformScriptConversionWorkspaceService().Apply(nested)).Message);
    }

    [Theory]
    [InlineData("missing", "found 0")]
    [InlineData("ambiguous", "found 2")]
    [InlineData("snippet", "no authored SQL conversion snippet")]
    [InlineData("placeholder", "no valid binding")]
    [InlineData("binding", "ambiguous bindings")]
    [InlineData("unsupported", "Unsupported")]
    [InlineData("statement", "scalar")]
    [InlineData("wrong-result", "exact destination")]
    public void InvalidConversions_FailClearlyWithoutWritingOutput(string failure, string diagnostic)
    {
        using var fixture = new Fixture("nvarchar", true,
            failure == "unsupported" ? "s.TextValue + N'X' AS Converted" : null);
        switch (failure)
        {
            case "missing": fixture.Conversions.DataTypeMappingList.Clear(); break;
            case "ambiguous":
                var mapping = fixture.Conversions.DataTypeMappingList.Single();
                fixture.Conversions.DataTypeMappingList.Add(new DataTypeMapping { Id = "duplicate", SourceDataTypeId = mapping.SourceDataTypeId,
                    TargetDataTypeId = mapping.TargetDataTypeId, ConversionImplementation = mapping.ConversionImplementation }); break;
            case "snippet": fixture.Conversions.ConversionImplementationList.Single().SqlConversionSnippet = null; break;
            case "placeholder": fixture.Conversions.SqlConversionSnippetList.Single().TransformScript = "CAST($(absent) AS $(destination))"; break;
            case "binding":
                var binding = fixture.Conversions.SqlConversionParameterBindingList.First();
                fixture.Conversions.SqlConversionParameterBindingList.Add(new SqlConversionParameterBinding { Id = "duplicate", Name = binding.Name,
                    SqlConversionParameter = binding.SqlConversionParameter, SqlConversionSnippet = binding.SqlConversionSnippet }); break;
            case "statement": fixture.Conversions.SqlConversionSnippetList.Single().TransformScript = "CAST($(source) AS $(destination)); SELECT 1"; break;
            case "wrong-result": fixture.Conversions.SqlConversionSnippetList.Single().TransformScript = "CAST(CAST($(source) AS $(destination)) AS nvarchar(100))"; break;
        }
        fixture.SaveConversions();
        var before = fixture.Snapshot();
        var ex = Assert.ThrowsAny<Exception>(() => fixture.Convert("Failed"));
        Assert.Contains(diagnostic, ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.False(Directory.Exists(Path.Combine(fixture.Root, "Failed")));
        Assert.Equal(before, fixture.Snapshot());
    }

    [Theory]
    [InlineData("int")]
    [InlineData("decimal")]
    [InlineData("nvarchar")]
    public async Task GeneratedAndBoundScript_ExecutesOnSqlServerWithExactValuesAndNulls(string destination)
    {
        using var fixture = new Fixture(destination, true);
        var output = fixture.Convert("Executable");
        fixture.AssertBound(output);
        var database = "MetaBiConversion_" + Guid.NewGuid().ToString("N");
        await using var master = new SqlConnection("Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false;Pooling=false");
        await master.OpenAsync();
        await using (var create = new SqlCommand($"CREATE DATABASE [{database}]", master)) await create.ExecuteNonQueryAsync();
        try
        {
            var cs = new SqlConnectionStringBuilder(master.ConnectionString) { InitialCatalog = database }.ConnectionString;
            await using var db = new SqlConnection(cs);
            await db.OpenAsync();
            await using (var setup = new SqlCommand("CREATE TABLE dbo.SourceRows(Id int NOT NULL, TextValue nvarchar(max) NULL)", db)) await setup.ExecuteNonQueryAsync();
            string?[] input = destination == "nvarchar" ? ["ordinary text", "\u00c5sa \u00e4ter r\u00e4ksm\u00f6rg\u00e5s.", null, ""] :
                destination == "decimal" ? ["12.50", "-7.25", null] : ["12", "-7", null];
            for (var i = 0; i < input.Length; i++)
            {
                await using var insert = new SqlCommand("INSERT dbo.SourceRows VALUES (@id,@value)", db);
                insert.Parameters.Add("@id", SqlDbType.Int).Value = i + 1;
                insert.Parameters.Add("@value", SqlDbType.NVarChar, -1).Value = (object?)input[i] ?? DBNull.Value;
                await insert.ExecuteNonQueryAsync();
            }
            async Task Verify(string workspace, bool replacesNull)
            {
                await using var select = new SqlCommand(fixture.Syntax.ExportToSqlCode(workspace), db);
                await using var reader = await select.ExecuteReaderAsync();
                var actual = new Dictionary<int, object?>();
                while (await reader.ReadAsync()) actual.Add(reader.GetInt32(0), reader.IsDBNull(1) ? null : reader.GetValue(1));
                Assert.Equal(input.Length, actual.Count);
                for (var i = 0; i < input.Length; i++)
                {
                    object? expected = input[i] is null ? (replacesNull ? 0 : null) : destination switch
                    {
                        "int" => int.Parse(input[i]!, System.Globalization.CultureInfo.InvariantCulture),
                        "decimal" => decimal.Parse(input[i]!, System.Globalization.CultureInfo.InvariantCulture),
                        _ => input[i]
                    };
                    Assert.Equal(expected, actual[i + 1]);
                }
            }
            await Verify(output, false);
            if (destination == "int")
            {
                fixture.Conversions.SqlConversionSnippetList.Single().TransformScript = "CAST(COALESCE($(source), N'0') AS $(destination))";
                fixture.SaveConversions();
                var changed = fixture.Convert("NullPolicy");
                fixture.AssertBound(changed);
                await Verify(changed, true);
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

    private sealed class Fixture : IDisposable
    {
        public string Root { get; } = Path.Combine(Path.GetTempPath(), "TransformScriptConversion", Guid.NewGuid().ToString("N"));
        public MetaTransformScriptSqlService Syntax { get; } = new();
        public MetaDataTypeModel Catalog { get; } = MetaDataTypeInstance.CreateBuiltIn();
        public MetaDataTypeConversionModel Conversions { get; } = MetaDataTypeConversionModel.CreateEmpty();
        public MetaSchemaModel Source { get; }
        private readonly string destinationId;
        private readonly SqlServerTypeFacets facets;
        private readonly string projectionId;
        private string PathOf(string name) => Path.Combine(Root, name);

        public Fixture(string destination, bool arbitraryIds, string? expression = null)
        {
            if (arbitraryIds)
            {
                foreach (var type in Catalog.DataTypeList.Where(type => type.DataTypeSystem.Name == "SqlServer")) type.Id = "user-" + type.Name;
                Catalog.DataTypeSystemList.Single(system => system.Name == "SqlServer").Id = "arbitrary-system";
            }
            string Id(string name) => Catalog.DataTypeList.Single(type => type.DataTypeSystem.Name == "SqlServer" && type.Name == name).Id;
            destinationId = Id(destination);
            facets = destination switch { "nvarchar" => new(Length: 25), "decimal" => new(Precision: 8, Scale: 2), _ => new() };
            Source = Schema("SourceRows", Id("int"), Id("nvarchar"), -1);
            var target = Schema("TargetRows", Id("int"), destinationId, facets.Length, facets.Precision, facets.Scale,
                expression == "s.TextValue" ? "TextValue" : "Converted");
            TypedWorkspaceModelMapper.Create(Source, PathOf("Source"), "xml");
            TypedWorkspaceModelMapper.Create(target, PathOf("Target"), "xml");
            TypedWorkspaceModelMapper.Create(Catalog, PathOf("Catalog"), "xml");
            Syntax.ImportFromSqlCodeToXmlWorkspaceAsync("SELECT s.Id, " + (expression ?? "CAST(s.TextValue AS nvarchar(50)) AS Converted") +
                " FROM dbo.SourceRows AS s WHERE s.Id > 0;", "dbo.TargetRows", PathOf("Input"), "ConversionProbe").GetAwaiter().GetResult();
            var model = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(PathOf("Input"), searchUpward: false);
            projectionId = model.SelectScalarExpressionList[1].Id;
            var snippet = new SqlConversionSnippet { Id = "snippet", TransformScript = "CAST($(source) AS $(destination))" };
            Conversions.SqlConversionSnippetList.Add(snippet);
            var implementation = new ConversionImplementation { Id = "implementation", Name = "User conversion", SqlConversionSnippet = snippet };
            Conversions.ConversionImplementationList.Add(implementation);
            Conversions.DataTypeMappingList.Add(new DataTypeMapping { Id = "mapping", SourceDataTypeId = Id("nvarchar"), TargetDataTypeId = destinationId, ConversionImplementation = implementation });
            foreach (var pair in new[] { ("source", "SourceExpression"), ("destination", "DestinationType") })
            {
                var parameter = new SqlConversionParameter { Id = pair.Item1, Name = pair.Item2 };
                Conversions.SqlConversionParameterList.Add(parameter);
                Conversions.SqlConversionParameterBindingList.Add(new SqlConversionParameterBinding { Id = pair.Item1, Name = pair.Item1, SqlConversionParameter = parameter, SqlConversionSnippet = snippet });
            }
            SaveConversions();
        }

        public void SaveConversions() => TypedWorkspaceModelMapper.Create(Conversions, PathOf("Conversions"), "xml");
        public string Convert(string output)
        {
            var result = new TransformScriptConversionWorkspaceService().Apply(new(PathOf("Input"), PathOf("Catalog"), PathOf("Conversions"), PathOf("Source"),
                [new(projectionId, destinationId, facets)], PathOf(output)));
            Assert.Equal(1, result.ConvertedProjectionCount);
            return result.WorkspacePath;
        }

        public void AssertBound(string output)
        {
            var result = new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(output, [PathOf("Source")], PathOf("Target"),
                "TestSystem", "dbo", output + "-Binding", dataTypeWorkspacePath: PathOf("Catalog"));
            Assert.Equal(0, result.ErrorCount);
            Assert.Equal(2, result.Model.ValidationTargetColumnTypeExactList.Count);
            Assert.Empty(result.Model.ValidationTargetColumnTypeSanctionedConversionList);
            var model = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(output, searchUpward: false);
            var bound = new TransformBindingService().BindTransform(model, model.TransformScriptList.Single(), Source, Catalog);
            var type = bound.TopLevelRowset!.Columns[1].DataType!;
            Assert.Equal(destinationId, type.MetaDataTypeId);
            Assert.Equal(facets.Length, type.Length);
            Assert.Equal(facets.Precision, type.Precision);
            Assert.Equal(facets.Scale, type.Scale);
        }

        public string Snapshot() => string.Join("\n", Directory.GetFiles(PathOf("Input"), "*", SearchOption.AllDirectories).OrderBy(path => path)
            .Select(path => Path.GetRelativePath(Root, path) + ":" + System.Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path)))));

        public string AllInputArtifacts() => string.Join("\n", new[] { "Input", "Source", "Target", "Catalog", "Conversions" }
            .SelectMany(name => Directory.GetFiles(PathOf(name), "*", SearchOption.AllDirectories)).OrderBy(path => path)
            .Select(path => Path.GetRelativePath(Root, path) + ":" + System.Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path)))));

        public void Dispose() { if (Directory.Exists(Root)) Directory.Delete(Root, true); }

        private static MetaSchemaModel Schema(string table, string intId, string valueId, int? length, int? precision = null, int? scale = null, string name = "TextValue")
        {
            var model = MetaSchemaModel.CreateEmpty();
            var system = new MetaSchema.System { Id = "system", Name = "TestSystem" };
            var schema = new Schema { Id = "schema", Name = "dbo", System = system };
            var obj = new SchemaObject { Id = table, Name = table, Schema = schema };
            model.SystemList.Add(system); model.SchemaList.Add(schema); model.SchemaObjectList.Add(obj);
            model.TableList.Add(new Table { Id = table, SchemaObject = obj });
            model.FieldList.Add(new Field { Id = "Id", Name = "Id", Ordinal = "1", IsNullable = "false", MetaDataTypeId = intId, SchemaObject = obj });
            var field = new Field { Id = "Value", Name = name, Ordinal = "2", IsNullable = "true", MetaDataTypeId = valueId, SchemaObject = obj };
            model.FieldList.Add(field);
            foreach (var facet in new[] { ("Length", length), ("Precision", precision), ("Scale", scale) })
                if (facet.Item2.HasValue) model.FieldDataTypeDetailList.Add(new FieldDataTypeDetail { Id = facet.Item1, Field = field, Name = facet.Item1, Value = facet.Item2.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) });
            return model;
        }
    }
}
