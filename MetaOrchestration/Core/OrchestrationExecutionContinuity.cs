using MO = MetaOrchestration;

namespace MetaOrchestration.Core;

public static class OrchestrationExecutionContinuity
{
    public const string OnSuccess = "OnSuccess";
    public const string OnFailure = "OnFailure";
    public const string Succeeded = "Succeeded";
    public const string Failed = "Failed";
    public const string SkippedBlocked = "SkippedBlocked";
    public const string SkippedConditionNotMet = "SkippedConditionNotMet";

    public static IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> BuildDependencyMap(MO.MetaOrchestrationModel model)
    {
        var predecessorSets = model.TaskAccessProfileList.ToDictionary(
            static item => item.Id,
            static _ => new List<OrchestrationExecutionDependency>(),
            StringComparer.Ordinal);

        foreach (var dependency in model.TaskDependencyList)
        {
            AddDependency(
                predecessorSets,
                dependency.Predecessor.Id,
                dependency.Successor.Id,
                NormalizeDependencyCondition(dependency.DependencyCondition),
                dependency.DependencyKind,
                dependency.Reason);
        }

        foreach (var resolution in model.TaskOrderingResolutionList.Where(static item => string.Equals(item.Status, "Active", StringComparison.OrdinalIgnoreCase)))
        {
            AddDependency(
                predecessorSets,
                resolution.Predecessor.Id,
                resolution.Successor.Id,
                NormalizeDependencyCondition(resolution.DependencyCondition),
                resolution.ResolutionKind,
                resolution.Reason);
        }

        return predecessorSets.ToDictionary(
            static item => item.Key,
            static item => item.Value
                .OrderBy(static dependency => dependency.PredecessorTaskProfileId, StringComparer.Ordinal)
                .ThenBy(static dependency => dependency.Condition, StringComparer.Ordinal)
                .ToArray(),
            StringComparer.Ordinal);
    }

    public static bool TryGetUnsatisfiedDependency(
        MO.PlannedTask plannedTask,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependenciesByTaskProfileId,
        IReadOnlyDictionary<string, string> taskOutcomesByTaskProfileId,
        out OrchestrationExecutionDependency dependency,
        out string skipOutcome,
        out string reason)
    {
        dependency = OrchestrationExecutionDependency.Empty;
        skipOutcome = string.Empty;
        reason = string.Empty;
        if (!dependenciesByTaskProfileId.TryGetValue(plannedTask.TaskAccessProfile.Id, out var dependencies))
        {
            return false;
        }

        foreach (var candidate in dependencies)
        {
            if (!taskOutcomesByTaskProfileId.TryGetValue(candidate.PredecessorTaskProfileId, out var predecessorOutcome))
            {
                dependency = candidate;
                skipOutcome = SkippedBlocked;
                reason = "predecessor has not completed";
                return true;
            }

            if (IsConditionSatisfied(candidate.Condition, predecessorOutcome))
            {
                continue;
            }

            dependency = candidate;
            skipOutcome = GetSkipOutcome(candidate.Condition, predecessorOutcome);
            reason = $"{candidate.Condition} dependency was not satisfied by predecessor outcome {predecessorOutcome}";
            return true;
        }

        return false;
    }

    public static OrchestrationTaskReadiness EvaluateReadiness(
        MO.PlannedTask plannedTask,
        IReadOnlyDictionary<string, OrchestrationExecutionDependency[]> dependenciesByTaskProfileId,
        IReadOnlyDictionary<string, string> taskOutcomesByTaskProfileId,
        out OrchestrationExecutionDependency dependency,
        out string skipOutcome,
        out string reason)
    {
        dependency = OrchestrationExecutionDependency.Empty;
        skipOutcome = string.Empty;
        reason = string.Empty;
        if (!dependenciesByTaskProfileId.TryGetValue(plannedTask.TaskAccessProfile.Id, out var dependencies))
        {
            return OrchestrationTaskReadiness.Ready;
        }

        foreach (var candidate in dependencies)
        {
            if (!taskOutcomesByTaskProfileId.TryGetValue(candidate.PredecessorTaskProfileId, out var predecessorOutcome))
            {
                dependency = candidate;
                reason = "predecessor has not completed";
                return OrchestrationTaskReadiness.Waiting;
            }

            if (IsConditionSatisfied(candidate.Condition, predecessorOutcome))
            {
                continue;
            }

            dependency = candidate;
            skipOutcome = GetSkipOutcome(candidate.Condition, predecessorOutcome);
            reason = $"{candidate.Condition} dependency was not satisfied by predecessor outcome {predecessorOutcome}";
            return OrchestrationTaskReadiness.Skip;
        }

        return OrchestrationTaskReadiness.Ready;
    }

    public static string OutcomeForExitCode(int exitCode) =>
        exitCode == 0 ? Succeeded : Failed;

    public static bool IsFailureOutcome(string outcome) =>
        string.Equals(outcome, Failed, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(outcome, SkippedBlocked, StringComparison.OrdinalIgnoreCase);

    private static void AddDependency(
        IDictionary<string, List<OrchestrationExecutionDependency>> dependenciesByTaskProfileId,
        string predecessorId,
        string successorId,
        string condition,
        string dependencyKind,
        string? reason)
    {
        if (!dependenciesByTaskProfileId.TryGetValue(successorId, out var dependencies))
        {
            return;
        }

        dependencies.Add(new OrchestrationExecutionDependency(
            predecessorId,
            successorId,
            condition,
            dependencyKind,
            string.IsNullOrWhiteSpace(reason) ? null : reason.Trim()));
    }

    private static bool IsConditionSatisfied(string condition, string predecessorOutcome)
    {
        return NormalizeDependencyCondition(condition) switch
        {
            OnSuccess => string.Equals(predecessorOutcome, Succeeded, StringComparison.OrdinalIgnoreCase),
            OnFailure => string.Equals(predecessorOutcome, Failed, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    private static string GetSkipOutcome(string condition, string predecessorOutcome)
    {
        if (string.Equals(predecessorOutcome, SkippedBlocked, StringComparison.OrdinalIgnoreCase))
        {
            return SkippedBlocked;
        }

        if (string.Equals(predecessorOutcome, SkippedConditionNotMet, StringComparison.OrdinalIgnoreCase))
        {
            return SkippedConditionNotMet;
        }

        return NormalizeDependencyCondition(condition) switch
        {
            OnSuccess => SkippedBlocked,
            OnFailure => SkippedConditionNotMet,
            _ => SkippedBlocked
        };
    }

    private static string NormalizeDependencyCondition(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "success" or "succeeded" or "on-success" or "onsuccess" => OnSuccess,
            "failure" or "failed" or "on-failure" or "onfailure" => OnFailure,
            _ => throw new InvalidOperationException($"Unsupported dependency condition '{value}'. Expected success or failure.")
        };
    }
}

public sealed record OrchestrationExecutionDependency(
    string PredecessorTaskProfileId,
    string SuccessorTaskProfileId,
    string Condition,
    string DependencyKind,
    string? Reason)
{
    public static OrchestrationExecutionDependency Empty { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty, null);
}

public enum OrchestrationTaskReadiness
{
    Ready,
    Waiting,
    Skip
}
