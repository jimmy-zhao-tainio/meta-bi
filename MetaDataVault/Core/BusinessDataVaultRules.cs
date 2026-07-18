using MetaBusinessDataVault;

namespace MetaDataVault.Core;

public static class BusinessDataVaultRules
{
    public static IReadOnlyList<BusinessHubKeyPart> GetHubKeyPartChain(
        BusinessHub hub,
        IEnumerable<BusinessHubKeyPart> keyParts)
    {
        ArgumentNullException.ThrowIfNull(hub);

        return GetKeyPartChain(
            "Business hub",
            hub,
            keyParts,
            row => row.Id,
            row => row.BusinessHub,
            row => row.PreviousKeyPart);
    }

    public static IReadOnlyList<BusinessReferenceKeyPart> GetReferenceKeyPartChain(
        BusinessReference reference,
        IEnumerable<BusinessReferenceKeyPart> keyParts)
    {
        ArgumentNullException.ThrowIfNull(reference);

        return GetKeyPartChain(
            "Business reference",
            reference,
            keyParts,
            row => row.Id,
            row => row.BusinessReference,
            row => row.PreviousKeyPart);
    }

    public static void ValidateLinkRoleNames(MetaBusinessDataVaultModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        foreach (var role in model.BusinessLinkRoleList)
        {
            if (string.IsNullOrWhiteSpace(role.Name))
            {
                throw new InvalidOperationException($"Business link role '{role.Id}' requires a name.");
            }
        }

        foreach (var group in model.BusinessLinkRoleList.GroupBy(role => role.BusinessLink))
        {
            var duplicate = group
                .GroupBy(role => role.Name, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(names => names.Count() > 1);
            if (duplicate is not null)
            {
                throw new InvalidOperationException(
                    $"Business link '{group.Key.Id}' already has a role named '{duplicate.Key}'.");
            }
        }
    }

    public static IReadOnlyList<BusinessBridgeTraversal> GetBridgeTraversalChain(
        BusinessBridge bridge,
        IEnumerable<BusinessBridgeTraversal> traversals)
    {
        ArgumentNullException.ThrowIfNull(bridge);
        ArgumentNullException.ThrowIfNull(traversals);

        var rows = traversals.ToList();
        if (rows.Count == 0)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Id}' requires at least one traversal.");
        }

        foreach (var traversal in rows)
        {
            if (!ReferenceEquals(traversal.BusinessBridge, bridge))
            {
                throw new InvalidOperationException(
                    $"Bridge traversal '{traversal.Id}' does not belong to bridge '{bridge.Id}'.");
            }

            if (ReferenceEquals(traversal.SourceRole, traversal.TargetRole))
            {
                throw new InvalidOperationException(
                    $"Bridge traversal '{traversal.Id}' must use distinct source and target roles.");
            }

            if (!ReferenceEquals(traversal.SourceRole.BusinessLink, traversal.TargetRole.BusinessLink))
            {
                throw new InvalidOperationException(
                    $"Bridge traversal '{traversal.Id}' must use source and target roles from the same business link.");
            }
        }

        foreach (var previous in rows
            .Where(row => row.PreviousTraversal is not null)
            .Select(row => row.PreviousTraversal!))
        {
            if (!rows.Any(row => ReferenceEquals(row, previous)))
            {
                throw new InvalidOperationException(
                    $"Bridge traversal '{previous.Id}' is not part of bridge '{bridge.Id}'.");
            }
        }

        var startingRows = rows.Where(row => row.PreviousTraversal is null).ToList();
        if (startingRows.Count != 1)
        {
            throw new InvalidOperationException(
                $"Bridge '{bridge.Id}' must have exactly one starting traversal.");
        }

        foreach (var predecessor in rows)
        {
            var successorCount = rows.Count(row => ReferenceEquals(row.PreviousTraversal, predecessor));
            if (successorCount > 1)
            {
                throw new InvalidOperationException(
                    $"Bridge '{bridge.Id}' branches after traversal '{predecessor.Id}'.");
            }
        }

        var ordered = new List<BusinessBridgeTraversal>(rows.Count);
        var current = startingRows[0];
        while (true)
        {
            if (ordered.Any(row => ReferenceEquals(row, current)))
            {
                throw new InvalidOperationException($"Bridge '{bridge.Id}' contains a traversal cycle.");
            }

            if (ordered.Count == 0)
            {
                if (!ReferenceEquals(current.SourceRole.BusinessHub, bridge.BusinessHub))
                {
                    throw new InvalidOperationException(
                        $"Bridge '{bridge.Id}' must start from its anchor hub '{bridge.BusinessHub.Id}'.");
                }
            }
            else if (!ReferenceEquals(ordered[^1].TargetRole.BusinessHub, current.SourceRole.BusinessHub))
            {
                throw new InvalidOperationException(
                    $"Bridge '{bridge.Id}' traversal '{current.Id}' does not continue from the preceding target hub.");
            }

            ordered.Add(current);

            var successor = rows.SingleOrDefault(row => ReferenceEquals(row.PreviousTraversal, current));
            if (successor is null)
            {
                break;
            }

            current = successor;
        }

        if (ordered.Count != rows.Count)
        {
            throw new InvalidOperationException(
                $"Bridge '{bridge.Id}' contains disconnected traversals.");
        }

        return ordered;
    }

    private static IReadOnlyList<TPart> GetKeyPartChain<TParent, TPart>(
        string parentKind,
        TParent parent,
        IEnumerable<TPart> keyParts,
        Func<TParent, string> parentIdSelector,
        Func<TPart, TParent> parentSelector,
        Func<TPart, TPart?> previousSelector)
        where TParent : class
        where TPart : class
    {
        ArgumentNullException.ThrowIfNull(keyParts);

        var rows = keyParts.ToList();
        if (rows.Count == 0)
        {
            return rows;
        }

        foreach (var keyPart in rows)
        {
            if (!ReferenceEquals(parentSelector(keyPart), parent))
            {
                throw new InvalidOperationException(
                    $"{parentKind} '{parentIdSelector(parent)}' contains a key part from a different parent.");
            }
        }

        foreach (var previous in rows
            .Select(previousSelector)
            .Where(previous => previous is not null)
            .Cast<TPart>())
        {
            if (!rows.Any(row => ReferenceEquals(row, previous)))
            {
                throw new InvalidOperationException(
                    $"{parentKind} '{parentIdSelector(parent)}' references a key-part predecessor outside its key.");
            }
        }

        var startingRows = rows.Where(row => previousSelector(row) is null).ToList();
        if (startingRows.Count != 1)
        {
            throw new InvalidOperationException(
                $"{parentKind} '{parentIdSelector(parent)}' must have exactly one starting key part.");
        }

        foreach (var predecessor in rows)
        {
            var successorCount = rows.Count(row => ReferenceEquals(previousSelector(row), predecessor));
            if (successorCount > 1)
            {
                throw new InvalidOperationException(
                    $"{parentKind} '{parentIdSelector(parent)}' key-part precedence branches.");
            }
        }

        var ordered = new List<TPart>(rows.Count);
        var current = startingRows[0];
        while (true)
        {
            if (ordered.Any(row => ReferenceEquals(row, current)))
            {
                throw new InvalidOperationException(
                    $"{parentKind} '{parentIdSelector(parent)}' key-part precedence contains a cycle.");
            }

            ordered.Add(current);

            var successor = rows.SingleOrDefault(row => ReferenceEquals(previousSelector(row), current));
            if (successor is null)
            {
                break;
            }

            current = successor;
        }

        if (ordered.Count != rows.Count)
        {
            throw new InvalidOperationException(
                $"{parentKind} '{parentIdSelector(parent)}' contains disconnected key parts.");
        }

        return ordered;
    }

}
