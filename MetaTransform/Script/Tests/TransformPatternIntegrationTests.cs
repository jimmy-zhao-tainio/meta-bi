using MetaConvert.TransformPatternToSqlScript;
using MetaTransformPattern.Core;
using MetaTransformPatternInstance.Core;
using MetaTransformScript.Sql;
using MSS = MetaSqlScript;
using MTP = MetaTransformPattern;
using MTPI = MetaTransformPatternInstance;
using MTS = MetaTransformScript;

public sealed class TransformPatternIntegrationTests
{
    private const string PatternText =
        "INSERT INTO $(target) ($(target-fields)) SELECT $(source-expressions) FROM $(source);";
    private const string ExpectedSql =
        "INSERT INTO [dbo].[Customer] ([CustomerId], [Name]) SELECT [s].[CustomerId], [s].[Name] FROM [stage].[Customer] AS [s];";

    [Fact]
    public void PatternAndInstance_MaterializeSqlScript_AndParseToTransformScript()
    {
        var (patterns, instances) = CreateInsertPattern();

        var scripts = TransformPatternToSqlScriptConverter.Convert(patterns, instances);

        var script = Assert.Single(scripts.SqlScriptList);
        Assert.Equal("load-customer", script.Id);
        Assert.Equal("LoadCustomer", script.Name);
        Assert.Equal(ExpectedSql, script.SqlText);

        var transform = MTS.MetaTransformScriptModel.CreateEmpty();
        var import = new MetaTransformScriptSqlService()
            .ImportSqlScriptWorkspace(transform, scripts);

        Assert.Same(transform, import.Model);
        Assert.Equal(1, import.ScriptCount);
        Assert.Equal("LoadCustomer", Assert.Single(transform.TransformScriptList).Name);
        Assert.Single(transform.InsertStatementList);
    }

    [Fact]
    public void ReusablePatternWorkspace_ContainsNoConcreteInstances()
    {
        var (patterns, instances) = CreateInsertPattern();

        Assert.Single(patterns.TransformPatternList);
        Assert.DoesNotContain(
            typeof(MTP.MetaTransformPatternModel).GetProperties(),
            property => property.Name.Contains("Application", StringComparison.Ordinal) ||
                        property.Name.Contains("Instance", StringComparison.Ordinal));
        Assert.Single(instances.TransformPatternInstanceList);
        Assert.NotEmpty(instances.TransformPatternInstancePlaceholderList);
    }

    [Fact]
    public void RepeatedPlaceholderOccurrences_ReuseTheSameScalarValue()
    {
        var patternService = new TransformPatternAuthoringService();
        var instanceService = new TransformPatternInstanceAuthoringService();
        var patterns = patternService.CreateWorkspace();
        var instances = instanceService.CreateWorkspace();
        patternService.AddPattern(patterns, "repeat", "Repeat", null, "SELECT $(value), $(value);");
        instanceService.AddInstance(instances, patterns, "repeat-once", "RepeatOnce", "repeat");
        instanceService.SetPlaceholderValue(instances, patterns, "repeat-once", "value", "42");

        var script = Assert.Single(
            TransformPatternToSqlScriptConverter.Convert(patterns, instances).SqlScriptList);

        Assert.Equal("SELECT 42, 42;", script.SqlText);
    }

    [Fact]
    public void SetPlaceholderValue_ReplacesTheExistingScalarValue()
    {
        var patternService = new TransformPatternAuthoringService();
        var instanceService = new TransformPatternInstanceAuthoringService();
        var patterns = patternService.CreateWorkspace();
        var instances = instanceService.CreateWorkspace();
        patternService.AddPattern(patterns, "scalar", "Scalar", null, "SELECT $(value);");
        instanceService.AddInstance(instances, patterns, "scalar-instance", "ScalarInstance", "scalar");

        instanceService.SetPlaceholderValue(instances, patterns, "scalar-instance", "value", "1");
        instanceService.SetPlaceholderValue(instances, patterns, "scalar-instance", "value", "2");

        var holder = Assert.Single(instances.TransformPatternInstancePlaceholderList);
        Assert.Equal("2", holder.SqlText);
        var script = Assert.Single(
            TransformPatternToSqlScriptConverter.Convert(patterns, instances).SqlScriptList);
        Assert.Equal("SELECT 2;", script.SqlText);
    }

    [Fact]
    public void FormerRepeatablePlaceholderSyntax_IsRejectedClearly()
    {
        var service = new TransformPatternAuthoringService();
        var model = service.CreateWorkspace();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            service.AddPattern(model, "repeatable", "Repeatable", null, "SELECT $(value JOIN ', ');"));

        Assert.Contains("Use $(name)", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void MultipleScalarBindingsForOnePlaceholder_AreRejectedBySanctionedWeave()
    {
        var (patterns, instances) = CreateInsertPattern();
        var holder = instances.TransformPatternInstancePlaceholderList.Single(candidate =>
            candidate.TransformPatternPlaceholderId.EndsWith(":target", StringComparison.Ordinal));
        instances.TransformPatternInstancePlaceholderList.Add(new MTPI.TransformPatternInstancePlaceholder
        {
            Id = "duplicate-target-binding",
            SqlText = "[dbo].[OtherCustomer]",
            TransformPatternInstance = holder.TransformPatternInstance,
            TransformPatternPlaceholderId = holder.TransformPatternPlaceholderId,
        });

        var exception = Assert.Throws<InvalidOperationException>(
            () => TransformPatternToSqlScriptConverter.Convert(patterns, instances));

        Assert.Contains("MWTP005", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void OnePattern_MaterializesOneSqlScriptPerInstance()
    {
        var (patterns, instances) = CreateInsertPattern();
        var service = new TransformPatternInstanceAuthoringService();
        service.AddInstance(instances, patterns, "load-product", "LoadProduct", "insert-select");
        service.SetPlaceholderValue(instances, patterns, "load-product", "target", "[dbo].[Product]");
        service.SetPlaceholderValue(instances, patterns, "load-product", "target-fields", "[ProductId]");
        service.SetPlaceholderValue(instances, patterns, "load-product", "source-expressions", "[s].[ProductId]");
        service.SetPlaceholderValue(instances, patterns, "load-product", "source", "[stage].[Product] AS [s]");

        var scripts = TransformPatternToSqlScriptConverter.Convert(patterns, instances).SqlScriptList;

        Assert.Equal(2, scripts.Count);
        Assert.Equal(
            "INSERT INTO [dbo].[Product] ([ProductId]) SELECT [s].[ProductId] FROM [stage].[Product] AS [s];",
            scripts.Single(static script => script.Id == "load-product").SqlText);
    }

    [Fact]
    public void PatternProjection_EscapesLiteralSqlCmdLikeText()
    {
        var service = new TransformPatternAuthoringService();
        var model = service.CreateWorkspace();
        const string projection = "SELECT '$$(literal)' AS Marker, $(value) AS Value;";

        service.AddPattern(model, "literal-marker", "Literal marker", null, projection);

        Assert.Equal(projection, service.EmitPattern(model, "literal-marker"));
    }

    [Fact]
    public void MissingPlaceholderValueHolder_IsRejectedBySanctionedWeave()
    {
        var (patterns, instances) = CreateInsertPattern();
        var missing = instances.TransformPatternInstancePlaceholderList.Single(holder =>
            holder.TransformPatternPlaceholderId.EndsWith(":source", StringComparison.Ordinal));
        instances.TransformPatternInstancePlaceholderList.Remove(missing);

        var exception = Assert.Throws<InvalidOperationException>(
            () => TransformPatternToSqlScriptConverter.Convert(patterns, instances));

        Assert.Contains("MWTP005", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void InstanceWhosePatternIsAbsentFromLibrary_IsRejectedClearly()
    {
        var (patterns, instances) = CreateInsertPattern();
        Assert.Single(instances.TransformPatternInstanceList).TransformPatternId = "missing-pattern";

        var exception = Assert.Throws<InvalidOperationException>(
            () => TransformPatternToSqlScriptConverter.Convert(patterns, instances));

        Assert.Contains("MWTP007", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BranchingPatternItemSequence_IsRejectedBySanctionedWeave()
    {
        var (patterns, instances) = CreateInsertPattern();
        var items = patterns.TransformPatternItemList.OrderBy(item => item.Id).ToArray();
        items[^1].PreviousItem = items[^3];

        var exception = Assert.Throws<InvalidOperationException>(
            () => TransformPatternToSqlScriptConverter.Convert(patterns, instances));

        Assert.Contains("MWTP004", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void MultipleTextShapesForOnePatternItem_AreRejectedBySanctionedWeave()
    {
        var (patterns, instances) = CreateInsertPattern();
        var item = patterns.TransformPatternItemList[0];
        patterns.TransformPatternTextList.Add(new MTP.TransformPatternText
        {
            Id = "duplicate-text",
            SqlText = "SELECT ",
            TransformPatternItem = item,
        });

        var exception = Assert.Throws<InvalidOperationException>(
            () => TransformPatternToSqlScriptConverter.Convert(patterns, instances));

        Assert.Contains("MWTP001", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void SqlScriptWorkspaceImport_RejectsBlankSqlTextClearly()
    {
        var scripts = MSS.MetaSqlScriptModel.CreateEmpty();
        scripts.SqlScriptList.Add(new MSS.SqlScript
        {
            Id = "blank",
            Name = "Blank",
            SqlText = "   ",
        });

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportSqlScriptWorkspace(
                MTS.MetaTransformScriptModel.CreateEmpty(),
                scripts));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.InvalidSqlInput, exception.Kind);
        Assert.Contains("blank", exception.Message, StringComparison.Ordinal);
    }

    private static (MTP.MetaTransformPatternModel Patterns, MTPI.MetaTransformPatternInstanceModel Instances)
        CreateInsertPattern()
    {
        var patternService = new TransformPatternAuthoringService();
        var instanceService = new TransformPatternInstanceAuthoringService();
        var patterns = patternService.CreateWorkspace();
        var instances = instanceService.CreateWorkspace();
        patternService.AddPattern(patterns, "insert-select", "Insert select", null, PatternText);
        instanceService.AddInstance(instances, patterns, "load-customer", "LoadCustomer", "insert-select");
        instanceService.SetPlaceholderValue(instances, patterns, "load-customer", "target", "[dbo].[Customer]");
        instanceService.SetPlaceholderValue(instances, patterns, "load-customer", "target-fields", "[CustomerId], [Name]");
        instanceService.SetPlaceholderValue(instances, patterns, "load-customer", "source-expressions", "[s].[CustomerId], [s].[Name]");
        instanceService.SetPlaceholderValue(instances, patterns, "load-customer", "source", "[stage].[Customer] AS [s]");
        return (patterns, instances);
    }
}
