using MetaSql.App;

internal static partial class Program
{
    private static int RunDeployTest(string[] args)
    {
        if (!TryReadTarget(args, "deploy-test", out var targetName, out var error))
        {
            return string.IsNullOrWhiteSpace(error) ? 0 : Fail(error, "meta-sql deploy-test <target>");
        }

        try
        {
            var inspection = BuildInspection(targetName);
            var output = new MetaSqlDeployTestOutputRenderer().Render(inspection);
            Console.WriteLine(output);
            if (inspection.ActionableIssues.Count == 0)
            {
                WriteScriptSummary(inspection);
                Console.WriteLine();
                Presenter.WriteOk(
                    "deploy-test complete",
                    ("Target", inspection.Context.Name),
                    ("MatchingScripts", inspection.MatchingScripts.Count.ToString()),
                    ("StaleScripts", inspection.StaleScripts.Count.ToString()),
                    ("NeedsAttention", "0"));
            }
            return 0;
        }
        catch (InvalidOperationException exception)
        {
            return Fail(exception.Message, "check deploy/workspace.xml and retry.", 4);
        }
    }
}
