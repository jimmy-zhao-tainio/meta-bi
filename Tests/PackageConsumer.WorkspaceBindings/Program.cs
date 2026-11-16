using Meta.Integration;
using Meta.Operations.Domain;
using MetaDataType;
using MetaDataType.Core;
using MetaSchema;
using MetaSchema.Core;
using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql;

if (args.Length != 1 || !Path.IsPathRooted(args[0]))
    throw new ArgumentException("Pass one explicit absolute path for a new evidence directory.");
var outputRoot = Path.GetFullPath(args[0]);
if (Directory.Exists(outputRoot) || File.Exists(outputRoot))
    throw new InvalidOperationException("The evidence directory must be new: " + outputRoot);
Directory.CreateDirectory(outputRoot);
var logPath = Path.Combine(outputRoot, "Result.txt");
void Log(string text)
{
    Console.WriteLine(text);
    File.AppendAllText(logPath, text + Environment.NewLine);
}
var assertions = 0;
void Require(bool condition, string description)
{
    if (!condition)
    {
        Log("FAIL " + description);
        throw new InvalidOperationException(description);
    }
    assertions++;
    Log("PASS " + description);
}
Log("Runtime root: " + AppContext.BaseDirectory);
Log("Evidence: " + outputRoot);
foreach (var name in new[] { "MetaDataType", "MetaSchema", "MetaTransformScript", "MetaTransformBinding" })
    Require(File.Exists(Path.Combine(AppContext.BaseDirectory, "Workspaces", name, "workspace.meta")), "transitive package delivers " + name);

var defaults = MetaDataTypeWorkspaceProvider.GetDefaultWorkspace();
var nextDefaults = MetaDataTypeWorkspaceProvider.GetDefaultWorkspace();
Require(!ReferenceEquals(defaults, nextDefaults), "default catalog graph is fresh");
Require(!ReferenceEquals(defaults.DataTypeList[0], nextDefaults.DataTypeList[0]), "default catalog rows are independent");
var original = nextDefaults.DataTypeList[0].Name;
defaults.DataTypeList[0].Name = "only first graph";
Require(nextDefaults.DataTypeList[0].Name == original, "mutating first default does not mutate second");
Require(MetaDataTypeWorkspaceProvider.GetDefaultWorkspace().DataTypeList[0].Name == original, "mutating default does not mutate saved package data");

var catalogState = await MetaDataTypeWorkspaceProvider.LoadDefaultStateAsync();
var catalog = MetaDataTypeModel.CreateEmpty();
var sqlSystem = new DataTypeSystem { Id = "selected/system-with-arbitrary-identity", Name = "SqlServer", Description = "" };
catalog.DataTypeSystemList.Add(sqlSystem);
catalog.DataTypeList.Add(new MetaDataType.DataType { Id = "selected/integer", Name = "int", DataTypeSystem = sqlSystem, Description = null });
catalog.DataTypeList.Add(new MetaDataType.DataType { Id = "selected/text", Name = "nvarchar", DataTypeSystem = sqlSystem, Description = " Åsa äter räksmörgås.\r\n\t " });
catalog.DataTypeList.Add(new MetaDataType.DataType { Id = "selected/ansi-text", Name = "varchar", DataTypeSystem = sqlSystem, Description = "" });

var schemaContract = MetaSchemaModels.CreateMetaSchemaModel();
var scriptContract = MetaTransformScriptModels.LoadContract();
var bindingContract = TransformBindingModels.LoadContract();
Require(schemaContract.Name == "MetaSchema", "schema contract loads from transitive package source");
Require(scriptContract.Name == "MetaTransformScript", "script contract loads from transitive package source");
Require(bindingContract.Name == "MetaTransformBinding", "binding contract loads from transitive package source");

MetaSchemaModel Schema(string tableName, string? description)
{
    var schema = MetaSchemaModel.CreateEmpty();
    var system = new MetaSchema.System { Id = "source-system", Name = "TestSystem", Description = description };
    var sqlSchema = new Schema { Id = "schema", Name = "dbo", System = system };
    var obj = new SchemaObject { Id = "object", Name = tableName, Schema = sqlSchema };
    var table = new Table { Id = "table", SchemaObject = obj };
    var id = new Field { Id = "id", Name = "Id", Ordinal = "0", MetaDataTypeId = "selected/integer", IsNullable = "false", SchemaObject = obj };
    var text = new Field { Id = "value", Name = "Value", Ordinal = "1", MetaDataTypeId = "selected/text", IsNullable = "true", SchemaObject = obj };
    schema.SystemList.Add(system);
    schema.SchemaList.Add(sqlSchema);
    schema.SchemaObjectList.Add(obj);
    schema.TableList.Add(table);
    schema.FieldList.AddRange([id, text]);
    schema.FieldDataTypeDetailList.Add(new FieldDataTypeDetail { Id = "length", Field = text, Name = "Length", Value = "50" });
    return schema;
}

var syntax = new MetaTransformScriptSqlService();
const string inputSql = "SELECT s.Id, CAST(s.Value AS nvarchar(50)) AS Value FROM dbo.Source AS s WHERE s.Value = N'Åsa äter räksmörgås.' OR s.Value = N'' OR s.Value IS NULL;";
var imported = syntax.ImportSqlCode(MetaTransformScriptModel.CreateEmpty(), inputSql, "dbo.Target", "Exact copy").Model;
var expectedSql = syntax.ExportToSqlCode(imported);
Require(expectedSql.Contains("Åsa äter räksmörgås.", StringComparison.Ordinal), "SQL emitter preserves exact Unicode literal");
Require(expectedSql.Contains("N''", StringComparison.Ordinal), "SQL emitter preserves empty literal");
Require(expectedSql.Contains("IS NULL", StringComparison.OrdinalIgnoreCase), "SQL emitter preserves NULL predicate");

foreach (var representation in new[] { "xml", "csharp" })
{
    var root = Path.Combine(outputRoot, representation);
    var catalogPath = Path.Combine(root, "catalog");
    var sourcePath = Path.Combine(root, "source");
    var targetPath = Path.Combine(root, "target");
    var scriptPath = Path.Combine(root, "script");
    var bindingPath = Path.Combine(root, "binding");
    var catalogBinding = new MetaDataTypeWorkspaceBinding();
    var expectedCatalog = catalogBinding.Write(catalog, catalogState.Model);
    await TypedWorkspaceModelMapper.CreateAsync(catalog, catalogState.Model, catalogBinding, catalogPath, representation);
    var restoredCatalog = MetaDataTypeWorkspaceProvider.LoadOrDefault(catalogPath);
    Require(restoredCatalog.DataTypeList.Single(type => type.Name == "int").Id == "selected/integer", representation + " selected arbitrary type identity survives");
    Require(restoredCatalog.DataTypeList.All(type => ReferenceEquals(restoredCatalog.DataTypeSystemList.Single(), type.DataTypeSystem)), representation + " catalog references are canonical");
    Require(InMemoryWorkspaceComparer.FindDifference(expectedCatalog, catalogBinding.Write(restoredCatalog, catalogState.Model)) is null,
        representation + " entire selected catalog graph is exact");
    Require(restoredCatalog.DataTypeList.Single(type => type.Name == "int").Description is null, representation + " optional NULL remains missing");
    Require(restoredCatalog.DataTypeList.Single(type => type.Name == "varchar").Description == "", representation + " empty remains distinct from NULL");
    Require(restoredCatalog.DataTypeList.Single(type => type.Name == "nvarchar").Description == " Åsa äter räksmörgås.\r\n\t ", representation + " Unicode/whitespace exact");
    var schemaBinding = new MetaSchemaWorkspaceBinding();
    var sourceModel = Schema("Source", null);
    var targetModel = Schema("Target", "");
    await TypedWorkspaceModelMapper.CreateAsync(sourceModel, schemaContract, schemaBinding, sourcePath, representation);
    await TypedWorkspaceModelMapper.CreateAsync(targetModel, schemaContract, schemaBinding, targetPath, representation);
    await TypedWorkspaceModelMapper.CreateAsync(imported, scriptContract, new MetaTransformScriptWorkspaceBinding(), scriptPath, representation);
    var script = await MetaTransformScriptInstance.LoadFromWorkspaceAsync(scriptPath);
    Require(syntax.ExportToSqlCode(script) == expectedSql, representation + " script round-trip exports exact SQL");
    var scriptState = await TypedWorkspaceModelMapper.LoadStateAsync(scriptPath);
    Require(InMemoryWorkspaceComparer.FindDifference(scriptState, new MetaTransformScriptWorkspaceBinding().Write(script, scriptState.Model)) is null, representation + " script graph is exact");
    var schema = TypedWorkspaceModelMapper.Load(sourcePath, schemaBinding);
    var restoredTarget = TypedWorkspaceModelMapper.Load(targetPath, schemaBinding);
    Require(InMemoryWorkspaceComparer.FindDifference(schemaBinding.Write(sourceModel, schemaContract), schemaBinding.Write(schema, schemaContract)) is null,
        representation + " entire source schema graph is exact");
    Require(InMemoryWorkspaceComparer.FindDifference(schemaBinding.Write(targetModel, schemaContract), schemaBinding.Write(restoredTarget, schemaContract)) is null,
        representation + " entire target schema graph is exact");
    Require(schema.SystemList.Single().Description is null && restoredTarget.SystemList.Single().Description == "",
        representation + " schema NULL and empty descriptions remain distinct");
    Require(schema.FieldList.All(field => ReferenceEquals(field.SchemaObject, schema.SchemaObjectList.Single())), representation + " schema object references are canonical");
    Require(ReferenceEquals(schema.FieldDataTypeDetailList.Single().Field, schema.FieldList.Single(field => field.Name == "Value")), representation + " facet field references are canonical");
    Require(schema.FieldDataTypeDetailList.Single().Value == "50", representation + " field length is 50");
    var expressionEvidence = new TransformBindingService().BindTransform(script, script.TransformScriptList.Single(), schema, restoredCatalog);
    var textResult = expressionEvidence.TopLevelRowset!.Columns.Single(column => column.Name == "Value").DataType!;
    Require(textResult.MetaDataTypeId == "selected/text", representation + " CAST resolves selected arbitrary text identity");
    Require(textResult.Length == 50, representation + " resulting CAST length is exactly 50");
    Require(textResult.IsNullable == true, representation + " resulting CAST preserves nullable source");
    Require(textResult.Precision is null && textResult.Scale is null, representation + " resulting CAST retains unknown precision/scale");
    var result = new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(scriptPath, [sourcePath], targetPath,
        "TestSystem", "dbo", bindingPath, dataTypeWorkspacePath: catalogPath);
    Require(result.ErrorCount == 0, representation + " production schema-aware Binding succeeds");
    Require(result.Model.ValidationTargetColumnTypeExactList.Count == 2, representation + " arbitrary int and CAST text receive exact assessments");
    Require(result.Model.ValidationTargetColumnTypeExactList.All(exact => exact.SourceMetaDataTypeId == exact.TargetMetaDataTypeId && exact.SourceMetaDataTypeId.StartsWith("selected/", StringComparison.Ordinal)), representation + " exact assessments retain selected catalog identities");
    var savedBinding = await TypedWorkspaceModelMapper.LoadStateAsync(bindingPath);
    var binding = new MetaTransformBindingWorkspaceBinding();
    Require(InMemoryWorkspaceComparer.FindDifference(savedBinding, binding.Write(binding.Read(savedBinding), bindingContract)) is null,
        representation + " resulting Binding graph round-trips exactly");
    if (representation == "csharp")
    {
        var csharpBindingPath = Path.Combine(root, "binding-csharp");
        await WorkspaceSurface.CreateAsync(savedBinding, csharpBindingPath, "csharp");
        var restoredBinding = await TypedWorkspaceModelMapper.LoadStateAsync(csharpBindingPath);
        Require(InMemoryWorkspaceComparer.FindDifference(savedBinding, restoredBinding) is null,
            "resulting Binding graph persists/reopens exactly through C# surface");
    }
}
Log($"SUCCESS: {assertions} assertions; one direct NuGet package reference; no repository project references; no SQL database execution.");
