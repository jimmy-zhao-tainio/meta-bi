using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MetaSql.Core;

namespace MetaSql.Workflow;

public sealed class BlockerIdentityService
{
    public IReadOnlyList<Blocker> Build(SqlServerPreflightPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        var blockers = new List<Blocker>();
        blockers.AddRange(plan.ManualRequiredItems.Select(note => CreateBlocker(BlockerKind.ManualRequired, note)));
        blockers.AddRange(plan.BlockedItems.Select(note => CreateBlocker(BlockerKind.Blocked, note)));
        return blockers;
    }

    private static Blocker CreateBlocker(BlockerKind kind, PlanNote note)
    {
        var reasons = note.Reasons.Select(NormalizeText).ToArray();
        var canonical = new StringBuilder();
        canonical.Append(kind.ToString().ToLowerInvariant());
        canonical.Append('\n');
        canonical.Append(NormalizeText(note.ObjectName));
        foreach (var reason in reasons)
        {
            canonical.Append('\n');
            canonical.Append(reason);
        }

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(canonical.ToString()));
        var id = "blk_" + Convert.ToHexString(hash[..8]).ToLowerInvariant();
        return new Blocker(id, kind, note.ObjectName, reasons);
    }

    private static string NormalizeText(string value)
    {
        return Regex.Replace(value.Trim(), @"\s+", " ");
    }
}
