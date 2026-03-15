using MetaSql.Core;
using MetaSql.Workflow;
using MetaSql.Workflow.Resolve;

namespace MetaSql.App;

public sealed partial class MetaSqlIssueClassifier
{
    private static ResolverScenario BuildScenario(
        Blocker blocker,
        DesiredSqlModel desiredModel,
        LiveDatabaseSnapshot liveSnapshot,
        IReadOnlyDictionary<string, SqlObjectTraits> traitsByObject)
    {
        var objectName = blocker.ObjectName;
        var desiredTable = desiredModel.Tables.FirstOrDefault(
            table => string.Equals(table.ObjectKey, objectName, StringComparison.OrdinalIgnoreCase));
        liveSnapshot.Tables.TryGetValue(objectName, out var liveTable);

        var liveOnlyNames = CollectLiveOnlyNames(desiredTable, liveTable);
        var desiredOnlyNames = CollectDesiredOnlyNames(desiredTable, liveTable);
        var sameIdentityChangedNames = CollectSameIdentityChangedNames(desiredTable, liveTable);
        var subjectKind = ResolveSubjectKind(blocker, desiredTable, liveTable, liveOnlyNames, desiredOnlyNames, sameIdentityChangedNames);
        var stateClass = ResolveStateClass(objectName, traitsByObject);
        var evidence = BuildEvidence(objectName, desiredTable, liveTable, liveOnlyNames, desiredOnlyNames, sameIdentityChangedNames, desiredModel, liveSnapshot);
        var family = ResolveFamily(
            blocker,
            subjectKind,
            stateClass,
            liveOnlyNames.Count,
            desiredOnlyNames.Count,
            sameIdentityChangedNames.Count,
            evidence.CrossObjectMovementSuspected);

        var scenario = new ResolverScenario
        {
            SubjectKind = subjectKind,
            StateClass = stateClass,
            Family = family,
            ObjectName = objectName,
            LiveOnlyCount = liveOnlyNames.Count,
            DesiredOnlyCount = desiredOnlyNames.Count,
            SameIdentityChangedCount = sameIdentityChangedNames.Count,
            LiveOnlyNames = liveOnlyNames,
            DesiredOnlyNames = desiredOnlyNames,
            SameIdentityChangedNames = sameIdentityChangedNames,
            Evidence = evidence,
            Reasons = BuildReasons(blocker, family, desiredTable, liveTable, liveOnlyNames, desiredOnlyNames, sameIdentityChangedNames, evidence)
        };

        scenario.Validate();
        return scenario;
    }

    private static IReadOnlyList<string> CollectLiveOnlyNames(DesiredTable? desiredTable, LiveTable? liveTable)
    {
        if (liveTable == null)
        {
            return [];
        }

        if (desiredTable == null)
        {
            return [liveTable.TableName];
        }

        var desiredNames = desiredTable.Columns
            .Select(column => column.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return liveTable.Columns.Keys
            .Where(name => !desiredNames.Contains(name))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }

    private static IReadOnlyList<string> CollectDesiredOnlyNames(DesiredTable? desiredTable, LiveTable? liveTable)
    {
        if (desiredTable == null)
        {
            return [];
        }

        if (liveTable == null)
        {
            return [desiredTable.TableName];
        }

        return desiredTable.Columns
            .Select(column => column.Name)
            .Where(name => !liveTable.Columns.ContainsKey(name))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }

    private static IReadOnlyList<string> CollectSameIdentityChangedNames(DesiredTable? desiredTable, LiveTable? liveTable)
    {
        if (desiredTable == null || liveTable == null)
        {
            return [];
        }

        return desiredTable.Columns
            .Where(column =>
                liveTable.Columns.TryGetValue(column.Name, out var liveColumn) &&
                (!string.Equals(column.TypeSql, liveColumn.TypeSql, StringComparison.OrdinalIgnoreCase) ||
                 column.IsNullable != liveColumn.IsNullable))
            .Select(column => column.Name)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }

    private static ResolverSubjectKind ResolveSubjectKind(
        Blocker blocker,
        DesiredTable? desiredTable,
        LiveTable? liveTable,
        IReadOnlyList<string> liveOnlyNames,
        IReadOnlyList<string> desiredOnlyNames,
        IReadOnlyList<string> sameIdentityChangedNames)
    {
        if (desiredTable == null || liveTable == null)
        {
            return ResolverSubjectKind.Table;
        }

        if (liveOnlyNames.Count > 0 || desiredOnlyNames.Count > 0 || sameIdentityChangedNames.Count > 0)
        {
            return ResolverSubjectKind.Column;
        }

        var mentionsIndex = blocker.Reasons.Any(reason => reason.Contains("INDEX", StringComparison.OrdinalIgnoreCase));
        var mentionsConstraint = blocker.Reasons.Any(reason => reason.Contains("CONSTRAINT", StringComparison.OrdinalIgnoreCase));
        return (mentionsIndex, mentionsConstraint) switch
        {
            (true, false) => ResolverSubjectKind.Index,
            (false, true) => ResolverSubjectKind.Constraint,
            (true, true) => ResolverSubjectKind.Mixed,
            _ => ResolverSubjectKind.Table
        };
    }

    private static ResolverStateClass ResolveStateClass(
        string objectName,
        IReadOnlyDictionary<string, SqlObjectTraits> traitsByObject)
    {
        if (!traitsByObject.TryGetValue(objectName, out var traits))
        {
            return ResolverStateClass.Persistent;
        }

        return traits.StateClass == SqlObjectStateClass.Replaceable
            ? ResolverStateClass.Replaceable
            : ResolverStateClass.Persistent;
    }

    private static ResolverScenarioEvidence BuildEvidence(
        string objectName,
        DesiredTable? desiredTable,
        LiveTable? liveTable,
        IReadOnlyList<string> liveOnlyNames,
        IReadOnlyList<string> desiredOnlyNames,
        IReadOnlyList<string> sameIdentityChangedNames,
        DesiredSqlModel desiredModel,
        LiveDatabaseSnapshot liveSnapshot)
    {
        var dataPresent = liveTable == null
            ? ResolverPresence.Unknown
            : liveTable.RowCount > 0 ? ResolverPresence.Yes : ResolverPresence.No;

        var relationshipInfo = CollectRelationshipInfo(desiredTable, liveTable);
        var (typeCompatible, nullabilityCompatible, nameSimilarityPercent) = CollectCompatibilitySignals(
            desiredTable,
            liveTable,
            liveOnlyNames,
            desiredOnlyNames,
            sameIdentityChangedNames);

        return new ResolverScenarioEvidence
        {
            DataPresent = dataPresent,
            RelationshipsPresent = relationshipInfo.Kinds == ResolverRelationshipKinds.None ? ResolverPresence.No : ResolverPresence.Yes,
            RelationshipKinds = relationshipInfo.Kinds,
            TypeCompatible = typeCompatible,
            NullabilityCompatible = nullabilityCompatible,
            NameSimilarityPercent = nameSimilarityPercent,
            CrossObjectMovementSuspected = DetectCrossObjectMovement(objectName, liveOnlyNames, desiredOnlyNames, desiredModel, liveSnapshot),
            HistoricalPattern = ResolverHistoricalPattern.Unknown,
            RelationshipCount = relationshipInfo.Count
        };
    }

    private static (ResolverRelationshipKinds Kinds, int Count) CollectRelationshipInfo(DesiredTable? desiredTable, LiveTable? liveTable)
    {
        var kinds = ResolverRelationshipKinds.None;
        var count = 0;

        var indexNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (desiredTable != null)
        {
            foreach (var index in desiredTable.Indexes)
            {
                indexNames.Add(index.Name);
            }
        }

        if (liveTable != null)
        {
            foreach (var indexName in liveTable.IndexNames)
            {
                indexNames.Add(indexName);
            }
        }

        if (indexNames.Count > 0)
        {
            kinds |= ResolverRelationshipKinds.Index;
            count += indexNames.Count;
        }

        var desiredConstraintKindsByName = new Dictionary<string, ResolverRelationshipKinds>(StringComparer.OrdinalIgnoreCase);
        if (desiredTable != null)
        {
            foreach (var constraint in desiredTable.InlineConstraints.Concat(desiredTable.AlterConstraints))
            {
                desiredConstraintKindsByName[constraint.Name] = constraint.ConstraintKind.ToUpperInvariant() switch
                {
                    "PRIMARY KEY" => ResolverRelationshipKinds.PrimaryKey,
                    "UNIQUE" => ResolverRelationshipKinds.UniqueKey,
                    "FOREIGN KEY" => ResolverRelationshipKinds.ForeignKey,
                    "CHECK" => ResolverRelationshipKinds.CheckConstraint,
                    "DEFAULT" => ResolverRelationshipKinds.DefaultConstraint,
                    _ => ResolverRelationshipKinds.Other
                };
            }
        }

        var constraintNames = new HashSet<string>(desiredConstraintKindsByName.Keys, StringComparer.OrdinalIgnoreCase);
        if (liveTable != null)
        {
            foreach (var constraintName in liveTable.ConstraintNames)
            {
                constraintNames.Add(constraintName);
            }
        }

        foreach (var constraintName in constraintNames)
        {
            count++;
            kinds |= desiredConstraintKindsByName.TryGetValue(constraintName, out var constraintKind)
                ? constraintKind
                : ResolverRelationshipKinds.Other;
        }

        return (kinds, count);
    }

    private static (bool TypeCompatible, bool NullabilityCompatible, int NameSimilarityPercent) CollectCompatibilitySignals(
        DesiredTable? desiredTable,
        LiveTable? liveTable,
        IReadOnlyList<string> liveOnlyNames,
        IReadOnlyList<string> desiredOnlyNames,
        IReadOnlyList<string> sameIdentityChangedNames)
    {
        if (desiredTable == null || liveTable == null)
        {
            return (false, false, 0);
        }

        var desiredColumnsByName = desiredTable.Columns.ToDictionary(column => column.Name, StringComparer.OrdinalIgnoreCase);

        if (liveOnlyNames.Count == 1 && desiredOnlyNames.Count == 1 && sameIdentityChangedNames.Count == 0)
        {
            var liveColumn = liveTable.Columns[liveOnlyNames[0]];
            var desiredColumn = desiredColumnsByName[desiredOnlyNames[0]];
            return (
                IsTypeCompatible(desiredColumn, liveColumn),
                desiredColumn.IsNullable == liveColumn.IsNullable,
                ComputeNameSimilarityPercent(liveOnlyNames[0], desiredOnlyNames[0]));
        }

        if (sameIdentityChangedNames.Count > 0 && liveOnlyNames.Count == 0 && desiredOnlyNames.Count == 0)
        {
            var allTypeCompatible = true;
            var allNullabilityCompatible = true;
            foreach (var name in sameIdentityChangedNames)
            {
                var desiredColumn = desiredColumnsByName[name];
                var liveColumn = liveTable.Columns[name];
                allTypeCompatible &= IsTypeCompatible(desiredColumn, liveColumn);
                allNullabilityCompatible &= desiredColumn.IsNullable == liveColumn.IsNullable;
            }

            return (allTypeCompatible, allNullabilityCompatible, 100);
        }

        return (false, false, 0);
    }

    private static bool DetectCrossObjectMovement(
        string objectName,
        IReadOnlyList<string> liveOnlyNames,
        IReadOnlyList<string> desiredOnlyNames,
        DesiredSqlModel desiredModel,
        LiveDatabaseSnapshot liveSnapshot)
    {
        if (liveOnlyNames.Count == 0 && desiredOnlyNames.Count == 0)
        {
            return false;
        }

        var currentLiveOnly = liveOnlyNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var currentDesiredOnly = desiredOnlyNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var desiredTable in desiredModel.Tables)
        {
            if (string.Equals(desiredTable.ObjectKey, objectName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            liveSnapshot.Tables.TryGetValue(desiredTable.ObjectKey, out var otherLiveTable);
            var otherDesiredOnlyNames = CollectDesiredOnlyNames(desiredTable, otherLiveTable);
            if (otherDesiredOnlyNames.Any(name => currentLiveOnly.Contains(name)))
            {
                return true;
            }

            var otherLiveOnlyNames = CollectLiveOnlyNames(desiredTable, otherLiveTable);
            if (otherLiveOnlyNames.Any(name => currentDesiredOnly.Contains(name)))
            {
                return true;
            }
        }

        return false;
    }

    private static ResolverScenarioFamily ResolveFamily(
        Blocker blocker,
        ResolverSubjectKind subjectKind,
        ResolverStateClass stateClass,
        int liveOnlyCount,
        int desiredOnlyCount,
        int sameIdentityChangedCount,
        bool crossObjectMovementSuspected)
    {
        if (stateClass == ResolverStateClass.Replaceable &&
            (liveOnlyCount > 0 || desiredOnlyCount > 0 || sameIdentityChangedCount > 0))
        {
            return ResolverScenarioFamily.ReplaceableStructureChurn;
        }

        if (crossObjectMovementSuspected)
        {
            if (sameIdentityChangedCount == 0 &&
                ((liveOnlyCount > 0 && desiredOnlyCount == 0) || (desiredOnlyCount > 0 && liveOnlyCount == 0)))
            {
                return ResolverScenarioFamily.RelocationCandidate;
            }

            return ResolverScenarioFamily.ComplexComposite;
        }

        if (liveOnlyCount == 0 && desiredOnlyCount > 0 && sameIdentityChangedCount == 0)
        {
            return ResolverScenarioFamily.AdditiveOnly;
        }

        if (liveOnlyCount > 0 && desiredOnlyCount == 0 && sameIdentityChangedCount == 0)
        {
            return ResolverScenarioFamily.LiveOnlyRemovalCandidate;
        }

        if (liveOnlyCount == 1 && desiredOnlyCount == 1 && sameIdentityChangedCount == 0)
        {
            return ResolverScenarioFamily.OneToOneReplacementCandidate;
        }

        if (liveOnlyCount == 1 && desiredOnlyCount > 1 && sameIdentityChangedCount == 0)
        {
            return ResolverScenarioFamily.OneToManySplitCandidate;
        }

        if (liveOnlyCount > 1 && desiredOnlyCount == 1 && sameIdentityChangedCount == 0)
        {
            return ResolverScenarioFamily.ManyToOneMergeCandidate;
        }

        if (sameIdentityChangedCount > 0 && liveOnlyCount == 0 && desiredOnlyCount == 0)
        {
            return ResolverScenarioFamily.SameIdentityShapeChange;
        }

        if (subjectKind is ResolverSubjectKind.Index or ResolverSubjectKind.Constraint or ResolverSubjectKind.Mixed &&
            liveOnlyCount == 0 &&
            desiredOnlyCount == 0 &&
            sameIdentityChangedCount == 0 &&
            blocker.Reasons.Count > 0)
        {
            return ResolverScenarioFamily.SupportingObjectOnlyChange;
        }

        if ((liveOnlyCount > 0 || desiredOnlyCount > 0) && sameIdentityChangedCount > 0)
        {
            return ResolverScenarioFamily.ComplexComposite;
        }

        return ResolverScenarioFamily.Unknown;
    }

    private static IReadOnlyList<string> BuildReasons(
        Blocker blocker,
        ResolverScenarioFamily family,
        DesiredTable? desiredTable,
        LiveTable? liveTable,
        IReadOnlyList<string> liveOnlyNames,
        IReadOnlyList<string> desiredOnlyNames,
        IReadOnlyList<string> sameIdentityChangedNames,
        ResolverScenarioEvidence evidence)
    {
        var reasons = new List<string>();

        if (liveTable == null && desiredTable != null)
        {
            reasons.Add($"Live table [{desiredTable.SchemaName}].[{desiredTable.TableName}] is missing.");
        }
        else if (desiredTable == null && liveTable != null)
        {
            reasons.Add($"Live table [{liveTable.SchemaName}].[{liveTable.TableName}] exists only in the live DB.");
        }
        else
        {
            if (liveOnlyNames.Count > 0)
            {
                reasons.Add($"In the live DB only: {FormatNameList(liveOnlyNames)}.");
            }

            if (desiredOnlyNames.Count > 0)
            {
                reasons.Add($"In desired SQL only: {FormatNameList(desiredOnlyNames)}.");
            }

            if (sameIdentityChangedNames.Count > 0)
            {
                reasons.Add($"Changed in place: {FormatNameList(sameIdentityChangedNames)}.");
            }
        }

        if (evidence.DataPresent == ResolverPresence.Yes)
        {
            reasons.Add("The live table has rows.");
        }
        else if (evidence.DataPresent == ResolverPresence.No)
        {
            reasons.Add("The live table is empty.");
        }

        if (evidence.RelationshipsPresent == ResolverPresence.Yes)
        {
            reasons.Add($"Known structural relationships are present: {FormatRelationshipKinds(evidence.RelationshipKinds)}.");
        }

        if (reasons.Count == 0)
        {
            reasons.AddRange(blocker.Reasons);
        }

        reasons.Add(BuildUncertaintyLine(family));

        return reasons;
    }

    private static string BuildUncertaintyLine(ResolverScenarioFamily family)
    {
        return family switch
        {
            ResolverScenarioFamily.LiveOnlyRemovalCandidate =>
                "MetaSql cannot decide automatically whether this live-only structure should be removed, preserved, or handled another way.",
            ResolverScenarioFamily.OneToOneReplacementCandidate =>
                "MetaSql cannot decide automatically whether this is a rename, a replacement, or another manual change.",
            ResolverScenarioFamily.OneToManySplitCandidate =>
                "MetaSql cannot decide automatically whether this is a split or another manual change.",
            ResolverScenarioFamily.ManyToOneMergeCandidate =>
                "MetaSql cannot decide automatically whether this is a merge or another manual change.",
            ResolverScenarioFamily.SameIdentityShapeChange =>
                "MetaSql cannot decide automatically how this in-place shape change should be handled.",
            ResolverScenarioFamily.RelocationCandidate =>
                "MetaSql cannot decide automatically whether this structure moved somewhere else or needs another manual change.",
            ResolverScenarioFamily.SupportingObjectOnlyChange =>
                "MetaSql cannot decide automatically how these supporting-object changes should be handled.",
            ResolverScenarioFamily.ReplaceableStructureChurn =>
                "This object is replaceable, but MetaSql still does not invent replacement SQL when rows or other uncertainty are present.",
            ResolverScenarioFamily.ComplexComposite =>
                "MetaSql cannot decide automatically because several change patterns are happening at once.",
            _ =>
                "MetaSql cannot decide automatically what change is intended."
        };
    }

    private static string FormatRelationshipKinds(ResolverRelationshipKinds kinds)
    {
        if (kinds == ResolverRelationshipKinds.None)
        {
            return "none";
        }

        var labels = new List<string>();
        if (kinds.HasFlag(ResolverRelationshipKinds.Index)) labels.Add("indexes");
        if (kinds.HasFlag(ResolverRelationshipKinds.PrimaryKey)) labels.Add("primary keys");
        if (kinds.HasFlag(ResolverRelationshipKinds.UniqueKey)) labels.Add("unique keys");
        if (kinds.HasFlag(ResolverRelationshipKinds.ForeignKey)) labels.Add("foreign keys");
        if (kinds.HasFlag(ResolverRelationshipKinds.CheckConstraint)) labels.Add("check constraints");
        if (kinds.HasFlag(ResolverRelationshipKinds.DefaultConstraint)) labels.Add("default constraints");
        if (kinds.HasFlag(ResolverRelationshipKinds.Other)) labels.Add("other constraints");
        return string.Join(", ", labels);
    }

    private static bool IsTypeCompatible(DesiredColumn desiredColumn, LiveColumn liveColumn)
    {
        return ExtractBaseType(desiredColumn.TypeSql) == ExtractBaseType(liveColumn.TypeSql);
    }

    private static string ExtractBaseType(string sqlType)
    {
        var marker = sqlType.IndexOf('(');
        return (marker >= 0 ? sqlType[..marker] : sqlType).Trim().ToLowerInvariant();
    }

    private static int ComputeNameSimilarityPercent(string left, string right)
    {
        var normalizedLeft = NormalizeName(left);
        var normalizedRight = NormalizeName(right);
        if (normalizedLeft == normalizedRight)
        {
            return 100;
        }

        if (normalizedLeft.Length < 2 || normalizedRight.Length < 2)
        {
            return 0;
        }

        var leftBigrams = BuildBigrams(normalizedLeft);
        var rightBigrams = BuildBigrams(normalizedRight);
        var overlap = leftBigrams.Intersect(rightBigrams).Count();
        var total = leftBigrams.Count + rightBigrams.Count;
        return total == 0 ? 0 : (int)Math.Round((2d * overlap / total) * 100d);
    }

    private static string NormalizeName(string value)
    {
        return new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
    }

    private static IReadOnlyList<string> BuildBigrams(string value)
    {
        var bigrams = new List<string>(Math.Max(0, value.Length - 1));
        for (var index = 0; index < value.Length - 1; index++)
        {
            bigrams.Add(value.Substring(index, 2));
        }

        return bigrams;
    }

    private static string FormatNameList(IReadOnlyList<string> names)
    {
        return string.Join(", ", names.Select(name => $"[{name}]"));
    }
}
