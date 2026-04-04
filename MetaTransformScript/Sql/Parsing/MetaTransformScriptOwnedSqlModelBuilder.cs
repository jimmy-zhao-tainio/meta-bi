namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptOwnedSqlModelBuilder
{
    private readonly MetaTransformScript.MetaTransformScriptModel model = MetaTransformScript.MetaTransformScriptModel.CreateEmpty();
    private readonly Dictionary<string, int> nextIdByEntityName = new(StringComparer.Ordinal);

    public MetaTransformScript.MetaTransformScriptModel Build() => model;

    private string NextId(string entityName)
    {
        nextIdByEntityName.TryGetValue(entityName, out var current);
        current++;
        nextIdByEntityName[entityName] = current;
        return $"{entityName}:{current}";
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
