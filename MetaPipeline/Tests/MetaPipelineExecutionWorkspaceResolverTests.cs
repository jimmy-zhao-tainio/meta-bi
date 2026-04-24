using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql;

namespace MetaPipeline.Tests;

public sealed class MetaPipelineExecutionWorkspaceResolverTests
{
    [Fact]
    public async Task Resolve_WhenScriptIsBlank_RequiresExplicitScript()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            var sqlService = new MetaTransformScriptSqlService();
            await sqlService.ImportFromSqlCodeToWorkspaceAsync(
                "select 1 as CustomerId",
                "dbo.CustomerTarget",
                transformWorkspacePath,
                "dbo.v_customer_one");
            await sqlService.AddSqlCodeToWorkspaceAsync(
                "select 2 as CustomerId",
                "dbo.CustomerTargetTwo",
                transformWorkspacePath,
                "dbo.v_customer_two");

            var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspacePath, searchUpward: false);
            var first = transformModel.TransformScriptList.Single(item => string.Equals(item.Name, "dbo.v_customer_one", StringComparison.OrdinalIgnoreCase));
            var second = transformModel.TransformScriptList.Single(item => string.Equals(item.Name, "dbo.v_customer_two", StringComparison.OrdinalIgnoreCase));

            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", first, "dbo.CustomerTarget", ["CustomerId"]),
                new BindingSeed("binding:2", second, "dbo.CustomerTargetTwo", ["CustomerId"]));

            var exception = Assert.Throws<ArgumentException>(() =>
                new MetaPipelineExecutionWorkspaceResolver().Resolve(transformWorkspacePath, bindingWorkspacePath, string.Empty));

            Assert.Contains("transformScriptName", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Resolve_UsesBindingTargetAndOrderedOutputColumns()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                "select 1 as CustomerId, 'A' as CustomerName",
                "dbo.CustomerLoad",
                transformWorkspacePath,
                "dbo.v_customer_load");

            var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);

            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, "warehouse.CustomerLoad", ["CustomerName", "CustomerId"], [1, 0]));

            var result = new MetaPipelineExecutionWorkspaceResolver().Resolve(
                transformWorkspacePath,
                bindingWorkspacePath,
                "dbo.v_customer_load");

            Assert.Equal("dbo.v_customer_load", result.TransformScriptName);
            Assert.Equal("warehouse.CustomerLoad", result.TargetSqlIdentifier);
            Assert.Collection(
                result.Columns,
                column =>
                {
                    Assert.Equal("CustomerId", column.Name);
                    Assert.Equal(0, column.Ordinal);
                },
                column =>
                {
                    Assert.Equal("CustomerName", column.Name);
                    Assert.Equal(1, column.Ordinal);
                });
            Assert.Contains("select", result.SourceSql, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Resolve_WhenTransformScriptHasParameters_FailsForStageOne()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                "create function dbo.fn_customer(@CustomerId int) returns table as return (select @CustomerId as CustomerId)",
                "dbo.CustomerLoad",
                transformWorkspacePath);

            var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);

            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, "warehouse.CustomerLoad", ["CustomerId"]));

            var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
                new MetaPipelineExecutionWorkspaceResolver().Resolve(
                    transformWorkspacePath,
                    bindingWorkspacePath,
                    "dbo.fn_customer"));

            Assert.Contains("parameterless transform scripts only", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Resolve_WhenSelectedBindingHasMultipleTargets_RequiresTarget()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                "select 1 as CustomerId",
                "dbo.CustomerLoad",
                transformWorkspacePath,
                "dbo.v_customer_load");

            var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);

            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, ["warehouse.CustomerLoad", "warehouse.CustomerLoadReplica"], ["CustomerId"]));

            var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
                new MetaPipelineExecutionWorkspaceResolver().Resolve(
                    transformWorkspacePath,
                    bindingWorkspacePath,
                    "dbo.v_customer_load"));

            Assert.Contains("Use --target", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Resolve_WithTarget_SelectsMatchingBindingTarget()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                "select 1 as CustomerId",
                "dbo.CustomerLoad",
                transformWorkspacePath,
                "dbo.v_customer_load");

            var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);

            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, ["warehouse.CustomerLoad", "warehouse.CustomerLoadReplica"], ["CustomerId"]));

            var result = new MetaPipelineExecutionWorkspaceResolver().Resolve(
                transformWorkspacePath,
                bindingWorkspacePath,
                "dbo.v_customer_load",
                "warehouse.CustomerLoadReplica");

            Assert.Equal("warehouse.CustomerLoadReplica", result.TargetSqlIdentifier);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    private static void BuildBindingWorkspace(
        string bindingWorkspacePath,
        params BindingSeed[] bindings)
    {
        var model = MetaTransformBindingModel.CreateEmpty();

        foreach (var binding in bindings)
        {
            model.TransformBindingList.Add(new TransformBinding
            {
                Id = binding.BindingId,
                MetaTransformScriptTransformScriptId = binding.Script.Id,
                TransformScriptName = binding.Script.Name,
            });

            var rowsetId = binding.BindingId + ":rowset:1";
            model.RowsetList.Add(new Rowset
            {
                Id = rowsetId,
                TransformBindingId = binding.BindingId,
                DerivationKind = "Output",
                Name = binding.Script.Name,
            });

            model.OutputRowsetList.Add(new OutputRowset
            {
                Id = binding.BindingId + ":output:1",
                TransformBindingId = binding.BindingId,
                RowsetId = rowsetId,
            });

            for (var index = 0; index < binding.TargetSqlIdentifiers.Count; index++)
            {
                model.TransformBindingTargetList.Add(new TransformBindingTarget
                {
                    Id = $"{binding.BindingId}:target:{index + 1}",
                    TransformBindingId = binding.BindingId,
                    SqlIdentifier = binding.TargetSqlIdentifiers[index],
                });
            }

            for (var index = 0; index < binding.Columns.Count; index++)
            {
                var ordinal = binding.Ordinals is not null ? binding.Ordinals[index] : index + 1;
                model.ColumnList.Add(new Column
                {
                    Id = $"{binding.BindingId}:column:{index + 1}",
                    RowsetId = rowsetId,
                    Name = binding.Columns[index],
                    Ordinal = ordinal.ToString(),
                });
            }
        }

        model.SaveToXmlWorkspace(bindingWorkspacePath);
    }

    private sealed record BindingSeed(
        string BindingId,
        TransformScript Script,
        IReadOnlyList<string> TargetSqlIdentifiers,
        IReadOnlyList<string> Columns,
        IReadOnlyList<int>? Ordinals = null)
    {
        public BindingSeed(
            string bindingId,
            TransformScript script,
            string targetSqlIdentifier,
            IReadOnlyList<string> columns,
            IReadOnlyList<int>? ordinals = null)
            : this(bindingId, script, [targetSqlIdentifier], columns, ordinals)
        {
        }
    }

    private static string CreateTempRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "MetaPipeline.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void DeleteIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
