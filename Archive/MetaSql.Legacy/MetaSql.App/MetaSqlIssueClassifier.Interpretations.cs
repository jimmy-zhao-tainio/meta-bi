using MetaSql.Workflow.Resolve;

namespace MetaSql.App;

public sealed partial class MetaSqlIssueClassifier
{
    private static IReadOnlyList<ResolverInterpretation> RankInterpretations(ResolverScenario scenario)
    {
        IReadOnlyList<ResolverInterpretation> interpretations = scenario.Family switch
        {
            ResolverScenarioFamily.OneToOneReplacementCandidate => new ResolverInterpretation[]
            {
                new("Rename", RankRenameInterpretation(scenario), BuildRenameReasons(scenario)),
                new("Replace in place", RankReplaceInPlaceInterpretation(scenario), BuildReplaceInPlaceReasons(scenario))
            },

            ResolverScenarioFamily.OneToManySplitCandidate => new ResolverInterpretation[]
            {
                new(
                    "Split one member into several",
                    80,
                    ["One live-only member and multiple desired-only members were found on the same object."])
            },

            ResolverScenarioFamily.ManyToOneMergeCandidate => new ResolverInterpretation[]
            {
                new(
                    "Merge several members into one",
                    80,
                    ["Multiple live-only members and one desired-only member were found on the same object."])
            },

            ResolverScenarioFamily.SameIdentityShapeChange => new ResolverInterpretation[]
            {
                new(
                    "Change the same member in place",
                    85,
                    BuildSameIdentityChangeReasons(scenario))
            },

            ResolverScenarioFamily.LiveOnlyRemovalCandidate => new ResolverInterpretation[]
            {
                new(
                    "Remove the live-only structure",
                    75,
                    ["Live-only structure remains in the target but is not present in desired SQL."])
            },

            ResolverScenarioFamily.RelocationCandidate => new ResolverInterpretation[]
            {
                new(
                    "Move structure across object boundaries",
                    80,
                    ["Observed structure suggests this change may cross object boundaries."])
            },

            ResolverScenarioFamily.ReplaceableStructureChurn => new ResolverInterpretation[]
            {
                new(
                    "Replaceable structure change",
                    85,
                    ["This object is replaceable and its structure changed non-additively."])
            },

            ResolverScenarioFamily.SupportingObjectOnlyChange => new ResolverInterpretation[]
            {
                new(
                    "Supporting-object change",
                    80,
                    ["Only supporting structural objects appear to differ."])
            },

            ResolverScenarioFamily.ComplexComposite => new ResolverInterpretation[]
            {
                new(
                    "Composite structural change",
                    70,
                    ["Several meaningful change patterns are present at once."])
            },

            _ => new ResolverInterpretation[]
            {
                new(
                    "Manual review",
                    50,
                    ["MetaSql could not reduce this to a narrower interpretation."])
            }
        };

        return interpretations
            .OrderByDescending(item => item.Rank)
            .ThenBy(item => item.Label, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<string> BuildDisplayDetails(
        ResolverScenario scenario,
        IReadOnlyList<ResolverInterpretation> interpretations)
    {
        _ = interpretations;
        return scenario.Reasons.ToArray();
    }

    private static int RankRenameInterpretation(ResolverScenario scenario)
    {
        var score = 40;
        if (scenario.Evidence.TypeCompatible) score += 20;
        if (scenario.Evidence.NullabilityCompatible) score += 10;
        score += scenario.Evidence.NameSimilarityPercent / 2;
        if (scenario.Evidence.RelationshipKinds.HasFlag(ResolverRelationshipKinds.PrimaryKey) ||
            scenario.Evidence.RelationshipKinds.HasFlag(ResolverRelationshipKinds.ForeignKey) ||
            scenario.Evidence.RelationshipKinds.HasFlag(ResolverRelationshipKinds.UniqueKey))
        {
            score -= 10;
        }

        if (scenario.Evidence.CrossObjectMovementSuspected) score -= 15;
        return score;
    }

    private static IReadOnlyList<string> BuildRenameReasons(ResolverScenario scenario)
    {
        var reasons = new List<string>
        {
            "One live-only member and one desired-only member were found on the same object."
        };
        if (scenario.Evidence.TypeCompatible)
        {
            reasons.Add("Types are compatible.");
        }

        if (scenario.Evidence.NullabilityCompatible)
        {
            reasons.Add("Nullability is compatible.");
        }

        if (scenario.Evidence.NameSimilarityPercent > 0)
        {
            reasons.Add($"Name similarity is {scenario.Evidence.NameSimilarityPercent}%.");
        }

        return reasons;
    }

    private static int RankReplaceInPlaceInterpretation(ResolverScenario scenario)
    {
        var score = 35;
        if (!scenario.Evidence.TypeCompatible) score += 15;
        if (!scenario.Evidence.NullabilityCompatible) score += 10;
        if (scenario.Evidence.NameSimilarityPercent < 50) score += 10;
        if (scenario.Evidence.RelationshipsPresent == ResolverPresence.Yes) score += 10;
        return score;
    }

    private static IReadOnlyList<string> BuildReplaceInPlaceReasons(ResolverScenario scenario)
    {
        var reasons = new List<string>
        {
            "An old member disappeared and a new member appeared on the same object."
        };
        if (scenario.Evidence.RelationshipsPresent == ResolverPresence.Yes)
        {
            reasons.Add("Known structural relationships are present.");
        }

        if (!scenario.Evidence.TypeCompatible)
        {
            reasons.Add("Types are not compatible.");
        }

        if (!scenario.Evidence.NullabilityCompatible)
        {
            reasons.Add("Nullability is not compatible.");
        }

        return reasons;
    }

    private static IReadOnlyList<string> BuildSameIdentityChangeReasons(ResolverScenario scenario)
    {
        var reasons = new List<string>
        {
            "The same member still exists, but its shape changed in place."
        };
        if (scenario.Evidence.TypeCompatible)
        {
            reasons.Add("The base type stayed compatible.");
        }
        else
        {
            reasons.Add("The base type changed.");
        }

        if (scenario.Evidence.NullabilityCompatible)
        {
            reasons.Add("Nullability stayed compatible.");
        }
        else
        {
            reasons.Add("Nullability changed.");
        }

        return reasons;
    }
}
