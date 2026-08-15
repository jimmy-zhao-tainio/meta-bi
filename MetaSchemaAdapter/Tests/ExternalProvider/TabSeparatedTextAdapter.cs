using System.Globalization;
using System.Runtime.CompilerServices;
using MetaPipeline;
using MetaSchema;
using MetaSchemaAdapter;
using MetaTransformBinding;
using MetaTransformScript;

namespace ExampleFileProvider;

/// <summary>
/// Test-only example of an adapter implemented in an external provider namespace.
/// It deliberately supports one bounded semantic shape: a single-table projection with
/// an optional equality predicate.
/// </summary>
internal sealed class TabSeparatedTextAdapter :
    IMetaSchemaDiscoveryAdapter,
    IMetaSchemaTransformAdapter,
    IMetaSchemaTargetWriteAdapter
{
    private readonly IReadOnlyDictionary<string, string> rootsByConnectionReference;

    public TabSeparatedTextAdapter(IReadOnlyDictionary<string, string> rootsByConnectionReference)
    {
        this.rootsByConnectionReference = rootsByConnectionReference;
    }

    public string Id => "tab-separated-text";

    public ValueTask<MetaSchemaModel> DiscoverSchemaAsync(
        MetaSchemaDiscoveryRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var root = ResolveRoot(request.ConnectionReference);
        var files = Directory.GetFiles(root, "*.tsv", SearchOption.TopDirectoryOnly)
            .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static path => path, StringComparer.Ordinal)
            .ToArray();
        if (files.Length == 0)
        {
            throw new InvalidOperationException($"No .tsv files were found for connection reference '{request.ConnectionReference}'.");
        }

        var model = MetaSchemaModel.CreateEmpty();
        var system = new MetaSchema.System
        {
            Id = $"tsv:system:{NormalizeIdPart(request.SystemName)}",
            Name = request.SystemName
        };
        var schema = new Schema
        {
            Id = $"{system.Id}:schema:files",
            Name = "files",
            System = system
        };
        model.SystemList.Add(system);
        model.SchemaList.Add(schema);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var tableName = Path.GetFileNameWithoutExtension(file);
            var tableId = $"{schema.Id}:table:{NormalizeIdPart(tableName)}";
            var schemaObject = new SchemaObject
            {
                Id = tableId,
                Name = tableName,
                Schema = schema
            };
            model.SchemaObjectList.Add(schemaObject);
            model.TableList.Add(new Table
            {
                Id = tableId,
                SchemaObject = schemaObject
            });

            var headers = ReadHeaders(file);
            for (var ordinal = 0; ordinal < headers.Length; ordinal++)
            {
                model.FieldList.Add(new Field
                {
                    Id = $"{tableId}:field:{NormalizeIdPart(headers[ordinal])}",
                    Name = headers[ordinal],
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture),
                    MetaDataTypeId = "meta:type:String",
                    IsNullable = "true",
                    SchemaObject = schemaObject
                });
            }
        }

        return ValueTask.FromResult(model);
    }

    public ValueTask<IPipelineRowStreamSource> CreateRowStreamSourceAsync(
        MetaSchemaRowStreamRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (request.BatchSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request),
                request.BatchSize,
                "BatchSize must be greater than zero.");
        }

        var root = ResolveRoot(request.ConnectionReference);
        var plan = CompileProjection(request, root);
        return ValueTask.FromResult<IPipelineRowStreamSource>(
            new TabSeparatedTransformSource(plan, request.OutputShape, request.BatchSize));
    }

    public ValueTask<IPipelineTargetWriteOperation> CreateInsertRowsOperationAsync(
        MetaSchemaInsertRowsRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var root = ResolveRoot(request.ConnectionReference);
        var target = ResolveSchemaObject(request.Schema, request.TargetIdentifier);
        var fields = request.Schema.FieldList
            .Where(field => string.Equals(field.SchemaObject.Id, target.Id, StringComparison.Ordinal))
            .OrderBy(field => ParseOrdinal(field.Ordinal))
            .Select((field, ordinal) => new PipelineColumn(field.Name, ordinal, field.MetaDataTypeId, field.MetaDataTypeId))
            .ToArray();
        var targetShape = new PipelineRowStreamShape(fields);
        request.Shape.EnsureCompatibleWith(targetShape, $"MetaSchema target '{request.TargetIdentifier}'");

        return ValueTask.FromResult<IPipelineTargetWriteOperation>(
            new TabSeparatedTargetWriteOperation(
                Path.Combine(root, target.Name + ".tsv"),
                request.Shape));
    }

    private ProjectionPlan CompileProjection(MetaSchemaRowStreamRequest request, string root)
    {
        var transform = request.Transforms.TransformScriptList.SingleOrDefault(item =>
                string.Equals(item.Id, request.TransformScriptId, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"TransformScript '{request.TransformScriptId}' was not found.");
        var binding = request.Binding.TransformBindingList.SingleOrDefault(item =>
                string.Equals(item.Id, request.TransformBindingId, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"TransformBinding '{request.TransformBindingId}' was not found.");
        if (!string.Equals(binding.MetaTransformScriptTransformScriptId, transform.Id, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"TransformBinding '{binding.Id}' does not bind TransformScript '{transform.Id}'.");
        }

        var statementLink = request.Transforms.TransformScriptStatementLinkList.SingleOrDefault(item =>
                string.Equals(item.TransformScript.Id, transform.Id, StringComparison.Ordinal))
            ?? throw Unsupported("exactly one statement");
        var select = request.Transforms.SelectStatementList.SingleOrDefault(item =>
                string.Equals(
                    item.StatementWithCtesAndXmlNamespaces.TSqlStatement.Id,
                    statementLink.TSqlStatement.Id,
                    StringComparison.Ordinal))
            ?? throw Unsupported("a SELECT statement");
        var queryLink = request.Transforms.SelectStatementQueryExpressionLinkList.Single(item =>
            string.Equals(item.SelectStatement.Id, select.Id, StringComparison.Ordinal));
        var query = request.Transforms.QuerySpecificationList.SingleOrDefault(item =>
                string.Equals(item.QueryExpression.Id, queryLink.QueryExpression.Id, StringComparison.Ordinal))
            ?? throw Unsupported("one query specification");

        var from = request.Transforms.QuerySpecificationFromClauseLinkList.SingleOrDefault(item =>
                string.Equals(item.QuerySpecification.Id, query.Id, StringComparison.Ordinal))
            ?? throw Unsupported("one FROM clause");
        var tableReference = request.Transforms.FromClauseTableReferencesItemList
            .Where(item => string.Equals(item.FromClause.Id, from.FromClause.Id, StringComparison.Ordinal))
            .Select(item => item.TableReference)
            .SingleOrDefault()
            ?? throw Unsupported("one table source");
        var namedTable = request.Transforms.NamedTableReferenceList.SingleOrDefault(item =>
                string.Equals(item.TableReferenceWithAlias.TableReference.Id, tableReference.Id, StringComparison.Ordinal))
            ?? throw Unsupported("a named table source");
        var boundTable = request.Binding.TableSourceList.SingleOrDefault(item =>
                string.Equals(item.TransformBinding.Id, binding.Id, StringComparison.Ordinal) &&
                string.Equals(item.MetaTransformScriptTableReferenceId, tableReference.Id, StringComparison.Ordinal))
            ?? throw new InvalidOperationException("The selected table source has no binding evidence.");
        var sourceObject = ResolveSchemaObject(request.Schema, boundTable.Rowset.SqlIdentifier ?? string.Empty);
        var sourcePath = Path.Combine(root, sourceObject.Name + ".tsv");
        var headers = ReadHeaders(sourcePath);

        var selectItems = request.Transforms.QuerySpecificationSelectElementsItemList
            .Where(item => string.Equals(item.QuerySpecification.Id, query.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .ToArray();
        var valueSelectors = selectItems
            .Select(item => CompileSelectElement(request, binding, item.SelectElement))
            .ToArray();

        var outputRowset = request.Binding.OutputRowsetList.SingleOrDefault(item =>
                string.Equals(item.TransformBinding.Id, binding.Id, StringComparison.Ordinal))
            ?? throw new InvalidOperationException("The selected binding has no output rowset.");
        var boundShape = new PipelineRowStreamShape(
            request.Binding.ColumnList
                .Where(item => string.Equals(item.Rowset.Id, outputRowset.Rowset.Id, StringComparison.Ordinal))
                .OrderBy(item => ParseOrdinal(item.Ordinal))
                .Select((item, ordinal) => new PipelineColumn(item.Name, ordinal))
                .ToArray());
        request.OutputShape.EnsureCompatibleWith(boundShape, "bound transform output");

        Func<IReadOnlyDictionary<string, string?>, bool> predicate = static _ => true;
        var whereLink = request.Transforms.QuerySpecificationWhereClauseLinkList.SingleOrDefault(item =>
            string.Equals(item.QuerySpecification.Id, query.Id, StringComparison.Ordinal));
        if (whereLink is not null)
        {
            var condition = request.Transforms.WhereClauseSearchConditionLinkList.Single(item =>
                string.Equals(item.WhereClause.Id, whereLink.WhereClause.Id, StringComparison.Ordinal));
            predicate = CompilePredicate(request, binding, condition.BooleanExpression);
        }

        return new ProjectionPlan(sourcePath, headers, valueSelectors, predicate);
    }

    private static Func<IReadOnlyDictionary<string, string?>, object?> CompileSelectElement(
        MetaSchemaRowStreamRequest request,
        TransformBinding binding,
        SelectElement selectElement)
    {
        var scalar = request.Transforms.SelectScalarExpressionList.SingleOrDefault(item =>
                string.Equals(item.SelectElement.Id, selectElement.Id, StringComparison.Ordinal))
            ?? throw Unsupported("scalar select elements");
        var expression = request.Transforms.SelectScalarExpressionExpressionLinkList.Single(item =>
            string.Equals(item.SelectScalarExpression.Id, scalar.Id, StringComparison.Ordinal));
        return CompileScalar(request, binding, expression.ScalarExpression);
    }

    private static Func<IReadOnlyDictionary<string, string?>, bool> CompilePredicate(
        MetaSchemaRowStreamRequest request,
        TransformBinding binding,
        BooleanExpression expression)
    {
        var comparison = request.Transforms.BooleanComparisonExpressionList.SingleOrDefault(item =>
                string.Equals(item.BooleanExpression.Id, expression.Id, StringComparison.Ordinal))
            ?? throw Unsupported("an equality predicate");
        if (!string.Equals(comparison.ComparisonType, "Equals", StringComparison.Ordinal))
        {
            throw Unsupported("an equality predicate");
        }

        var first = request.Transforms.BooleanComparisonExpressionFirstExpressionLinkList.Single(item =>
            string.Equals(item.BooleanComparisonExpression.Id, comparison.Id, StringComparison.Ordinal));
        var second = request.Transforms.BooleanComparisonExpressionSecondExpressionLinkList.Single(item =>
            string.Equals(item.BooleanComparisonExpression.Id, comparison.Id, StringComparison.Ordinal));
        var firstValue = CompileScalar(request, binding, first.ScalarExpression);
        var secondValue = CompileScalar(request, binding, second.ScalarExpression);
        return row => string.Equals(
            Convert.ToString(firstValue(row), CultureInfo.InvariantCulture),
            Convert.ToString(secondValue(row), CultureInfo.InvariantCulture),
            StringComparison.Ordinal);
    }

    private static Func<IReadOnlyDictionary<string, string?>, object?> CompileScalar(
        MetaSchemaRowStreamRequest request,
        TransformBinding binding,
        ScalarExpression expression)
    {
        var primary = request.Transforms.PrimaryExpressionList.SingleOrDefault(item =>
                string.Equals(item.ScalarExpression.Id, expression.Id, StringComparison.Ordinal))
            ?? throw Unsupported("column and literal scalar expressions");
        var columnReference = request.Transforms.ColumnReferenceExpressionList.SingleOrDefault(item =>
            string.Equals(item.PrimaryExpression.Id, primary.Id, StringComparison.Ordinal));
        if (columnReference is not null)
        {
            var boundColumn = request.Binding.ColumnReferenceList.SingleOrDefault(item =>
                    string.Equals(item.TransformBinding.Id, binding.Id, StringComparison.Ordinal) &&
                    string.Equals(item.MetaTransformScriptColumnReferenceId, columnReference.Id, StringComparison.Ordinal))
                ?? throw new InvalidOperationException(
                    $"ColumnReferenceExpression '{columnReference.Id}' has no binding evidence.");
            return row => row[boundColumn.Column.Name];
        }

        var literal = request.Transforms.LiteralList.SingleOrDefault(item =>
            string.Equals(item.ValueExpression.PrimaryExpression.Id, primary.Id, StringComparison.Ordinal));
        if (literal is not null)
        {
            return _ => literal.Value;
        }

        throw Unsupported("column and literal scalar expressions");
    }

    private SchemaObject ResolveSchemaObject(MetaSchemaModel schema, string identifier)
    {
        var parts = identifier
            .Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(Unquote)
            .ToArray();
        if (parts.Length is < 1 or > 3)
        {
            throw new InvalidOperationException($"Schema object identifier '{identifier}' is not supported.");
        }

        var matches = schema.SchemaObjectList.Where(item =>
        {
            if (!string.Equals(item.Name, parts[^1], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (parts.Length >= 2 && !string.Equals(item.Schema.Name, parts[^2], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return parts.Length < 3 ||
                   string.Equals(item.Schema.System.Name, parts[^3], StringComparison.OrdinalIgnoreCase);
        }).ToArray();

        return matches.Length switch
        {
            1 => matches[0],
            0 => throw new InvalidOperationException($"Schema object '{identifier}' was not found in MetaSchema."),
            _ => throw new InvalidOperationException($"Schema object '{identifier}' is ambiguous in MetaSchema.")
        };
    }

    private string ResolveRoot(string connectionReference)
    {
        if (!rootsByConnectionReference.TryGetValue(connectionReference, out var root))
        {
            throw new InvalidOperationException($"Connection reference '{connectionReference}' is not configured.");
        }

        return root;
    }

    private static string[] ReadHeaders(string path)
    {
        var firstLine = File.ReadLines(path).FirstOrDefault()
            ?? throw new InvalidOperationException($"Tab-separated file '{path}' has no header row.");
        var headers = Split(firstLine);
        if (headers.Length == 0 || headers.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidOperationException($"Tab-separated file '{path}' has an invalid header row.");
        }

        var duplicates = headers
            .GroupBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .Where(static group => group.Count() > 1)
            .Select(static group => group.Key)
            .ToArray();
        if (duplicates.Length > 0)
        {
            throw new InvalidOperationException(
                $"Tab-separated file '{path}' has duplicate columns: {string.Join(", ", duplicates)}.");
        }

        return headers;
    }

    private static string[] Split(string line) => line.Split('\t');

    private static int ParseOrdinal(string? value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ordinal)
            ? ordinal
            : int.MaxValue;

    private static string NormalizeIdPart(string value) =>
        new(value.Trim().Select(character => char.IsLetterOrDigit(character) ? character : '_').ToArray());

    private static string Unquote(string value)
    {
        var trimmed = value.Trim();
        return trimmed.Length >= 2 && trimmed[0] == '[' && trimmed[^1] == ']'
            ? trimmed[1..^1]
            : trimmed;
    }

    private static NotSupportedException Unsupported(string supportedShape) =>
        new($"The tab-separated text witness adapter supports {supportedShape}.");

    private sealed record ProjectionPlan(
        string SourcePath,
        IReadOnlyList<string> Headers,
        IReadOnlyList<Func<IReadOnlyDictionary<string, string?>, object?>> ValueSelectors,
        Func<IReadOnlyDictionary<string, string?>, bool> Predicate);

    private sealed class TabSeparatedTransformSource(
        ProjectionPlan plan,
        PipelineRowStreamShape shape,
        int batchSize) : IPipelineRowStreamSource
    {
        public PipelineRowStreamShape Shape { get; } = shape;

        public async IAsyncEnumerable<PipelineDataBatch> ReadBatchesAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var rows = new List<object?[]>(batchSize);
            foreach (var line in File.ReadLines(plan.SourcePath).Skip(1))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var values = Split(line);
                if (values.Length != plan.Headers.Count)
                {
                    throw new InvalidOperationException(
                        $"Tab-separated row has {values.Length} values but {plan.Headers.Count} columns were discovered.");
                }

                var row = plan.Headers
                    .Select((header, ordinal) => (header, value: (string?)values[ordinal]))
                    .ToDictionary(item => item.header, item => item.value, StringComparer.OrdinalIgnoreCase);
                if (!plan.Predicate(row))
                {
                    continue;
                }

                rows.Add(plan.ValueSelectors.Select(selector => selector(row)).ToArray());
                if (rows.Count < batchSize)
                {
                    continue;
                }

                yield return new PipelineDataBatch(Shape, rows.ToArray());
                rows.Clear();
                await Task.Yield();
            }

            if (rows.Count > 0)
            {
                yield return new PipelineDataBatch(Shape, rows.ToArray());
            }
        }
    }

    private sealed class TabSeparatedTargetWriteOperation(
        string path,
        PipelineRowStreamShape shape) : IPipelineTargetWriteOperation
    {
        private StreamWriter? writer;

        public string Name => "TabSeparatedInsertRows";

        public PipelineRowStreamShape Shape { get; } = shape;

        public async ValueTask BeginAsync(CancellationToken cancellationToken = default)
        {
            writer = new StreamWriter(path, append: false);
            await writer.WriteLineAsync(
                string.Join('\t', Shape.Columns.Select(column => column.Name)).AsMemory(),
                cancellationToken);
        }

        public async Task WriteBatchAsync(
            PipelineDataBatch batch,
            CancellationToken cancellationToken = default)
        {
            var activeWriter = writer ?? throw new InvalidOperationException("The target operation has not started.");
            foreach (var row in batch.Rows)
            {
                var line = string.Join('\t', row.Select(value => Convert.ToString(value, CultureInfo.InvariantCulture)));
                await activeWriter.WriteLineAsync(line.AsMemory(), cancellationToken);
            }
        }

        public async ValueTask CompleteAsync(CancellationToken cancellationToken = default)
        {
            if (writer is not null)
            {
                await writer.FlushAsync(cancellationToken);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (writer is not null)
            {
                await writer.DisposeAsync();
                writer = null;
            }
        }
    }
}
