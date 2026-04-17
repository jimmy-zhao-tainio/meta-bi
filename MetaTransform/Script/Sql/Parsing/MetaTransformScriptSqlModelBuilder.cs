namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    private readonly MetaTransformScript.MetaTransformScriptModel model;
    private readonly Dictionary<string, int> nextIdByEntityName = new(StringComparer.Ordinal);

    public MetaTransformScriptSqlModelBuilder()
    {
        model = MetaTransformScript.MetaTransformScriptModel.CreateEmpty();
    }

    public MetaTransformScriptSqlModelBuilder(MetaTransformScript.MetaTransformScriptModel seedModel)
    {
        ArgumentNullException.ThrowIfNull(seedModel);
        model = seedModel;
        InitializeNextIdState(seedModel);
    }

    public MetaTransformScript.MetaTransformScriptModel Build() => model;

    private string NextId(string entityName)
    {
        nextIdByEntityName.TryGetValue(entityName, out var current);
        current++;
        nextIdByEntityName[entityName] = current;
        return $"{entityName}:{current}";
    }

    private void InitializeNextIdState(MetaTransformScript.MetaTransformScriptModel seedModel)
    {
        var modelType = seedModel.GetType();
        var properties = modelType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var property in properties)
        {
            if (!property.Name.EndsWith("List", StringComparison.Ordinal))
            {
                continue;
            }

            if (!typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                continue;
            }

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

        private BuiltNode(IReadOnlyDictionary<string, string> idsByEntityName)
        {
            this.idsByEntityName = idsByEntityName;
        }

        public string GetId(string entityName) =>
            idsByEntityName.TryGetValue(entityName, out var id)
                ? id
                : throw new InvalidOperationException($"Built node did not expose entity id '{entityName}'.");

        public static BuiltNode Create(params (string EntityName, string Id)[] ids) =>
            new(ids.ToDictionary(static pair => pair.EntityName, static pair => pair.Id, StringComparer.Ordinal));
    }
}
