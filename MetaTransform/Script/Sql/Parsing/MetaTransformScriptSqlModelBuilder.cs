namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    private static readonly System.Reflection.PropertyInfo[] ModelListProperties =
        typeof(MetaTransformScript.MetaTransformScriptModel)
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(static property =>
                property.Name.EndsWith("List", StringComparison.Ordinal) &&
                typeof(System.Collections.IList).IsAssignableFrom(property.PropertyType) &&
                property.GetIndexParameters().Length == 0)
            .ToArray();

    private readonly MetaTransformScript.MetaTransformScriptModel model;
    private readonly Dictionary<string, int> nextIdByEntityName = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ModelRowIndex> rowIndexesByEntityName = new(StringComparer.Ordinal);

    public MetaTransformScriptSqlModelBuilder()
    {
        model = MetaTransformScript.MetaTransformScriptModel.CreateEmpty();
        BuiltNode.SetResolver(ResolveBuiltNodeReference);
    }

    public MetaTransformScriptSqlModelBuilder(MetaTransformScript.MetaTransformScriptModel seedModel)
    {
        ArgumentNullException.ThrowIfNull(seedModel);
        model = seedModel;
        InitializeNextIdState(seedModel);
        BuiltNode.SetResolver(ResolveBuiltNodeReference);
    }

    public MetaTransformScript.MetaTransformScriptModel Build() => model;

    internal Checkpoint CreateCheckpoint()
    {
        var rowCounts = new Dictionary<System.Collections.IList, int>(ReferenceEqualityComparer.Instance);
        foreach (var property in ModelListProperties)
        {
            if (property.GetValue(model) is System.Collections.IList rows)
            {
                rowCounts.Add(rows, rows.Count);
            }
        }

        return new Checkpoint(
            rowCounts,
            new Dictionary<string, int>(nextIdByEntityName, StringComparer.Ordinal));
    }

    internal void Rollback(Checkpoint checkpoint)
    {
        ArgumentNullException.ThrowIfNull(checkpoint);

        foreach (var (rows, count) in checkpoint.RowCounts)
        {
            while (rows.Count > count)
            {
                rows.RemoveAt(rows.Count - 1);
            }
        }

        nextIdByEntityName.Clear();
        foreach (var (entityName, nextId) in checkpoint.NextIdByEntityName)
        {
            nextIdByEntityName.Add(entityName, nextId);
        }

        rowIndexesByEntityName.Clear();
    }

    private string NextId(string entityName)
    {
        nextIdByEntityName.TryGetValue(entityName, out var current);
        current++;
        nextIdByEntityName[entityName] = current;
        return $"{entityName}:{current}";
    }

    private object ResolveBuiltNodeReference(string entityName, string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        if (!rowIndexesByEntityName.TryGetValue(entityName, out var rowIndex))
        {
            var listProperty = model.GetType().GetProperty(entityName + "List")
                ?? throw new InvalidOperationException($"Model does not expose list for '{entityName}'.");
            if (listProperty.GetValue(model) is not System.Collections.IList rows)
            {
                throw new InvalidOperationException($"Model list for '{entityName}' is not readable.");
            }

            rowIndex = new ModelRowIndex(rows);
            rowIndexesByEntityName.Add(entityName, rowIndex);
        }

        return rowIndex.Resolve(entityName, id);
    }

    private sealed class ModelRowIndex(System.Collections.IList rows)
    {
        private readonly Dictionary<string, object> rowsById = new(StringComparer.Ordinal);
        private int indexedCount;

        public object Resolve(string entityName, string id)
        {
            IndexAppendedRows();
            return rowsById.GetValueOrDefault(id)
                ?? throw new InvalidOperationException($"Could not resolve '{entityName}' row '{id}'.");
        }

        private void IndexAppendedRows()
        {
            while (indexedCount < rows.Count)
            {
                var row = rows[indexedCount++];
                if (row is null)
                {
                    continue;
                }

                var rowId = row.GetType().GetProperty("Id")?.GetValue(row) as string;
                if (rowId is not null)
                {
                    rowsById.TryAdd(rowId, row);
                }
            }
        }
    }

    internal sealed record Checkpoint(
        IReadOnlyDictionary<System.Collections.IList, int> RowCounts,
        IReadOnlyDictionary<string, int> NextIdByEntityName);

    private void InitializeNextIdState(MetaTransformScript.MetaTransformScriptModel seedModel)
    {
        foreach (var property in ModelListProperties)
        {
            var rows = property.GetValue(seedModel) as System.Collections.IEnumerable;
            if (rows is null)
            {
                continue;
            }

            foreach (var row in rows)
            {
                var idValue = row?.GetType().GetProperty("Id")?.GetValue(row) as string;
                if (!TryReadEntityId(idValue, out var entityName, out var numericId))
                {
                    continue;
                }

                if (!nextIdByEntityName.TryGetValue(entityName, out var current) || numericId > current)
                {
                    nextIdByEntityName[entityName] = numericId;
                }
            }
        }
    }

    private static bool TryReadEntityId(string? value, out string entityName, out int numericId)
    {
        entityName = string.Empty;
        numericId = 0;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var separator = value.LastIndexOf(':');
        if (separator <= 0 || separator >= value.Length - 1)
        {
            return false;
        }

        var suffix = value[(separator + 1)..];
        if (!int.TryParse(suffix, out numericId))
        {
            return false;
        }

        entityName = value[..separator];
        return !string.IsNullOrWhiteSpace(entityName);
    }

    internal sealed class BuiltNode
    {
        private readonly IReadOnlyDictionary<string, string> idsByEntityName;
        private static readonly System.Threading.AsyncLocal<Func<string, string, object>?> Resolve = new();

        private BuiltNode(IReadOnlyDictionary<string, string> idsByEntityName)
        {
            this.idsByEntityName = idsByEntityName;
        }

        public string GetId(string entityName) =>
            idsByEntityName.TryGetValue(entityName, out var id)
                ? id
                : throw new InvalidOperationException($"Built node did not expose entity id '{entityName}'.");

        public T GetRef<T>(string entityName)
            where T : class
        {
            var id = GetId(entityName);
            var resolver = Resolve.Value;
            if (resolver is null)
            {
                throw new InvalidOperationException("Built node resolver is not configured.");
            }

            return (T)resolver(entityName, id);
        }

        public bool TryGetId(string entityName, out string id) =>
            idsByEntityName.TryGetValue(entityName, out id!);

        public static void SetResolver(Func<string, string, object> resolver)
        {
            Resolve.Value = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public static BuiltNode Create(params (string EntityName, string Id)[] ids) =>
            new(ids.ToDictionary(static pair => pair.EntityName, static pair => pair.Id, StringComparer.Ordinal));
    }
}
