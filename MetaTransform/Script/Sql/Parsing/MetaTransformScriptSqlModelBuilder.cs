using MTS = global::MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    private readonly MetaTransformScript.MetaTransformScriptModel model;
    private readonly Dictionary<string, int> nextIdByEntityName = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IModelRowIndex> rowIndexesByEntityName = new(StringComparer.Ordinal);

    public MetaTransformScriptSqlModelBuilder()
    {
        model = MetaTransformScript.MetaTransformScriptModel.CreateEmpty();
        MTS.MetaTransformScriptModelAccess.VisitCollections(new InitializeCollections(this, initializeIds: false));
        BuiltNode.SetResolver(ResolveBuiltNodeReference);
    }

    public MetaTransformScriptSqlModelBuilder(MetaTransformScript.MetaTransformScriptModel seedModel)
    {
        ArgumentNullException.ThrowIfNull(seedModel);
        model = seedModel;
        MTS.MetaTransformScriptModelAccess.VisitCollections(new InitializeCollections(this, initializeIds: true));
        BuiltNode.SetResolver(ResolveBuiltNodeReference);
    }

    public MetaTransformScript.MetaTransformScriptModel Build() => model;

    internal Checkpoint CreateCheckpoint()
    {
        var rowCounts = rowIndexesByEntityName.Values.ToDictionary(index => index, index => index.Count);

        return new Checkpoint(
            rowCounts,
            new Dictionary<string, int>(nextIdByEntityName, StringComparer.Ordinal));
    }

    internal void Rollback(Checkpoint checkpoint)
    {
        ArgumentNullException.ThrowIfNull(checkpoint);

        foreach (var (rows, count) in checkpoint.RowCounts)
        {
            rows.Rollback(count);
        }

        nextIdByEntityName.Clear();
        foreach (var (entityName, nextId) in checkpoint.NextIdByEntityName)
        {
            nextIdByEntityName.Add(entityName, nextId);
        }
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
            throw new InvalidOperationException($"Model does not expose list for '{entityName}'.");
        }

        return rowIndex.Resolve(entityName, id);
    }

    internal interface IModelRowIndex
    {
        int Count { get; }
        object Resolve(string entityName, string id);
        void Rollback(int count);
    }

    private sealed class ModelRowIndex<TEntity>(IList<TEntity> rows, Func<TEntity, string> readId) : IModelRowIndex
        where TEntity : class
    {
        private readonly Dictionary<string, object> rowsById = new(StringComparer.Ordinal);
        private int indexedCount;
        public int Count => rows.Count;

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

                var rowId = readId(row);
                if (rowId is not null)
                {
                    rowsById.TryAdd(rowId, row);
                }
            }
        }

        public void Rollback(int count)
        {
            while (rows.Count > count) rows.RemoveAt(rows.Count - 1);
            rowsById.Clear();
            indexedCount = 0;
        }
    }

    internal sealed record Checkpoint(
        IReadOnlyDictionary<IModelRowIndex, int> RowCounts,
        IReadOnlyDictionary<string, int> NextIdByEntityName);

    private sealed class InitializeCollections(MetaTransformScriptSqlModelBuilder owner, bool initializeIds)
        : MTS.IMetaTransformScriptCollectionVisitor
    {
        public void Visit<TEntity>(
            string entityName,
            Func<MTS.MetaTransformScriptModel, IList<TEntity>> collection,
            Func<TEntity, string> readId,
            Action<TEntity, string> writeId)
            where TEntity : class
        {
            var rows = collection(owner.model);
            owner.rowIndexesByEntityName.Add(entityName, new ModelRowIndex<TEntity>(rows, readId));
            if (!initializeIds) return;
            foreach (var row in rows)
            {
                var idValue = row is null ? null : readId(row);
                if (!TryReadEntityId(idValue, out var idEntityName, out var numericId))
                {
                    continue;
                }

                if (!owner.nextIdByEntityName.TryGetValue(idEntityName, out var current) || numericId > current)
                {
                    owner.nextIdByEntityName[idEntityName] = numericId;
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
