using MetaSchema;
using MetaTransformBinding;
using MetaTransformScript;

namespace MetaTransform.Binding;

public sealed class TransformBindingWorkspaceService
{
    public BindToWorkspaceResult BindValidatedToWorkspace(
        string transformWorkspacePath,
        IEnumerable<string> sourceSchemaWorkspacePaths,
        string targetSchemaWorkspacePath,
        string executeSystemName,
        string? executeSystemDefaultSchemaName,
        string newWorkspacePath,
        TransformBindingValidationOptions? validationOptions = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentNullException.ThrowIfNull(sourceSchemaWorkspacePaths);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetSchemaWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(executeSystemName);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        var transformWorkspaceFullPath = Path.GetFullPath(transformWorkspacePath);
        var targetSchemaWorkspaceFullPath = Path.GetFullPath(targetSchemaWorkspacePath);
        var bindingWorkspaceFullPath = Path.GetFullPath(newWorkspacePath);
        var normalizedExecuteSystemName = executeSystemName.Trim();
        var normalizedExecuteSystemDefaultSchemaName = executeSystemDefaultSchemaName?.Trim() ?? string.Empty;

        var sourceSchemas = LoadSourceSchemaWorkspaces(sourceSchemaWorkspacePaths);
        var targetSchema = LoadSchemaWorkspace(targetSchemaWorkspaceFullPath, "target");

        var sourceSystemNames = sourceSchemas
            .Select(item => item.SystemName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspaceFullPath, searchUpward: false);
        var transformScripts = ResolveScripts(transformModel);
        var sourceIdentifiers = CollectSourceIdentifierUsages(transformModel, transformScripts);

        var hasOnePartSourceIdentifier = sourceIdentifiers.Any(item => item.PartCount == 1);
        var hasOneOrTwoPartSourceIdentifier = sourceIdentifiers.Any(item => item.PartCount is 1 or 2);

        if (hasOneOrTwoPartSourceIdentifier &&
            !sourceSystemNames.Contains(normalizedExecuteSystemName))
        {
            throw new TransformBindingValidationException(
                "ExecuteSystemNotPresentInSourceSchemas",
                $"Execute system '{normalizedExecuteSystemName}' is required for one/two-part source identifiers but was not found among source schema systems: {string.Join(", ", sourceSystemNames.OrderBy(static item => item, StringComparer.OrdinalIgnoreCase))}.");
        }

        if (hasOnePartSourceIdentifier &&
            string.IsNullOrWhiteSpace(normalizedExecuteSystemDefaultSchemaName))
        {
            throw new TransformBindingValidationException(
                "ExecuteSystemDefaultSchemaNameRequired",
                "At least one source identifier is one-part and requires --execute-system-default-schema-name.");
        }

        var combinedSourceSchemaModel = BuildCombinedSourceSchemaModel(sourceSchemas);
        var sourceResolver = new MetaSchemaTableResolver(combinedSourceSchemaModel);
        var targetResolver = new MetaSchemaTableResolver(targetSchema.Model);

        var packages = BindTransformScripts(
            transformModel,
            transformScripts,
            sourceResolver,
            targetResolver,
            normalizedExecuteSystemName,
            normalizedExecuteSystemDefaultSchemaName);
        EnsureBindingSucceeded(packages);

        var bindingModel = BuildCombinedBindingModel(packages);
        var baseOptions = validationOptions ?? TransformBindingValidationOptions.Default;
        var resolvedOptions = TransformBindingValidationOptions.Create(
            baseOptions.IgnoredTargetColumnNames,
            normalizedExecuteSystemName,
            normalizedExecuteSystemDefaultSchemaName);

        var validatedModel = new TransformBindingValidationService().ApplyValidation(
            bindingModel,
            sourceSchemaModel: combinedSourceSchemaModel,
            targetSchemaModel: targetSchema.Model,
            resolvedOptions);

        validatedModel.SaveToXmlWorkspace(bindingWorkspaceFullPath);

        return new BindToWorkspaceResult(
            validatedModel,
            bindingWorkspaceFullPath,
            packages.Count,
            validatedModel.TransformBindingList.Count,
            validatedModel.RowsetList.Count(item =>
                string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(item.SqlIdentifier)),
            validatedModel.TransformBindingTargetList.Count,
            0,
            0,
            validatedModel.ValidationSourceRowsetLinkList.Count,
            validatedModel.ValidationTargetRowsetLinkList.Count,
            validatedModel.ValidationSourceColumnLinkList.Count,
            validatedModel.ValidationTargetColumnLinkList.Count);
    }

    public BindToWorkspaceResult BindToWorkspace(
        string transformWorkspacePath,
        string newWorkspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        var transformWorkspaceFullPath = Path.GetFullPath(transformWorkspacePath);
        var bindingWorkspaceFullPath = Path.GetFullPath(newWorkspacePath);

        var transformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspaceFullPath, searchUpward: false);
        var transformScripts = ResolveScripts(transformModel);
        var packages = BindTransformScripts(transformModel, transformScripts);
        var bindingModel = BuildCombinedBindingModel(packages);

        bindingModel.SaveToXmlWorkspace(bindingWorkspaceFullPath);

        var issueCount = packages.Sum(item => item.Bound.Issues.Count);
        var errorCount = issueCount;

        return new BindToWorkspaceResult(
            bindingModel,
            bindingWorkspaceFullPath,
            packages.Count,
            bindingModel.TransformBindingList.Count,
            bindingModel.RowsetList.Count(item =>
                string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(item.SqlIdentifier)),
            bindingModel.TransformBindingTargetList.Count,
            issueCount,
            errorCount);
    }

    private static TransformScript[] ResolveScripts(MetaTransformScriptModel model)
    {
        var scripts = model.TransformScriptList.ToArray();
        if (scripts.Length == 0)
        {
            throw new InvalidOperationException("MetaTransformScript workspace does not contain any TransformScript rows.");
        }

        return scripts;
    }

    private static TransformBindingTargetResolution CreateTargetFromTransformScript(TransformScript transformScript)
    {
        var trimmed = transformScript.TargetSqlIdentifier?.Trim() ?? string.Empty;
        var parts = trimmed
            .Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is < 1 or > 3)
        {
            throw new InvalidOperationException(
                $"Transform script '{transformScript.Name}' target '{transformScript.TargetSqlIdentifier}' uses {parts.Length} identifier parts; binding supports table, schema.table, or database.schema.table targets only.");
        }

        return new TransformBindingTargetResolution(trimmed, null);
    }

    private static List<ScriptBindingPackage> BindTransformScripts(
        MetaTransformScriptModel transformModel,
        IReadOnlyList<TransformScript> transformScripts,
        MetaSchemaModel? sourceSchema = null)
    {
        var bindingService = new TransformBindingService();
        var packages = new List<ScriptBindingPackage>(transformScripts.Count);

        foreach (var transformScript in transformScripts)
        {
            var target = CreateTargetFromTransformScript(transformScript);
            var bound = sourceSchema is null
                ? bindingService.BindTransform(transformModel, transformScript)
                : bindingService.BindTransform(transformModel, transformScript, sourceSchema);
            packages.Add(new ScriptBindingPackage(transformScript, bound, target));
        }

        return packages;
    }

    private static List<ScriptBindingPackage> BindTransformScripts(
        MetaTransformScriptModel transformModel,
        IReadOnlyList<TransformScript> transformScripts,
        MetaSchemaTableResolver sourceResolver,
        MetaSchemaTableResolver targetResolver,
        string executeSystemName,
        string executeSystemDefaultSchemaName)
    {
        var bindingService = new TransformBindingService();
        var packages = new List<ScriptBindingPackage>(transformScripts.Count);

        foreach (var transformScript in transformScripts)
        {
            var target = CreateTargetFromTransformScript(transformScript);
            var bound = bindingService.BindTransform(
                transformModel,
                transformScript,
                sourceResolver,
                targetResolver,
                executeSystemName,
                executeSystemDefaultSchemaName);
            packages.Add(new ScriptBindingPackage(transformScript, bound, target));
        }

        return packages;
    }

    private static void EnsureBindingSucceeded(IReadOnlyList<ScriptBindingPackage> packages)
    {
        foreach (var package in packages)
        {
            if (!package.Bound.HasErrors)
            {
                continue;
            }

            var firstError = package.Bound.Issues.FirstOrDefault();
            var errorMessage = firstError is null
                ? $"Transform script '{package.TransformScript.Name}' produced one or more binding errors."
                : $"Transform script '{package.TransformScript.Name}' failed binding with {firstError.Code}: {firstError.Message}";

            throw new TransformBindingValidationException("BindingFailed", errorMessage);
        }
    }

    private static MetaTransformBindingModel BuildCombinedBindingModel(IReadOnlyList<ScriptBindingPackage> packages)
    {
        var model = MetaTransformBindingModel.CreateEmpty();

        foreach (var package in packages)
        {
            var partial = TransformBindingModelBuilder.Create(package.Bound, [package.Target]);
            MergeById(model.TransformBindingList, partial.TransformBindingList, static item => item.Id, "TransformBinding", package.TransformScript.Name);
            MergeById(model.TransformBindingTargetList, partial.TransformBindingTargetList, static item => item.Id, "TransformBindingTarget", package.TransformScript.Name);
            MergeById(model.RowsetList, partial.RowsetList, static item => item.Id, "Rowset", package.TransformScript.Name);
            MergeById(model.SourceTargetList, partial.SourceTargetList, static item => item.Id, "SourceTarget", package.TransformScript.Name);
            MergeById(model.ColumnList, partial.ColumnList, static item => item.Id, "Column", package.TransformScript.Name);
            MergeById(model.ColumnReferenceList, partial.ColumnReferenceList, static item => item.Id, "ColumnReference", package.TransformScript.Name);
            MergeById(model.TableSourceList, partial.TableSourceList, static item => item.Id, "TableSource", package.TransformScript.Name);
            MergeById(model.OutputRowsetList, partial.OutputRowsetList, static item => item.Id, "OutputRowset", package.TransformScript.Name);
        }

        return model;
    }

    private static void MergeById<T>(
        List<T> destination,
        IReadOnlyList<T> source,
        Func<T, string> idSelector,
        string entityName,
        string transformScriptName)
    {
        var seen = destination
            .Select(idSelector)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var item in source)
        {
            var id = idSelector(item);
            if (!seen.Add(id))
            {
                throw new InvalidOperationException(
                    $"Binding merge produced duplicate {entityName} Id '{id}' while processing transform script '{transformScriptName}'.");
            }

            destination.Add(item);
        }
    }

    private static IReadOnlyList<SchemaWorkspaceInput> LoadSourceSchemaWorkspaces(IEnumerable<string> sourceSchemaWorkspacePaths)
    {
        var loaded = new List<SchemaWorkspaceInput>();
        var seenPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var sourceSchemaWorkspacePath in sourceSchemaWorkspacePaths)
        {
            if (string.IsNullOrWhiteSpace(sourceSchemaWorkspacePath))
            {
                continue;
            }

            var fullPath = Path.GetFullPath(sourceSchemaWorkspacePath);
            if (!seenPaths.Add(fullPath))
            {
                continue;
            }

            loaded.Add(LoadSchemaWorkspace(fullPath, "source"));
        }

        if (loaded.Count == 0)
        {
            throw new TransformBindingValidationException(
                "SourceSchemaWorkspaceMissing",
                "Bind requires at least one --source-schema workspace.");
        }

        var duplicateSystemNames = loaded
            .GroupBy(item => item.SystemName, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (duplicateSystemNames.Length > 0)
        {
            throw new TransformBindingValidationException(
                "SourceSchemaSystemNameDuplicate",
                $"Source schema workspaces must expose unique System.Name values. Duplicates: {string.Join(", ", duplicateSystemNames)}.");
        }

        return loaded;
    }

    private static SchemaWorkspaceInput LoadSchemaWorkspace(string workspaceFullPath, string role)
    {
        var model = MetaSchemaModel.LoadFromXmlWorkspace(workspaceFullPath, searchUpward: false);
        if (model.SystemList.Count != 1)
        {
            throw new TransformBindingValidationException(
                "SchemaWorkspaceSystemCardinalityInvalid",
                $"The {role} schema workspace '{workspaceFullPath}' contains {model.SystemList.Count} system rows; exactly one is required.");
        }

        var systemName = model.SystemList[0].Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(systemName))
        {
            throw new TransformBindingValidationException(
                "SchemaWorkspaceSystemNameMissing",
                $"The {role} schema workspace '{workspaceFullPath}' has a system row with blank Name.");
        }

        return new SchemaWorkspaceInput(workspaceFullPath, model, systemName);
    }

    private static IReadOnlyList<SourceIdentifierUsage> CollectSourceIdentifierUsages(
        MetaTransformScriptModel transformModel,
        IReadOnlyList<TransformScript> transformScripts)
    {
        var bindingService = new TransformBindingService();
        var usages = new List<SourceIdentifierUsage>();

        foreach (var transformScript in transformScripts)
        {
            var bound = bindingService.BindTransform(transformModel, transformScript);
            foreach (var sourceRowset in bound.Rowsets.Where(item =>
                         string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                         !string.IsNullOrWhiteSpace(item.SqlIdentifier)))
            {
                if (!SourceSqlIdentifierExpansion.TryGetPartCount(sourceRowset.SqlIdentifier!, out var partCount))
                {
                    continue;
                }

                usages.Add(new SourceIdentifierUsage(transformScript.Name, sourceRowset.SqlIdentifier!, partCount));
            }
        }

        return usages;
    }

    private static MetaSchemaModel BuildCombinedSourceSchemaModel(IReadOnlyList<SchemaWorkspaceInput> sourceSchemas)
    {
        var combined = MetaSchemaModel.CreateEmpty();

        for (var index = 0; index < sourceSchemas.Count; index++)
        {
            var source = sourceSchemas[index];
            var idPrefix = $"source:{source.SystemName}:";

            var systemIdMap = source.Model.SystemList
                .ToDictionary(
                    item => item.Id,
                    item => $"{idPrefix}system:{item.Id}",
                    StringComparer.Ordinal);
            var schemaIdMap = source.Model.SchemaList
                .ToDictionary(
                    item => item.Id,
                    item => $"{idPrefix}schema:{item.Id}",
                    StringComparer.Ordinal);
            var tableIdMap = source.Model.TableList
                .ToDictionary(
                    item => item.Id,
                    item => $"{idPrefix}table:{item.Id}",
                    StringComparer.Ordinal);
            var fieldIdMap = source.Model.FieldList
                .ToDictionary(
                    item => item.Id,
                    item => $"{idPrefix}field:{item.Id}",
                    StringComparer.Ordinal);

            foreach (var system in source.Model.SystemList)
            {
                combined.SystemList.Add(new MetaSchema.System
                {
                    Id = systemIdMap[system.Id],
                    Name = system.Name,
                    Description = system.Description
                });
            }

            foreach (var schema in source.Model.SchemaList)
            {
                combined.SchemaList.Add(new Schema
                {
                    Id = schemaIdMap[schema.Id],
                    SystemId = systemIdMap[schema.SystemId],
                    Name = schema.Name
                });
            }

            foreach (var table in source.Model.TableList)
            {
                combined.TableList.Add(new Table
                {
                    Id = tableIdMap[table.Id],
                    SchemaId = schemaIdMap[table.SchemaId],
                    Name = table.Name,
                    ObjectType = table.ObjectType
                });
            }

            foreach (var field in source.Model.FieldList)
            {
                combined.FieldList.Add(new Field
                {
                    Id = fieldIdMap[field.Id],
                    TableId = tableIdMap[field.TableId],
                    Name = field.Name,
                    Ordinal = field.Ordinal,
                    MetaDataTypeId = field.MetaDataTypeId,
                    IsNullable = field.IsNullable,
                    IsIdentity = field.IsIdentity,
                    IdentitySeed = field.IdentitySeed,
                    IdentityIncrement = field.IdentityIncrement
                });
            }

            foreach (var detail in source.Model.FieldDataTypeDetailList)
            {
                if (!fieldIdMap.TryGetValue(detail.FieldId, out var mappedFieldId))
                {
                    continue;
                }

                combined.FieldDataTypeDetailList.Add(new FieldDataTypeDetail
                {
                    Id = $"{idPrefix}field-detail:{detail.Id}",
                    FieldId = mappedFieldId,
                    Name = detail.Name,
                    Value = detail.Value
                });
            }
        }

        return combined;
    }

    private sealed record ScriptBindingPackage(
        TransformScript TransformScript,
        TransformBindingResult Bound,
        TransformBindingTargetResolution Target);

    private sealed record SchemaWorkspaceInput(
        string WorkspacePath,
        MetaSchemaModel Model,
        string SystemName);

    private sealed record SourceIdentifierUsage(
        string TransformScriptName,
        string SqlIdentifier,
        int PartCount);
}

public sealed record BindToWorkspaceResult(
    MetaTransformBindingModel Model,
    string WorkspacePath,
    int TransformScriptCount,
    int TransformBindingCount,
    int SourceCount,
    int TargetCount,
    int IssueCount,
    int ErrorCount,
    int SourceRowsetValidationCount = 0,
    int TargetRowsetValidationCount = 0,
    int SourceColumnValidationCount = 0,
    int TargetColumnValidationCount = 0);
