using Meta.Integration;
using Meta.Operations.Domain;
using Meta.Surfaces;
using Meta.TypedModels;
using MetaBi.Tests.Common;
using MetaConvert.SchemaToDataVault;
using MetaRawDataVault;
using System.Text.Json;
using CSharpReference = MetaConvert.SchemaToDataVault.Reference.RawDataVaultFromMetaSchemaService;
using MS = MetaSchema;

namespace MetaDataVault.Tests;

public sealed class SchemaToRawDataVaultWeaveTests
{
    [Fact]
    public async Task SanctionedWeave_PreservesAdventureWorksOutputExactly()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var sourcePath = Path.Combine(
            repoRoot,
            "Demos",
            "AdventureWorksBiStackDemo",
            "Runs",
            "source",
            "AdventureWorks2022",
            "Schema");
        var expectedPath = Path.Combine(
            repoRoot,
            "Demos",
            "AdventureWorksBiStackDemo",
            "Runs",
            "rdv",
            "RawDataVault");
        var source = await TypedWorkspaceModelMapper.LoadAsync<MS.MetaSchemaModel>(
            sourcePath,
            searchUpward: false);
        var expectedModel = await TypedWorkspaceModelMapper.LoadAsync<MetaRawDataVaultModel>(
            expectedPath,
            searchUpward: false);
        var expected = TypedWorkspaceModelMapper.ToInMemoryWorkspace(expectedModel);

        var converted = new RawDataVaultFromMetaSchemaService().Materialize(source);
        var actual = TypedWorkspaceModelMapper.ToInMemoryWorkspace(converted);

        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, actual));
    }

    [Fact]
    public void SanctionedWeave_MatchesReferenceForTypedOptionsIncludingTrailingSpaces()
    {
        var source = CreateOptionsWitness();
        var ignoredNames = new[] { "Ignore Exactly", "ignore exactly", " " };
        var ignoredSuffixes = new[] { "Tail ", "tail ", string.Empty };
        var expected = new CSharpReference().MaterializeWithReport(
            source,
            ignoredNames,
            ignoredSuffixes,
            includeViews: true);

        var actual = new RawDataVaultFromMetaSchemaService().MaterializeWithReport(
            source,
            ignoredNames,
            ignoredSuffixes,
            includeViews: true);

        Assert.Null(InMemoryWorkspaceComparer.FindDifference(
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(expected.Model),
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(actual.Model)));
        Assert.Equal(
            JsonSerializer.Serialize(expected.Report),
            JsonSerializer.Serialize(actual.Report));
        Assert.Contains(actual.Model.FieldList, field => field.Name == "Résumé View Field");
        Assert.DoesNotContain(actual.Model.RawHubSatelliteAttributeList, attribute => attribute.Name == "Ignore Exactly");
        Assert.DoesNotContain(actual.Model.RawHubSatelliteAttributeList, attribute => attribute.Name == "Ends With Tail ");
    }

    [Fact]
    public void LinkNaming_PreservesSourceTextAndIsIndependentOfRelationshipOrder()
    {
        var forward = new RawDataVaultFromMetaSchemaService().Materialize(
            CreateNamingWitness(reverseRelationships: false));
        var reversed = new RawDataVaultFromMetaSchemaService().Materialize(
            CreateNamingWitness(reverseRelationships: true));

        Assert.Null(InMemoryWorkspaceComparer.FindDifference(
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(forward),
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(reversed)));
        Assert.Equal(
            [
                "Søurce SetTärgét-Set_Code--Å_relationship:a",
                "Søurce SetTärgét-Set_Code--Å_relationship:b",
                "Søurce SetTärgét-Set_Region  Name",
            ],
            forward.RawLinkList
                .OrderBy(link => link.Id, StringComparer.Ordinal)
                .Select(link => link.Name)
                .ToArray());
    }

    private static MS.MetaSchemaModel CreateOptionsWitness()
    {
        var model = CreateBaseModel(out var schema);
        var table = AddTable(model, schema, "table:options", "Options Table");
        var key = AddField(model, table.SchemaObject, "field:key", "Business Key", "1");
        AddField(model, table.SchemaObject, "field:keep", "Keep Me", "2");
        AddField(model, table.SchemaObject, "field:ignore", "Ignore Exactly", "3");
        AddField(model, table.SchemaObject, "field:suffix", "Ends With Tail ", "4");
        AddPrimaryKey(model, table, "key:options", key);

        var viewObject = new MS.SchemaObject
        {
            Id = "view:resume",
            Name = "Résumé View",
            Schema = schema,
        };
        model.SchemaObjectList.Add(viewObject);
        model.ViewList.Add(new MS.View { Id = "view-record:resume", SchemaObject = viewObject });
        AddField(model, viewObject, "field:view", "Résumé View Field", "1");
        return model;
    }

    private static MS.MetaSchemaModel CreateNamingWitness(bool reverseRelationships)
    {
        var model = CreateBaseModel(out var schema);
        var sourceTable = AddTable(model, schema, "table:source", "Søurce Set");
        var targetTable = AddTable(model, schema, "table:target", "Tärgét-Set");
        var sourceKey = AddField(model, sourceTable.SchemaObject, "field:source-key", "Source Key", "1");
        var duplicateName = AddField(model, sourceTable.SchemaObject, "field:duplicate", "Code--Å", "2");
        var spacedName = AddField(model, sourceTable.SchemaObject, "field:spaced", "Region  Name", "3");
        var targetKey = AddField(model, targetTable.SchemaObject, "field:target-key", "Target Key", "1");
        AddPrimaryKey(model, sourceTable, "key:source", sourceKey);
        AddPrimaryKey(model, targetTable, "key:target", targetKey);

        var relationships = new[]
        {
            CreateRelationship("relationship:a", "First relationship", sourceTable, targetTable, duplicateName, targetKey),
            CreateRelationship("relationship:b", "Second.relationship", sourceTable, targetTable, duplicateName, targetKey),
            CreateRelationship("relationship:c", "Third relationship", sourceTable, targetTable, spacedName, targetKey),
        };
        foreach (var pair in reverseRelationships ? relationships.Reverse() : relationships)
        {
            model.TableRelationshipList.Add(pair.Relationship);
            model.TableRelationshipFieldList.Add(pair.Field);
        }

        return model;
    }

    private static MS.MetaSchemaModel CreateBaseModel(out MS.Schema schema)
    {
        var model = MS.MetaSchemaModel.CreateEmpty();
        var system = new MS.System { Id = "system:test", Name = "Test System" };
        schema = new MS.Schema { Id = "schema:test", Name = "dbo", System = system };
        model.SystemList.Add(system);
        model.SchemaList.Add(schema);
        return model;
    }

    private static MS.Table AddTable(
        MS.MetaSchemaModel model,
        MS.Schema schema,
        string id,
        string name)
    {
        var schemaObject = new MS.SchemaObject { Id = id, Name = name, Schema = schema };
        var table = new MS.Table { Id = id + ":record", SchemaObject = schemaObject };
        model.SchemaObjectList.Add(schemaObject);
        model.TableList.Add(table);
        return table;
    }

    private static MS.Field AddField(
        MS.MetaSchemaModel model,
        MS.SchemaObject table,
        string id,
        string name,
        string ordinal)
    {
        var field = new MS.Field
        {
            Id = id,
            Name = name,
            Ordinal = ordinal,
            MetaDataTypeId = "meta:type:String",
            SchemaObject = table,
        };
        model.FieldList.Add(field);
        return field;
    }

    private static void AddPrimaryKey(
        MS.MetaSchemaModel model,
        MS.Table table,
        string id,
        MS.Field field)
    {
        var key = new MS.Key { Id = id, Name = id, Table = table };
        model.KeyList.Add(key);
        model.KeyFieldList.Add(new MS.KeyField
        {
            Id = id + ":field",
            Ordinal = "1",
            Key = key,
            Field = field,
        });
        model.PrimaryKeyList.Add(new MS.PrimaryKey { Id = id + ":primary", Key = key });
    }

    private static (MS.TableRelationship Relationship, MS.TableRelationshipField Field) CreateRelationship(
        string id,
        string name,
        MS.Table sourceTable,
        MS.Table targetTable,
        MS.Field sourceField,
        MS.Field targetField)
    {
        var relationship = new MS.TableRelationship
        {
            Id = id,
            Name = name,
            SourceTable = sourceTable,
            TargetTable = targetTable,
        };
        return (
            relationship,
            new MS.TableRelationshipField
            {
                Id = id + ":field",
                Ordinal = "1",
                TableRelationship = relationship,
                SourceField = sourceField,
                TargetField = targetField,
            });
    }
}
