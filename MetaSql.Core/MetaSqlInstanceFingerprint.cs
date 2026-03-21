using System.Security.Cryptography;
using System.Text;
using Meta.Core.Domain;

namespace MetaSql;

internal static class MetaSqlInstanceFingerprint
{
    internal static string Compute(Workspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        var builder = new StringBuilder();
        foreach (var entityName in workspace.Instance.RecordsByEntity.Keys.OrderBy(row => row, StringComparer.Ordinal))
        {
            var records = workspace.Instance.GetOrCreateEntityRecords(entityName)
                .OrderBy(row => row.Id, StringComparer.Ordinal);
            foreach (var record in records)
            {
                builder.Append("E|").Append(entityName).Append('|').Append(record.Id).Append('\n');
                foreach (var key in record.Values.Keys.OrderBy(row => row, StringComparer.Ordinal))
                {
                    builder.Append("V|").Append(key).Append('|').Append(record.Values[key]).Append('\n');
                }

                foreach (var key in record.RelationshipIds.Keys.OrderBy(row => row, StringComparer.Ordinal))
                {
                    builder.Append("R|").Append(key).Append('|').Append(record.RelationshipIds[key]).Append('\n');
                }
            }
        }

        var bytes = Encoding.UTF8.GetBytes(builder.ToString());
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
