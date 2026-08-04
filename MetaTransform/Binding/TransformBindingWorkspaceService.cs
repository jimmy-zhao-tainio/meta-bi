using MetaSchema;
using MetaTransformBinding;
using MetaTransformScript;
using MetaDataTypeConversion.Core;

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
        TransformBindingValidationOptions? validationOptions = null,
        string? dataTypeConversionWorkspacePath = null,
        bool allowPartial = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);
        var workspacePath = Path.GetFullPath(newWorkspacePath);
        var result = BindValidated(
            transformWorkspacePath,
            sourceSchemaWorkspacePaths,
            targetSchemaWorkspacePath,
            executeSystemName,
            executeSystemDefaultSchemaName,
            validationOptions,
            dataTypeConversionWorkspacePath,
            allowPartial);
        result.Model.SaveToXmlWorkspace(workspacePath);
        return result with { WorkspacePath = workspacePath };
    }

    public BindToWorkspaceResult BindValidated(
        string transformWorkspacePath,
        IEnumerable<string> sourceSchemaWorkspacePaths,
        string targetSchemaWorkspacePath,
        string executeSystemName,
        string? executeSystemDefaultSchemaName,
        TransformBindingValidationOptions? validationOptions = null,
        string? dataTypeConversionWorkspacePath = null,
        bool allowPartial = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentNullException.ThrowIfNull(sourceSchemaWorkspacePaths);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetSchemaWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(executeSystemName);

        var transformWorkspaceFullPath = Path.GetFullPath(transformWorkspacePath);
        var targetSchemaWorkspaceFullPath = Path.GetFullPath(targetSchemaWorkspacePath);
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

        var baseOptions = validationOptions ?? TransformBindingValidationOptions.Default;
        var resolvedOptions = TransformBindingValidationOptions.Create(
            baseOptions.IgnoredTargetColumnNames,
            baseOptions.IgnoredTargetColumnNamesIfPresent,
            normalizedExecuteSystemName,
            normalizedExecuteSystemDefaultSchemaName);

        var dataTypeConversionWorkspace = MetaDataTypeConversionWorkspaceProvider.LoadOrDefault(dataTypeConversionWorkspacePath);
        var validationService = new TransformBindingValidationService(
            new MetaDataTypeConversionService(),
            dataTypeConversionWorkspace);

        var objectIssues = new List<BindWorkspaceObjectIssue>();
        MetaTransformBindingModel validatedModel;

        if (!allowPartial)
        {
            var packages = BindTransformScripts(
                transformModel,
                transformScripts,
                sourceResolver,
                targetResolver,
                normalizedExecuteSystemName,
                normalizedExecuteSystemDefaultSchemaName);
            EnsureBindingSucceeded(packages);

            var bindingModel = BuildCombinedBindingModel(packages);
            validatedModel = validationService.ApplyValidation(
                bindingModel,
                sourceSchemaModel: combinedSourceSchemaModel,
                targetSchemaModel: targetSchema.Model,
                packages.Select(item => item.Bound).ToArray(),
                resolvedOptions);
        }
        else
        {
            var packages = BindTransformScripts(
                transformModel,
                transformScripts,
                sourceResolver,
                targetResolver,
                normalizedExecuteSystemName,
                normalizedExecuteSystemDefaultSchemaName,
                objectIssues);

            validatedModel = MetaTransformBindingModel.CreateEmpty();
            foreach (var package in packages)
            {
                var packageModel = BuildCombinedBindingModel([package]);
                try
                {
                    var validatedPackageModel = validationService.ApplyValidation(
                        packageModel,
                        sourceSchemaModel: combinedSourceSchemaModel,
                        targetSchemaModel: targetSchema.Model,
                        [package.Bound],
                        resolvedOptions);
                    MergeBindingModel(validatedModel, validatedPackageModel, package.TransformScript.Name);
                }
                catch (TransformBindingValidationException ex)
                {
                    objectIssues.Add(CreateObjectIssue(package.TransformScript, "Validation", ex.Code, ex.Message));
                }
            }
        }

        return new BindToWorkspaceResult(
            validatedModel,
            string.Empty,
            transformScripts.Length,
            validatedModel.TransformBindingList.Count,
            validatedModel.RowsetList.Count(item =>
                string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(item.SqlIdentifier)),
            validatedModel.TransformBindingTargetList.Count,
            objectIssues.Count,
            objectIssues.Count,
            validatedModel.ValidationSourceRowsetLinkList.Count,
            validatedModel.ValidationTargetRowsetLinkList.Count,
            validatedModel.ValidationSourceColumnLinkList.Count,
            validatedModel.ValidationTargetColumnLinkList.Count,
            objectIssues);
    }

    // Test support for consumers that need syntax-derived rowsets without schema contracts.
    // Strict read/write/delete facts belong to BindValidatedToWorkspace.
    internal BindToWorkspaceResult BindStructureToWorkspace(
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

        var objectIssues = packages
            .SelectMany(package => package.Bound.Issues.Select(issue =>
                CreateObjectIssue(
                    package.TransformScript,
                    "Binding",
                    issue.Code,
                    issue.Message)))
            .ToArray();
        var issueCount = objectIssues.Length;
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
            errorCount,
            ObjectIssues: objectIssues);
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

    private static TransformBindingTargetResolution? CreateTargetFromTransformScript(
        MetaTransformScriptModel transformModel,
        TransformScript transformScript)
    {
        var navigator = new TransformScriptNavigator(transformModel);
        var scriptObjectView = transformModel.ScriptObjectViewList.SingleOrDefault(item =>
            string.Equals(item.TransformScript.Id, transformScript.Id, StringComparison.Ordinal));
        var scriptObjectTvf = transformModel.ScriptObjectTVFList.SingleOrDefault(item =>
            string.Equals(item.TransformScript.Id, transformScript.Id, StringComparison.Ordinal));
        var scriptObjectScalarFunction = transformModel.ScriptObjectScalarFunctionList.SingleOrDefault(item =>
            string.Equals(item.TransformScript.Id, transformScript.Id, StringComparison.Ordinal));
        var scriptObjectStoredProcedure = transformModel.ScriptObjectStoredProcedureList.SingleOrDefault(item =>
            string.Equals(item.TransformScript.Id, transformScript.Id, StringComparison.Ordinal));

        var objectTypeCount =
            (scriptObjectView is null ? 0 : 1) +
            (scriptObjectTvf is null ? 0 : 1) +
            (scriptObjectScalarFunction is null ? 0 : 1) +
            (scriptObjectStoredProcedure is null ? 0 : 1);
        if (objectTypeCount > 1)
        {
            throw new TransformBindingValidationException(
                "TransformScriptObjectTypeAmbiguous",
                $"Transform script '{transformScript.Name}' has more than one script object row. Exactly one script object type is allowed.");
        }

        var isInlineTableValuedFunction = scriptObjectTvf is not null;
        var isScalarFunction = scriptObjectScalarFunction is not null;
        var isStoredProcedure = scriptObjectStoredProcedure is not null;
        var trimmed = scriptObjectView?.TargetSqlIdentifier?.Trim()
            ?? navigator.TryGetMutationTargetSqlIdentifier(transformScript)?.Trim()
            ?? string.Empty;

        if (isInlineTableValuedFunction)
        {
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                throw new TransformBindingValidationException(
                    "InlineTvfTargetNotAllowed",
                    $"Transform script '{transformScript.Name}' is an inline TVF and must not define TargetSqlIdentifier.");
            }

            return null;
        }

        if (isScalarFunction)
        {
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                throw new TransformBindingValidationException(
                    "ScalarFunctionTargetNotAllowed",
                    $"Transform script '{transformScript.Name}' is a scalar function and must not define TargetSqlIdentifier.");
            }

            return null;
        }

        if (isStoredProcedure)
        {
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                throw new TransformBindingValidationException(
                    "StoredProcedureTargetNotAllowed",
                    $"Transform script '{transformScript.Name}' is a stored procedure and must declare target effects through StoredProcedureContractOperation rows instead of TargetSqlIdentifier.");
            }

            return null;
        }

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            var statementKind = navigator.GetTransformScriptStatementKind(transformScript);
            if (statementKind is BoundStatementKind.Select)
            {
                return null;
            }

            throw new TransformBindingValidationException(
                "TransformScriptTargetSqlIdentifierMissing",
                $"Transform script '{transformScript.Name}' is missing TargetSqlIdentifier.");
        }

        var parts = trimmed
            .Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is < 1 or > 3)
        {
            throw new InvalidOperationException(
                $"Transform script '{transformScript.Name}' target '{scriptObjectView?.TargetSqlIdentifier}' uses {parts.Length} identifier parts; binding supports table, schema.table, or database.schema.table targets only.");
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
            var target = CreateTargetFromTransformScript(transformModel, transformScript);
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
            var target = CreateTargetFromTransformScript(transformModel, transformScript);
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

    private static List<ScriptBindingPackage> BindTransformScripts(
        MetaTransformScriptModel transformModel,
        IReadOnlyList<TransformScript> transformScripts,
        MetaSchemaTableResolver sourceResolver,
        MetaSchemaTableResolver targetResolver,
        string executeSystemName,
        string executeSystemDefaultSchemaName,
        ICollection<BindWorkspaceObjectIssue> objectIssues)
    {
        var bindingService = new TransformBindingService();
        var packages = new List<ScriptBindingPackage>(transformScripts.Count);

        foreach (var transformScript in transformScripts)
        {
            try
            {
                var target = CreateTargetFromTransformScript(transformModel, transformScript);
                var bound = bindingService.BindTransform(
                    transformModel,
                    transformScript,
                    sourceResolver,
                    targetResolver,
                    executeSystemName,
                    executeSystemDefaultSchemaName);

                if (bound.HasErrors)
                {
                    objectIssues.Add(CreateBindingIssue(transformScript, bound));
                    continue;
                }

                packages.Add(new ScriptBindingPackage(transformScript, bound, target));
            }
            catch (TransformBindingValidationException ex)
            {
                objectIssues.Add(CreateObjectIssue(transformScript, "Binding", ex.Code, ex.Message));
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
            {
                objectIssues.Add(CreateObjectIssue(transformScript, "Binding", ex.GetType().Name, ex.Message));
            }
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
            var partial = package.Target is null
                ? TransformBindingModelBuilder.Create(package.Bound)
                : TransformBindingModelBuilder.Create(package.Bound, [package.Target]);
            MergeBindingModel(model, partial, package.TransformScript.Name);
        }

        return model;
    }

    private static void MergeBindingModel(
        MetaTransformBindingModel destination,
        MetaTransformBindingModel source,
        string transformScriptName)
    {
        MergeById(destination.TransformBindingList, source.TransformBindingList, static item => item.Id, "TransformBinding", transformScriptName);
        MergeById(destination.TransformBindingTargetList, source.TransformBindingTargetList, static item => item.Id, "TransformBindingTarget", transformScriptName);
        MergeById(destination.RowsetList, source.RowsetList, static item => item.Id, "Rowset", transformScriptName);
        MergeById(destination.SourceTargetList, source.SourceTargetList, static item => item.Id, "SourceTarget", transformScriptName);
        MergeById(destination.ColumnList, source.ColumnList, static item => item.Id, "Column", transformScriptName);
        MergeById(destination.ColumnReferenceList, source.ColumnReferenceList, static item => item.Id, "ColumnReference", transformScriptName);
        MergeById(destination.TableSourceList, source.TableSourceList, static item => item.Id, "TableSource", transformScriptName);
        MergeById(destination.OutputRowsetList, source.OutputRowsetList, static item => item.Id, "OutputRowset", transformScriptName);
        MergeById(destination.ValidationList, source.ValidationList, static item => item.Id, "Validation", transformScriptName);
        MergeById(destination.ValidationSourceRowsetLinkList, source.ValidationSourceRowsetLinkList, static item => item.Id, "ValidationSourceRowsetLink", transformScriptName);
        MergeById(destination.ValidationTargetRowsetLinkList, source.ValidationTargetRowsetLinkList, static item => item.Id, "ValidationTargetRowsetLink", transformScriptName);
        MergeById(destination.ValidationSourceColumnLinkList, source.ValidationSourceColumnLinkList, static item => item.Id, "ValidationSourceColumnLink", transformScriptName);
        MergeById(destination.ValidationTargetColumnLinkList, source.ValidationTargetColumnLinkList, static item => item.Id, "ValidationTargetColumnLink", transformScriptName);
        MergeById(destination.ValidationTargetColumnTypeExactList, source.ValidationTargetColumnTypeExactList, static item => item.Id, "ValidationTargetColumnTypeExact", transformScriptName);
        MergeById(destination.ValidationTargetColumnTypeSanctionedConversionList, source.ValidationTargetColumnTypeSanctionedConversionList, static item => item.Id, "ValidationTargetColumnTypeSanctionedConversion", transformScriptName);
        MergeById(destination.ValidationTargetIgnoredColumnList, source.ValidationTargetIgnoredColumnList, static item => item.Id, "ValidationTargetIgnoredColumn", transformScriptName);
        MergeById(destination.TargetColumnReferenceList, source.TargetColumnReferenceList, static item => item.Id, "TargetColumnReference", transformScriptName);
        MergeById(destination.WriteList, source.WriteList, static item => item.Id, "Write", transformScriptName);
        MergeById(destination.WriteValueList, source.WriteValueList, static item => item.Id, "WriteValue", transformScriptName);
        MergeById(destination.WriteValueScalarExpressionList, source.WriteValueScalarExpressionList, static item => item.Id, "WriteValueScalarExpression", transformScriptName);
        MergeById(destination.InsertQueryWriteList, source.InsertQueryWriteList, static item => item.Id, "InsertQueryWrite", transformScriptName);
        MergeById(destination.InsertValuesWriteList, source.InsertValuesWriteList, static item => item.Id, "InsertValuesWrite", transformScriptName);
        MergeById(destination.UpdateWriteList, source.UpdateWriteList, static item => item.Id, "UpdateWrite", transformScriptName);
        MergeById(destination.MergeInsertWriteList, source.MergeInsertWriteList, static item => item.Id, "MergeInsertWrite", transformScriptName);
        MergeById(destination.MergeUpdateWriteList, source.MergeUpdateWriteList, static item => item.Id, "MergeUpdateWrite", transformScriptName);
        MergeById(destination.DeleteList, source.DeleteList, static item => item.Id, "Delete", transformScriptName);
        MergeById(destination.MergeDeleteList, source.MergeDeleteList, static item => item.Id, "MergeDelete", transformScriptName);
        MergeById(destination.TruncateList, source.TruncateList, static item => item.Id, "Truncate", transformScriptName);
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
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

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
            var schemaObjectIdMap = source.Model.SchemaObjectList
                .ToDictionary(
                    item => item.Id,
                    item => $"{idPrefix}schema-object:{item.Id}",
                    StringComparer.Ordinal);
            var tableIdMap = source.Model.TableList
                .ToDictionary(
                    item => item.Id,
                    item => $"{idPrefix}table:{item.Id}",
                    StringComparer.Ordinal);
            var viewIdMap = source.Model.ViewList
                .ToDictionary(
                    item => item.Id,
                    item => $"{idPrefix}view:{item.Id}",
                    StringComparer.Ordinal);
            var fieldIdMap = source.Model.FieldList
                .ToDictionary(
                    item => item.Id,
                    item => $"{idPrefix}field:{item.Id}",
                    StringComparer.Ordinal);
            var systemsByOriginalId = new Dictionary<string, MetaSchema.System>(StringComparer.Ordinal);
            var schemasByOriginalId = new Dictionary<string, Schema>(StringComparer.Ordinal);
            var schemaObjectsByOriginalId = new Dictionary<string, SchemaObject>(StringComparer.Ordinal);
            var fieldsByOriginalId = new Dictionary<string, Field>(StringComparer.Ordinal);

            foreach (var system in source.Model.SystemList)
            {
                var combinedSystem = new MetaSchema.System
                {
                    Id = systemIdMap[system.Id],
                    Name = system.Name,
                    Description = system.Description
                };
                combined.SystemList.Add(combinedSystem);
                systemsByOriginalId.Add(system.Id, combinedSystem);
            }

            foreach (var schema in source.Model.SchemaList)
            {
                var combinedSchema = new Schema
                {
                    Id = schemaIdMap[schema.Id],
                    System = systemsByOriginalId[schema.System.Id],
                    Name = schema.Name
                };
                combined.SchemaList.Add(combinedSchema);
                schemasByOriginalId.Add(schema.Id, combinedSchema);
            }

            foreach (var schemaObject in source.Model.SchemaObjectList)
            {
                var combinedSchemaObject = new SchemaObject
                {
                    Id = schemaObjectIdMap[schemaObject.Id],
                    Schema = schemasByOriginalId[schemaObject.Schema.Id],
                    Name = schemaObject.Name
                };
                combined.SchemaObjectList.Add(combinedSchemaObject);
                schemaObjectsByOriginalId.Add(schemaObject.Id, combinedSchemaObject);
            }

            foreach (var table in source.Model.TableList)
            {
                combined.TableList.Add(new Table
                {
                    Id = tableIdMap[table.Id],
                    SchemaObject = schemaObjectsByOriginalId[table.SchemaObject.Id]
                });
            }

            foreach (var view in source.Model.ViewList)
            {
                combined.ViewList.Add(new View
                {
                    Id = viewIdMap[view.Id],
                    SchemaObject = schemaObjectsByOriginalId[view.SchemaObject.Id]
                });
            }

            foreach (var field in source.Model.FieldList)
            {
                var combinedField = new Field
                {
                    Id = fieldIdMap[field.Id],
                    SchemaObject = schemaObjectsByOriginalId[field.SchemaObject.Id],
                    Name = field.Name,
                    Ordinal = field.Ordinal,
                    MetaDataTypeId = field.MetaDataTypeId,
                    IsNullable = field.IsNullable,
                    IsIdentity = field.IsIdentity,
                    IdentitySeed = field.IdentitySeed,
                    IdentityIncrement = field.IdentityIncrement
                };
                combined.FieldList.Add(combinedField);
                fieldsByOriginalId.Add(field.Id, combinedField);
            }

            foreach (var detail in source.Model.FieldDataTypeDetailList)
            {
                if (!fieldsByOriginalId.TryGetValue(detail.Field.Id, out var mappedField))
                {
                    continue;
                }

                combined.FieldDataTypeDetailList.Add(new FieldDataTypeDetail
                {
                    Id = $"{idPrefix}field-detail:{detail.Id}",
                    Field = mappedField,
                    Name = detail.Name,
                    Value = detail.Value
                });
            }
        }

        return combined;
    }

    private static BindWorkspaceObjectIssue CreateBindingIssue(
        TransformScript transformScript,
        TransformBindingResult bound)
    {
        var firstError = bound.Issues.FirstOrDefault();
        return firstError is null
            ? CreateObjectIssue(
                transformScript,
                "Binding",
                "BindingFailed",
                $"Transform script '{transformScript.Name}' produced one or more binding errors.")
            : CreateObjectIssue(transformScript, "Binding", firstError.Code, firstError.Message);
    }

    private static BindWorkspaceObjectIssue CreateObjectIssue(
        TransformScript transformScript,
        string stage,
        string code,
        string message)
    {
        return new BindWorkspaceObjectIssue(
            transformScript.Id,
            transformScript.Name,
            stage,
            code,
            message);
    }

    private sealed record ScriptBindingPackage(
        TransformScript TransformScript,
        TransformBindingResult Bound,
        TransformBindingTargetResolution? Target);

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
    int TargetColumnValidationCount = 0,
    IReadOnlyList<BindWorkspaceObjectIssue>? ObjectIssues = null)
{
    public int SkippedTransformScriptCount => ObjectIssues?.Count ?? 0;
}

public sealed record BindWorkspaceObjectIssue(
    string TransformScriptId,
    string TransformScriptName,
    string Stage,
    string Code,
    string Message);
