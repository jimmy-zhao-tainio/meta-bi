using Meta.Integration;
using Meta.Operations.Domain;
using Meta.TypedModels;
using MetaDataType.Core;
using MetaDataTypeConversion.Core;
using MetaDataVault.Core;
using MetaDataWarehouse.Core;
using MetaAnalytics.Core;
using MetaTabular.Core;
using MetaMultiDimensional.Core;
using MetaOrchestration.Core;
using MetaDataQuality.Core;
using MetaTransformPattern.Core;
using MetaTransformPatternInstance.Core;
using MetaTransform.Binding;
using MetaTransformScript.Sql;

if (args.Length != 1 || !Path.IsPathRooted(args[0]))
    throw new ArgumentException("Pass one explicit absolute path for a new evidence directory.");
var evidence = Path.GetFullPath(args[0]);
if (Directory.Exists(evidence) || File.Exists(evidence))
    throw new InvalidOperationException("Evidence directory must be new: " + evidence);
Directory.CreateDirectory(evidence);
var assertions = 0;
void Log(string line)
{
    Console.WriteLine(line);
    File.AppendAllText(Path.Combine(evidence, "Result.txt"), line + Environment.NewLine);
}
void Require(bool condition, string description)
{
    Log((condition ? "PASS " : "FAIL ") + description);
    if (!condition) throw new InvalidOperationException(description);
    assertions++;
}
void EqualState(InMemoryWorkspace expected, InMemoryWorkspace actual, string description)
{
    var difference = InMemoryWorkspaceComparer.FindDifference(expected, actual);
    Require(difference is null, description + (difference is null ? "" : ": " + difference));
}
string PackagedPath(string name) => Path.Combine(AppContext.BaseDirectory, "Workspaces", name);
async Task Probe<T>(string name, Func<GenericModel> loadContract, ITypedWorkspaceBinding<T> binding)
    where T : class
{
    var path = PackagedPath(name);
    Require(File.Exists(Path.Combine(path, "workspace.meta")), name + " transitive workspace descriptor delivered");
    Require(Directory.EnumerateFiles(path, "*.meta.cs", SearchOption.AllDirectories).Any(), name + " authored C# metadata delivered");
    Require(!Directory.EnumerateFiles(path, "*.bindings.cs", SearchOption.AllDirectories).Any() &&
        !Directory.EnumerateFiles(path, "*.access.cs", SearchOption.AllDirectories).Any(), name + " derived bindings excluded from authored source set");
    var state = await TypedWorkspaceModelMapper.LoadStateAsync(path);
    var contract = loadContract();
    Require(contract.Name == name && contract.Entities.Count > 0, name + " public contract loader reads actual packaged metadata");
    Require(contract.ComputeContractSignature() == state.Model.ComputeContractSignature(), name + " public and neutral contracts agree exactly");
    var first = binding.Read(state);
    var second = binding.Read(state);
    Require(!ReferenceEquals(first, second), name + " explicit binding creates independent roots");
    EqualState(state, binding.Write(first, contract), name + " exact generic -> typed -> generic equality");
    EqualState(state, binding.Write(second, contract), name + " independent graph preserves selected default data");
    Log($"MODEL {name}: entities={contract.Entities.Count}; authored rows={state.Instance.RecordsByEntity.Sum(pair => pair.Value.Count)}");
}

Log("Runtime root: " + AppContext.BaseDirectory);
Log("Evidence: " + evidence);
await Probe("MetaDataType", MetaDataTypeWorkspaceProvider.LoadContract, new MetaDataType.MetaDataTypeWorkspaceBinding());
await Probe("MetaDataTypeConversion", MetaDataTypeConversionWorkspaceProvider.LoadContract, new MetaDataTypeConversion.MetaDataTypeConversionWorkspaceBinding());
await Probe("MetaSchema", MetaSchema.Core.MetaSchemaModels.CreateMetaSchemaModel, new MetaSchema.MetaSchemaWorkspaceBinding());
await Probe("MetaRawDataVault", RawDataVaultModels.LoadContract, new MetaRawDataVault.MetaRawDataVaultWorkspaceBinding());
await Probe("MetaBusinessDataVault", BusinessDataVaultModels.LoadContract, new MetaBusinessDataVault.MetaBusinessDataVaultWorkspaceBinding());
await Probe("MetaDataVaultImplementation", DataVaultImplementationModels.LoadContract, new MetaDataVaultImplementation.MetaDataVaultImplementationWorkspaceBinding());
await Probe("MetaDataWarehouse", DataWarehouseModels.LoadContract, new MetaDataWarehouse.MetaDataWarehouseWorkspaceBinding());
await Probe("MetaDataWarehouseImplementation", DataWarehouseImplementationModels.LoadContract, new MetaDataWarehouseImplementation.MetaDataWarehouseImplementationWorkspaceBinding());
await Probe("MetaAnalytics", AnalyticsModels.LoadContract, new MetaAnalytics.MetaAnalyticsWorkspaceBinding());
await Probe("MetaTabular", TabularModels.LoadContract, new MetaTabular.MetaTabularWorkspaceBinding());
await Probe("MetaMultiDimensional", MultiDimensionalModels.LoadContract, new MetaMultiDimensional.MetaMultiDimensionalWorkspaceBinding());
await Probe("MetaPipeline", MetaPipeline.MetaPipelineModels.LoadContract, new MetaPipeline.MetaPipelineWorkspaceBinding());
await Probe("MetaOrchestration", MetaOrchestrationModels.LoadContract, new MetaOrchestration.MetaOrchestrationWorkspaceBinding());
await Probe("MetaDataQuality", MetaDataQualityModels.LoadContract, new MetaDataQuality.MetaDataQualityWorkspaceBinding());
await Probe("MetaTransformPattern", MetaTransformPatternModels.LoadContract, new MetaTransformPattern.MetaTransformPatternWorkspaceBinding());
await Probe("MetaTransformPatternInstance", MetaTransformPatternInstanceModels.LoadContract, new MetaTransformPatternInstance.MetaTransformPatternInstanceWorkspaceBinding());
await Probe("MetaSqlScript", MetaSqlScript.MetaSqlScriptModels.LoadContract, new MetaSqlScript.MetaSqlScriptWorkspaceBinding());
// MetaSql.CSharp is intentionally lightweight. Generic interpretation belongs to Meta,
// rather than adding an Integration dependency to that model assembly.
GenericModel NeutralContract(string name) => TypedWorkspaceModelMapper.LoadStateAsync(PackagedPath(name)).GetAwaiter().GetResult().Model;
await Probe("MetaSql", () => NeutralContract("MetaSql"), new MetaSql.MetaSqlWorkspaceBinding());
await Probe("MetaSqlDeployManifest", () => NeutralContract("MetaSqlDeployManifest"), new MetaSqlDeployManifest.MetaSqlDeployManifestWorkspaceBinding());
await Probe("MetaTransformScript", MetaTransformScriptModels.LoadContract, new MetaTransformScript.MetaTransformScriptWorkspaceBinding());
await Probe("MetaTransformBinding", TransformBindingModels.LoadContract, new MetaTransformBinding.MetaTransformBindingWorkspaceBinding());

var defaultTypes = MetaDataTypeWorkspaceProvider.GetDefaultWorkspace();
var nextTypes = MetaDataTypeWorkspaceProvider.GetDefaultWorkspace();
Require(defaultTypes.DataTypeList.Count > 0 && defaultTypes.DataTypeSystemList.Count > 0, "default catalog contains actual authored instance data");
var firstType = defaultTypes.DataTypeList[0];
Require(ReferenceEquals(firstType.DataTypeSystem, defaultTypes.DataTypeSystemList.Single(row => row.Id == firstType.DataTypeSystem.Id)), "default catalog references are canonical");
Require(!ReferenceEquals(firstType, nextTypes.DataTypeList[0]), "default catalog loads independent row graphs");
var originalTypeName = nextTypes.DataTypeList[0].Name;
firstType.Name = "mutation confined to first load";
Require(nextTypes.DataTypeList[0].Name == originalTypeName && MetaDataTypeWorkspaceProvider.GetDefaultWorkspace().DataTypeList[0].Name == originalTypeName, "default catalog mutation cannot replace packaged data");
var defaultConversions = MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace();
var nextConversions = MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace();
Require(defaultConversions.DataTypeMappingList.Count > 0, "default conversions contain actual authored instance data");
var firstMapping = defaultConversions.DataTypeMappingList[0];
Require(ReferenceEquals(firstMapping.ConversionImplementation, defaultConversions.ConversionImplementationList.Single(row => row.Id == firstMapping.ConversionImplementation.Id)), "default conversion implementation reference is canonical");
Require(!ReferenceEquals(firstMapping, nextConversions.DataTypeMappingList[0]), "default conversions load independent row graphs");
Require(!new MetaDataTypeConversionService(nextTypes).Check(defaultConversions).HasErrors, "real conversion consumer accepts packaged selected catalog and mappings");

const string unicode = " Åsa äter räksmörgås.\r\n\t ";
var graph = new MetaAnalytics.MetaAnalyticsModel();
var root = new MetaAnalytics.AnalyticsModel { Id = "opaque/root", Name = unicode, DefaultCulture = "", Description = null };
var table = new MetaAnalytics.Table { Id = "opaque/table", Name = "Text", Kind = "Dimension", Description = "", DisplayFolder = "\r\n\t ", AnalyticsModel = root };
var attribute = new MetaAnalytics.Attribute { Id = "opaque/attribute", Name = "Value", DataTypeId = "opaque/type", Ordinal = "0", Description = unicode, FormatString = "", SourceName = null, Table = table };
var source = new MetaAnalytics.DataSource { Id = "opaque/source", Name = "Local", Description = "", Provider = unicode, ConnectionReference = null, AnalyticsModel = root };
graph.AnalyticsModelList.Add(root);
graph.TableList.Add(table);
graph.AttributeList.Add(attribute);
graph.DataSourceList.Add(source);
var analyticsBinding = new MetaAnalytics.MetaAnalyticsWorkspaceBinding();
var analyticsContract = AnalyticsModels.LoadContract();
var expected = analyticsBinding.Write(graph, analyticsContract);
foreach (var representation in new[] { "xml", "csharp" })
{
    var path = Path.Combine(evidence, "analytics-" + representation);
    await WorkspaceSurface.CreateAsync(expected, path, representation);
    var persisted = await TypedWorkspaceModelMapper.LoadStateAsync(path);
    EqualState(expected, persisted, representation + " persistence preserves exact contract, values and relationships");
    var first = analyticsBinding.Read(persisted);
    var second = analyticsBinding.Read(persisted);
    Require(ReferenceEquals(first.TableList.Single().AnalyticsModel, first.AnalyticsModelList.Single()) &&
        ReferenceEquals(first.AttributeList.Single().Table, first.TableList.Single()) &&
        ReferenceEquals(first.DataSourceList.Single().AnalyticsModel, first.AnalyticsModelList.Single()), representation + " loaded graph uses canonical references");
    Require(first.AnalyticsModelList.Single().Name == unicode && first.AttributeList.Single().Description == unicode &&
        first.DataSourceList.Single().Provider == unicode && first.TableList.Single().DisplayFolder == "\r\n\t ", representation + " Unicode and whitespace survive exactly");
    Require(first.AnalyticsModelList.Single().Description is null && first.AttributeList.Single().SourceName is null &&
        first.DataSourceList.Single().ConnectionReference is null, representation + " NULL survives exactly");
    Require(first.AnalyticsModelList.Single().DefaultCulture == "" && first.TableList.Single().Description == "" &&
        first.AttributeList.Single().FormatString == "" && first.DataSourceList.Single().Description == "", representation + " empty remains distinct from NULL");
    Require(!ReferenceEquals(first.TableList.Single(), second.TableList.Single()) &&
        !ReferenceEquals(first.AnalyticsModelList.Single(), second.AnalyticsModelList.Single()), representation + " independent reads create independent row graphs");
    EqualState(persisted, analyticsBinding.Write(first, persisted.Model), representation + " saved state projects exactly through explicit binding");
    first.TableList.Single().Name = "changed only in first graph";
    Require(second.TableList.Single().Name == "Text", representation + " typed mutation does not affect another projection");
    EqualState(expected, await TypedWorkspaceModelMapper.LoadStateAsync(path), representation + " typed mutation does not affect persisted authority");
}
Log($"PASS: {assertions} assertions; 21 packaged models; build/publish runs have separate evidence directories.");
