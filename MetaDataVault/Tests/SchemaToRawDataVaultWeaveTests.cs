using Meta.Integration;
using Meta.Operations.Domain;
using Meta.Surfaces;
using Meta.TypedModels;
using MetaBi.Tests.Common;
using MetaConvert.SchemaToDataVault;
using MetaRawDataVault;
using MetaWeaveScript.Execution;
using System.Reflection;
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
        var expectedReport = expected.Report with
        {
            Tables = expected.Report.Tables
                .Select(table => table.Reason == "no source primary or unique key metadata was available"
                    ? table with { Reason = "no source key metadata was available" }
                    : table)
                .ToList()
        };
        Assert.Equal(
            JsonSerializer.Serialize(expectedReport),
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

    [Fact]
    public void ReportIdentifiesAnOrdinaryModeledKeyFromWeaveEvidence()
    {
        var model = CreateBaseModel(out var schema);
        var table = AddTable(model, schema, "table:ordinary-key", "Ordinary Key Table");
        var secondField = AddField(model, table.SchemaObject, "field:second", "Second Part", "2");
        var firstField = AddField(model, table.SchemaObject, "field:first", "First Part", "1");
        AddOrdinaryKey(
            model,
            table,
            "key:ordinary",
            "Fallback Key",
            (secondField, "2"),
            (firstField, "1"));

        var result = new RawDataVaultFromMetaSchemaService().MaterializeWithReport(model);

        var tableReport = Assert.Single(result.Report.Tables);
        Assert.True(tableReport.HubCreated);
        var selectedKey = Assert.IsType<RawDataVaultFromMetaSchemaSelectedKeyReport>(tableReport.SelectedKey);
        Assert.Equal("other", selectedKey.KeyType);
        Assert.Equal("Fallback Key", selectedKey.KeyName);
        Assert.Equal(["First Part", "Second Part"], selectedKey.FieldNames);
    }

    [Fact]
    public void ReportIdentifiesATableWithoutModeledKeyMetadata()
    {
        var model = CreateBaseModel(out var schema);
        var table = AddTable(model, schema, "table:no-key", "No Key Table");
        AddField(model, table.SchemaObject, "field:value", "Value", "1");

        var result = new RawDataVaultFromMetaSchemaService().MaterializeWithReport(model);

        var tableReport = Assert.Single(result.Report.Tables);
        Assert.False(tableReport.HubCreated);
        Assert.Null(tableReport.SelectedKey);
        Assert.Equal("no source key metadata was available", tableReport.Reason);
    }

    [Fact]
    public void ReportIdentifiesAModeledKeyWithoutKeyFields()
    {
        var model = CreateBaseModel(out var schema);
        var table = AddTable(model, schema, "table:empty-key", "Empty Key Table");
        AddField(model, table.SchemaObject, "field:value", "Value", "1");
        AddOrdinaryKey(model, table, "key:empty", "Empty Key");

        var result = new RawDataVaultFromMetaSchemaService().MaterializeWithReport(model);

        var tableReport = Assert.Single(result.Report.Tables);
        Assert.False(tableReport.HubCreated);
        Assert.Null(tableReport.SelectedKey);
        Assert.Equal("source key metadata contained no key fields", tableReport.Reason);
    }

    [Fact]
    public void ReportIdentifiesKeyFieldsExcludedByExplicitOptions()
    {
        var model = CreateBaseModel(out var schema);
        var table = AddTable(model, schema, "table:ignored-key", "Ignored Key Table");
        var keyField = AddField(model, table.SchemaObject, "field:ignored-key", "Ignored Key", "1");
        AddPrimaryKey(model, table, "key:ignored", keyField);

        var result = new RawDataVaultFromMetaSchemaService().MaterializeWithReport(
            model,
            ignoredFieldNames: ["Ignored Key"]);

        var tableReport = Assert.Single(result.Report.Tables);
        Assert.False(tableReport.HubCreated);
        Assert.Null(tableReport.SelectedKey);
        Assert.Equal(
            "all source key fields were excluded by explicit ignore options",
            tableReport.Reason);
    }

    [Fact]
    public void EvidenceReaderRejectsOrphanSelectedKeyFields()
    {
        var relationOutputs = CreateEvidenceOutputs(
            selectedKeyRows: [],
            selectedKeyFieldRows:
            [
                Row("table:test", "key:orphan", "key-field:orphan", "Orphan Field", 1),
            ],
            assessment: "no-modeled-key");

        AssertEvidenceReadFails(relationOutputs, "without a corresponding 'SelectedKeys' row");
    }

    [Fact]
    public void EvidenceReaderRejectsSelectedKeyFieldFromAnotherKey()
    {
        var relationOutputs = CreateEvidenceOutputs(
            selectedKeyRows:
            [
                Row("key:selected", "Selected Key", "table:test", 0),
            ],
            selectedKeyFieldRows:
            [
                Row("table:test", "key:other", "key-field:other", "Other Field", 1),
            ],
            assessment: "selected");

        AssertEvidenceReadFails(relationOutputs, "but 'SelectedKeys' selected key 'key:selected'");
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

    private static void AddOrdinaryKey(
        MS.MetaSchemaModel model,
        MS.Table table,
        string id,
        string name,
        params (MS.Field Field, string Ordinal)[] fields)
    {
        var key = new MS.Key { Id = id, Name = name, Table = table };
        model.KeyList.Add(key);
        for (var index = 0; index < fields.Length; index++)
        {
            model.KeyFieldList.Add(new MS.KeyField
            {
                Id = $"{id}:field:{index + 1}",
                Ordinal = fields[index].Ordinal,
                Key = key,
                Field = fields[index].Field,
            });
        }
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

    private static IReadOnlyDictionary<string, MetaWeaveScriptQueryOutput> CreateEvidenceOutputs(
        IReadOnlyList<MetaWeaveScriptQueryRow> selectedKeyRows,
        IReadOnlyList<MetaWeaveScriptQueryRow> selectedKeyFieldRows,
        string assessment)
        => new Dictionary<string, MetaWeaveScriptQueryOutput>(StringComparer.OrdinalIgnoreCase)
        {
            ["IncludedTables"] = Relation(
                ["TableId", "TableName", "SchemaId", "SchemaName", "SystemId"],
                Row("table:test", "Test Table", "schema:test", "dbo", "system:test")),
            ["IncludedRelationships"] = Relation(
                [
                    "RelationshipId",
                    "SourceTableId",
                    "SourceTableName",
                    "TargetTableId",
                    "TargetTableName",
                    "StructuralName",
                ]),
            ["SelectedKeys"] = Relation(
                ["KeyId", "KeyName", "TableId", "KeyPriority"],
                [.. selectedKeyRows]),
            ["SelectedKeyFields"] = Relation(
                ["TableId", "KeyId", "KeyFieldId", "FieldName", "KeyFieldNumber"],
                [.. selectedKeyFieldRows]),
            ["KeyAssessments"] = Relation(
                ["TableId", "KeyAssessment"],
                Row("table:test", assessment)),
        };

    private static MetaWeaveScriptQueryOutput Relation(
        IReadOnlyList<string> columns,
        params MetaWeaveScriptQueryRow[] rows)
        => new(
            columns.Select(column => new MetaWeaveScriptQueryColumn(column)).ToList(),
            rows);

    private static MetaWeaveScriptQueryRow Row(params object[] values)
        => new(values.Select(value => value switch
        {
            string text => MetaWeaveScriptValue.FromString(text),
            int number => MetaWeaveScriptValue.FromInteger(number),
            _ => throw new ArgumentOutOfRangeException(nameof(values), value, null),
        }).ToList());

    private static void AssertEvidenceReadFails(
        IReadOnlyDictionary<string, MetaWeaveScriptQueryOutput> relationOutputs,
        string expectedMessage)
    {
        var readEvidence = typeof(RawDataVaultFromMetaSchemaService).GetMethod(
            "ReadEvidence",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(readEvidence);

        var invocation = Assert.Throws<TargetInvocationException>(() =>
            readEvidence.Invoke(null, [relationOutputs]));
        var failure = Assert.IsType<InvalidOperationException>(invocation.InnerException);
        Assert.Contains(expectedMessage, failure.Message, StringComparison.Ordinal);
    }
}
