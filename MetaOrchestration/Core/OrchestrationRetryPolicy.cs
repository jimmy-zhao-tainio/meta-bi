using System.Globalization;
using MetaOrchestration.WorkerProtocol;
using MO = MetaOrchestration;

namespace MetaOrchestration.Core;

public sealed record ResolvedOrchestrationRetryPolicy(
    string PolicyId,
    string Name,
    int MaxAttempts,
    int InitialDelayMilliseconds,
    int MaxDelayMilliseconds,
    double BackoffMultiplier,
    bool RetryReadOnlyTasksByDefault,
    bool RetryWriteTasksByDefault,
    IReadOnlyCollection<string> RetryableFailureClasses)
{
    public OrchestrationRetryDecision Evaluate(OrchestrationRetryEvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (MaxAttempts <= 1)
        {
            return OrchestrationRetryDecision.DoNotRetry("retry policy allows only one attempt");
        }

        if (context.AttemptNumber >= MaxAttempts)
        {
            return OrchestrationRetryDecision.DoNotRetry("retry budget exhausted");
        }

        if (!IsRetryableFailureClass(context.FailureClass))
        {
            return OrchestrationRetryDecision.DoNotRetry($"failure class '{context.FailureClass}' is not retryable");
        }

        if (!context.IsTaskRetrySafe)
        {
            return OrchestrationRetryDecision.DoNotRetry("task is not retry-safe");
        }

        return OrchestrationRetryDecision.Retry(
            context.AttemptNumber + 1,
            CalculateDelay(context.AttemptNumber),
            $"failure class '{context.FailureClass}' is retryable");
    }

    public static ResolvedOrchestrationRetryPolicy FromRunPlan(MO.MetaOrchestrationModel model, MO.RunPlan runPlan)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(runPlan);

        var assignments = model.RunPlanRetryPolicyList
            .Where(item => ReferenceEquals(item.RunPlan, runPlan))
            .Where(static item => string.Equals(item.PolicyRole, "Default", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (assignments.Length != 1)
        {
            throw new InvalidOperationException(
                $"Run plan '{runPlan.Name}' must have exactly one default RunPlanRetryPolicy assignment, found {assignments.Length}.");
        }

        var policy = assignments[0].RetryPolicy;
        if (!string.Equals(policy.Status, "Active", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Retry policy '{policy.Name}' for run plan '{runPlan.Name}' is not active. Status: {policy.Status}");
        }

        var retryableFailureClasses = model.RetryPolicyFailureClassList
            .Where(item => ReferenceEquals(item.RetryPolicy, policy))
            .Where(static item => string.Equals(item.RetryBehavior, "Retry", StringComparison.OrdinalIgnoreCase))
            .Select(static item => item.FailureClass.Trim())
            .Where(static item => item.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new ResolvedOrchestrationRetryPolicy(
            policy.Id,
            policy.Name,
            ParsePositiveInt(policy.MaxAttempts, "MaxAttempts", policy.Name),
            ParseNonNegativeInt(policy.InitialDelayMilliseconds, "InitialDelayMilliseconds", policy.Name),
            ParseNonNegativeInt(policy.MaxDelayMilliseconds, "MaxDelayMilliseconds", policy.Name),
            ParseBackoffMultiplier(policy.BackoffMultiplier, policy.Name),
            ParseBool(policy.RetryReadOnlyTasksByDefault, "RetryReadOnlyTasksByDefault", policy.Name),
            ParseBool(policy.RetryWriteTasksByDefault, "RetryWriteTasksByDefault", policy.Name),
            retryableFailureClasses);
    }

    public bool IsTaskRetrySafe(bool hasWriteEffect) =>
        hasWriteEffect ? RetryWriteTasksByDefault : RetryReadOnlyTasksByDefault;

    private TimeSpan CalculateDelay(int completedAttemptNumber)
    {
        var exponent = Math.Max(0, completedAttemptNumber - 1);
        var delay = InitialDelayMilliseconds * Math.Pow(Math.Max(1.0, BackoffMultiplier), exponent);
        var bounded = Math.Min(Math.Max(0, delay), Math.Max(0, MaxDelayMilliseconds));
        return TimeSpan.FromMilliseconds(bounded);
    }

    private bool IsRetryableFailureClass(string failureClass) =>
        RetryableFailureClasses.Contains(failureClass, StringComparer.OrdinalIgnoreCase);

    private static int ParsePositiveInt(string value, string propertyName, string policyName)
    {
        var parsed = ParseNonNegativeInt(value, propertyName, policyName);
        if (parsed <= 0)
        {
            throw new InvalidOperationException(
                $"Retry policy '{policyName}' has invalid {propertyName} '{value}'. Value must be greater than zero.");
        }

        return parsed;
    }

    private static int ParseNonNegativeInt(string value, string propertyName, string policyName)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) || parsed < 0)
        {
            throw new InvalidOperationException(
                $"Retry policy '{policyName}' has invalid {propertyName} '{value}'. Value must be a non-negative integer.");
        }

        return parsed;
    }

    private static double ParseBackoffMultiplier(string value, string policyName)
    {
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) || parsed < 1)
        {
            throw new InvalidOperationException(
                $"Retry policy '{policyName}' has invalid BackoffMultiplier '{value}'. Value must be greater than or equal to 1.");
        }

        return parsed;
    }

    private static bool ParseBool(string value, string propertyName, string policyName)
    {
        if (!bool.TryParse(value, out var parsed))
        {
            throw new InvalidOperationException(
                $"Retry policy '{policyName}' has invalid {propertyName} '{value}'. Value must be true or false.");
        }

        return parsed;
    }
}

public sealed record OrchestrationRetryEvaluationContext(
    string TaskId,
    int AttemptNumber,
    string FailureClass,
    bool IsTaskRetrySafe,
    int ExitCode,
    string FailureMessage);

public sealed record OrchestrationRetryDecision(
    bool ShouldRetry,
    int NextAttemptNumber,
    TimeSpan Delay,
    string Reason)
{
    public static OrchestrationRetryDecision Retry(int nextAttemptNumber, TimeSpan delay, string reason) =>
        new(true, nextAttemptNumber, delay, reason);

    public static OrchestrationRetryDecision DoNotRetry(string reason) =>
        new(false, 0, TimeSpan.Zero, reason);
}

public static class OrchestrationRetryFailureClasses
{
    public const string TransientSql = WorkerFailureClasses.TransientSql;
    public const string TransientConnectivity = WorkerFailureClasses.TransientConnectivity;
    public const string WorkerCrashBeforeTerminalEvent = WorkerFailureClasses.WorkerCrashBeforeTerminalEvent;
    public const string HeartbeatTimeout = WorkerFailureClasses.HeartbeatTimeout;
    public const string TaskTimeout = WorkerFailureClasses.TaskTimeout;
    public const string WorkerReportedRetryable = WorkerFailureClasses.WorkerReportedRetryable;
    public const string VersionMismatch = WorkerFailureClasses.VersionMismatch;
    public const string MalformedProtocol = WorkerFailureClasses.MalformedProtocol;
    public const string InvalidWorkspace = WorkerFailureClasses.InvalidWorkspace;
    public const string MissingTaskId = WorkerFailureClasses.MissingTaskId;
    public const string DeterministicModelError = WorkerFailureClasses.DeterministicModelError;
    public const string RetryBudgetExhausted = WorkerFailureClasses.RetryBudgetExhausted;
}
