using MetaSql.App;
using MetaSql.Workflow;
using MetaSql.Workflow.Resolve;
using System.Text;

public sealed class ResolveConsoleInteraction
{
    internal ResolveBlockerAction HandleIssue(MetaSqlResolutionSession session, MetaSqlTargetInspection inspection, MetaSqlIssue issue)
    {
        ArgumentNullException.ThrowIfNull(issue);
        if (issue.Blocker == null)
        {
            throw new InvalidOperationException("resolve can only create scripts for blocker-backed issues.");
        }

        Console.WriteLine(BuildIssuePrompt(inspection, issue));
        while (true)
        {
            Console.Write("> ");
            var input = (Console.ReadLine() ?? string.Empty).Trim();
            switch (input)
            {
                case "1":
                {
                    var path = session.CreateBaselineStub(inspection, issue.Blocker);
                    Console.WriteLine($"Created {path}");
                    Console.WriteLine();
                    return ResolveBlockerAction.CreatedBaselineScript;
                }
                case "2":
                {
                    var path = session.CreateTargetStub(inspection, issue.Blocker);
                    Console.WriteLine($"Created {path}");
                    Console.WriteLine();
                    return ResolveBlockerAction.CreatedTargetScript;
                }
                case "3":
                    Console.WriteLine("Skipped.");
                    Console.WriteLine();
                    return ResolveBlockerAction.Skipped;
                default:
                    Console.WriteLine("Enter 1, 2, or 3.");
                    break;
            }
        }
    }

    internal ResolveStaleScriptAction HandleStaleScript(MetaSqlResolutionSession session, MetaSqlTargetInspection inspection, BlockerScriptFile staleScript)
    {
        Console.WriteLine(BuildStaleScriptPrompt(inspection, staleScript));
        while (true)
        {
            Console.Write("> ");
            var input = (Console.ReadLine() ?? string.Empty).Trim();
            switch (input)
            {
                case "1":
                {
                    var path = session.ArchiveStaleScript(inspection, staleScript);
                    Console.WriteLine($"Archived to {path}");
                    Console.WriteLine();
                    return ResolveStaleScriptAction.Archived;
                }
                case "2":
                    Console.WriteLine("Kept.");
                    Console.WriteLine();
                    return ResolveStaleScriptAction.Kept;
                default:
                    Console.WriteLine("Enter 1 or 2.");
                    break;
            }
        }
    }

    public static string BuildIssuePrompt(MetaSqlTargetInspection inspection, MetaSqlIssue issue)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Issue:");
        builder.Append("    ");
        builder.AppendLine(issue.ObjectName);
        foreach (var detail in issue.Details)
        {
            builder.Append("        ");
            builder.AppendLine(detail);
        }

        if (issue.Scenario != null)
        {
            builder.AppendLine();
            builder.AppendLine("Detected pattern:");
            builder.Append("    ");
            builder.AppendLine(RenderFamilyLabel(issue.Scenario.Family));
        }

        if (issue.Interpretations is { Count: > 0 })
        {
            builder.AppendLine();
            builder.AppendLine("Possible interpretations:");
            foreach (var interpretation in issue.Interpretations.Take(2))
            {
                builder.Append("    ");
                builder.AppendLine(interpretation.Label);
                foreach (var reason in interpretation.Reasons)
                {
                    builder.Append("        ");
                    builder.AppendLine(reason);
                }
            }
        }

        builder.AppendLine();
        builder.AppendLine("Where should the SQL for this change go?");
        builder.AppendLine("  1. Baseline");
        builder.AppendLine($"     Path: {inspection.Context.BaselinePath}");
        builder.AppendLine("     Use this when the same SQL should travel with the normal release path.");
        builder.AppendLine($"  2. Target-specific for {inspection.Context.Name}");
        builder.AppendLine($"     Path: {inspection.Context.TargetPath}");
        builder.AppendLine($"     Use this when only {inspection.Context.Name} currently needs this SQL.");
        builder.AppendLine("  3. Leave it unresolved");
        builder.AppendLine("     deploy-test and deploy will continue to stop here.");
        return builder.ToString().TrimEnd();
    }

    private static string RenderFamilyLabel(ResolverScenarioFamily family)
    {
        return family switch
        {
            ResolverScenarioFamily.LiveOnlyRemovalCandidate => "Live-only structure remains.",
            ResolverScenarioFamily.OneToOneReplacementCandidate => "One live-only member and one desired-only member were found on the same object.",
            ResolverScenarioFamily.OneToManySplitCandidate => "One live-only member and multiple desired-only members were found on the same object.",
            ResolverScenarioFamily.ManyToOneMergeCandidate => "Multiple live-only members and one desired-only member were found on the same object.",
            ResolverScenarioFamily.SameIdentityShapeChange => "The same member still exists, but its shape changed in place.",
            ResolverScenarioFamily.RelocationCandidate => "Observed structure suggests the change may cross object boundaries.",
            ResolverScenarioFamily.SupportingObjectOnlyChange => "Only supporting structural objects appear to differ.",
            ResolverScenarioFamily.ReplaceableStructureChurn => "A replaceable object changed non-additively.",
            ResolverScenarioFamily.ComplexComposite => "Several structural change patterns are present at once.",
            ResolverScenarioFamily.AdditiveOnly => "Only additive structure is missing from the live DB.",
            _ => "MetaSql could not reduce this to a narrower pattern."
        };
    }

    public static string BuildStaleScriptPrompt(MetaSqlTargetInspection inspection, BlockerScriptFile staleScript)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Stale script:");
        builder.Append("    ");
        builder.AppendLine(staleScript.Path);
        builder.AppendLine("        This script no longer matches the current target state.");
        builder.AppendLine();
        builder.AppendLine("How should this be handled?");
        builder.AppendLine("  1. Move it to archive");
        builder.AppendLine($"     Folder: {inspection.Context.ArchivePath}");
        builder.AppendLine("  2. Leave it where it is for now");
        builder.AppendLine("     deploy-test and deploy will continue to treat it as stale.");
        return builder.ToString().TrimEnd();
    }
}

internal enum ResolveBlockerAction
{
    CreatedBaselineScript,
    CreatedTargetScript,
    Skipped
}

internal enum ResolveStaleScriptAction
{
    Archived,
    Kept
}
