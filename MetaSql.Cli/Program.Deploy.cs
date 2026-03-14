using MetaSql.App;

internal static partial class Program
{
    private static int RunDeploy(string[] args)
    {
        if (!TryReadTarget(args, "deploy", out var targetName, out var error))
        {
            return string.IsNullOrWhiteSpace(error) ? 0 : Fail(error, "meta-sql deploy <target>");
        }

        try
        {
            var inspection = BuildInspection(targetName);
            var finalInspection = new MetaSqlDeploymentSession().Deploy(inspection);
            Console.WriteLine(finalInspection.RenderedPlan);
            WriteScriptSummary(finalInspection);
            Console.WriteLine();
            Presenter.WriteOk(
                "deploy complete",
                ("Target", finalInspection.Context.Name),
                ("MatchingScripts", finalInspection.MatchingScripts.Count.ToString()),
                ("StaleScripts", finalInspection.StaleScripts.Count.ToString()));
            return 0;
        }
        catch (InvalidOperationException exception)
        {
            var inspection = TryBuildInspection(targetName);
            if (inspection != null)
            {
                Console.WriteLine(inspection.RenderedPlan);
                WriteScriptSummary(inspection);
            }

            return Fail(exception.Message, "run meta-sql deploy-test <target> for details.", 4);
        }
    }
}
