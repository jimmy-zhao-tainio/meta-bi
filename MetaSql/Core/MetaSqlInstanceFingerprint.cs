using System.Security.Cryptography;
using System.Text;
using Meta.Operations.Domain;
using MetaSql.Extractors.SqlServer;

namespace MetaSql;

internal static class MetaSqlInstanceFingerprint
{
    internal static string Compute(InMemoryWorkspace workspace, SqlServerDataTypeCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(catalog);
        var canonical = MetaSqlCanonicalizer.Canonicalize(workspace);
        using var bytes = new MemoryStream();
        using (var writer = new BinaryWriter(bytes, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write("MetaSql.SqlServerInstanceContract.v1");
            writer.Write(canonical.Model.Name);
            var entities = canonical.Instance.RecordsByEntity
                .Where(row => row.Value.Count > 0)
                .OrderBy(row => row.Key, StringComparer.Ordinal).ToArray();
            writer.Write(entities.Length);
            foreach (var entity in entities)
            {
                writer.Write(entity.Key);
                var records = entity.Value.OrderBy(row => row.Id, StringComparer.Ordinal).ToArray();
                writer.Write(records.Length);
                foreach (var record in records)
                {
                    writer.Write(record.Id);
                    WriteDictionary(writer, record.Values);
                    WriteDictionary(writer, record.RelationshipIds);
                }
            }

            var usedIds = canonical.Instance.RecordsByEntity.TryGetValue("TableColumn", out var columns)
                ? columns.Select(row => row.Values.GetValueOrDefault("MetaDataTypeId") ?? string.Empty)
                    .Distinct(StringComparer.Ordinal).OrderBy(id => id, StringComparer.Ordinal).ToArray()
                : Array.Empty<string>();
            writer.Write(usedIds.Length);
            foreach (var id in usedIds)
            {
                writer.Write(id);
                writer.Write(catalog.ResolveId(id, "SQL instance contract").Name);
            }
        }
        return "sqlserver-v1:" + Convert.ToHexString(SHA256.HashData(bytes.ToArray())).ToLowerInvariant();
    }

    private static void WriteDictionary(BinaryWriter writer, IReadOnlyDictionary<string, string> values)
    {
        writer.Write(values.Count);
        foreach (var pair in values.OrderBy(row => row.Key, StringComparer.Ordinal))
        {
            writer.Write(pair.Key);
            writer.Write(pair.Value is not null);
            if (pair.Value is not null) writer.Write(pair.Value);
        }
    }
}
