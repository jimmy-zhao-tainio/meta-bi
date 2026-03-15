using MetaSql.App;

internal static partial class Program
{
    private static int RunResolve(string[] args)
    {
        if (!TryReadTarget(args, "resolve", out var targetName, out var error))
        {
            return string.IsNullOrWhiteSpace(error) ? 0 : Fail(error, "meta-sql resolve <target>");
        }

        try
        {
            var session = new MetaSqlResolutionSession();
            var interaction = new ResolveConsoleInteraction();
            var inspection = BuildInspection(targetName);
            var createdScriptCount = 0;
            var skippedIssueCount = 0;
            var keptStaleScriptCount = 0;
            new MetaSqlResolveGuard().EnsureResolveAllowed(inspection.Context);
            Console.WriteLine(session.BuildSummary(inspection));
            Console.WriteLine();

            if (inspection.ActionableIssues.Count == 0)
            {
                Presenter.WriteOk("nothing needs attention", ("Target", inspection.Context.Name));
                return 0;
            }

            foreach (var issue in inspection.ActionableIssues.Where(item => item.Blocker != null))
            {
                switch (interaction.HandleIssue(session, inspection, issue))
                {
                    case ResolveBlockerAction.CreatedBaselineScript:
                    case ResolveBlockerAction.CreatedTargetScript:
                        createdScriptCount++;
                        break;
                    case ResolveBlockerAction.Skipped:
                        skippedIssueCount++;
                        break;
                }
            }

            foreach (var staleScript in inspection.StaleScripts)
            {
                switch (interaction.HandleStaleScript(session, inspection, staleScript))
                {
                    case ResolveStaleScriptAction.Archived:
                        break;
                    case ResolveStaleScriptAction.Kept:
                        keptStaleScriptCount++;
                        break;
                }
            }

            Console.WriteLine();
            if (createdScriptCount > 0)
            {
                Presenter.WriteOk(
                    "script files created",
                    ("Target", inspection.Context.Name),
                    ("CreatedScripts", createdScriptCount.ToString()));
                Console.WriteLine($"Fill in the created SQL script{(createdScriptCount == 1 ? string.Empty : "s")}, then run meta-sql deploy-test {inspection.Context.Name} again.");
                if (skippedIssueCount > 0)
                {
                    Console.WriteLine($"{skippedIssueCount} issue{(skippedIssueCount == 1 ? string.Empty : "s")} were skipped and still need attention.");
                }

                if (keptStaleScriptCount > 0)
                {
                    Console.WriteLine($"{keptStaleScriptCount} stale script{(keptStaleScriptCount == 1 ? string.Empty : "s")} were kept and still block normal deploy.");
                }

                return 0;
            }

            var finalInspection = BuildInspection(targetName);
            Console.WriteLine(session.BuildSummary(finalInspection));
            if (finalInspection.ActionableIssues.Count > 0)
            {
                Console.WriteLine("No changes were made.");
                Console.WriteLine($"Target: {finalInspection.Context.Name}");
            }
            else
            {
                Presenter.WriteOk("resolve session finished", ("Target", finalInspection.Context.Name));
            }
            return 0;
        }
        catch (InvalidOperationException exception)
        {
            return Fail(exception.Message, "check deploy/workspace.xml and retry.", 4);
        }
    }
}
