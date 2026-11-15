using Meta.Integration;
using MetaBi.Tests.Common;
using MetaConvert.DataWarehouseToSql;
using MetaDataType;
using MetaDataType.Core;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;
using MetaSql;

namespace MetaDataWarehouse.Tests;

public sealed class SelectedConversionWorkspaceTests
{
    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task WarehouseWeaveUsesSelectedCatalogAndMappingWithArbitraryPhysicalIds(string representation)
    {
        var directory = Path.Combine(Path.GetTempPath(), "selected-warehouse-types", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var catalogState = await MetaDataTypeWorkspaceProvider.LoadDefaultStateAsync();
            var catalogBinding = new MetaDataTypeWorkspaceBinding();
            var catalog = catalogBinding.Read(catalogState);
            var physicalSystem = catalog.DataTypeSystemList.Single(row => row.Name == "SqlServer");
            physicalSystem.Id = "selected:system:sql";
            catalog.DataTypeSystemList.Single(row => row.Name == "Meta").Id = "selected:system:business";
            const string selectedTargetId = "selected:physical:text";
            catalog.DataTypeList.Add(new DataType
            {
                Id = selectedTargetId, Name = "varchar", Category = "Text", DataTypeSystem = physicalSystem
            });
            var mappingState = await MetaDataTypeConversionWorkspaceProvider.LoadDefaultStateAsync();
            var mappingBinding = new MetaDataTypeConversionWorkspaceBinding();
            var mappings = mappingBinding.Read(mappingState);
            mappings.ConversionImplementationList.Single(row => row.Name == "Direct").Id = "selected:implementation:direct";
            mappings.DataTypeMappingList.Single(row => row.SourceDataTypeId == "meta:type:String" &&
                row.TargetDataTypeId == "sqlserver:type:nvarchar").TargetDataTypeId = selectedTargetId;
            var catalogPath = Path.Combine(directory, "catalog");
            var mappingPath = Path.Combine(directory, "mappings");
            await WorkspaceSurface.CreateAsync(catalogBinding.Write(catalog, catalogState.Model), catalogPath, representation);
            await WorkspaceSurface.CreateAsync(mappingBinding.Write(mappings, mappingState.Model), mappingPath, representation);

            var repository = CliTestRunner.FindRepositoryRoot();
            var output = await DataWarehouseToSqlConverter.ConvertAsync(
                Path.Combine(repository, "MetaDataWarehouse", "Workspaces", "SampleDataWarehouseCommerce"),
                Path.Combine(repository, "MetaDataWarehouse", "Workspaces", "MetaDataWarehouseImplementation"),
                "SelectedWarehouse",
                dataTypeWorkspacePath: catalogPath,
                dataTypeConversionWorkspacePath: mappingPath);

            var sql = new MetaSqlWorkspaceBinding().Read(output);
            var column = sql.TableColumnList.Single(row => row.Table.Name == "Dim_Customer" && row.Name == "CustomerNumber");
            Assert.Equal(selectedTargetId, column.MetaDataTypeId);
            Assert.Contains(sql.TableColumnDataTypeDetailList,
                row => ReferenceEquals(row.TableColumn, column) && row.Name == "Length" && row.Value == "256");
            Assert.Equal("SelectedWarehouse", Assert.Single(sql.DatabaseList).Name);
            Assert.Same(sql.TableList.Single(row => row.Name == "Dim_Customer"), column.Table);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
