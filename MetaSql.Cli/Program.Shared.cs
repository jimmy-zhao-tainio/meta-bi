using MetaSql.App;

internal static partial class Program
{
    private static MetaSqlTargetInspection BuildInspection(string targetName)
    {
        var context = new MetaSqlTargetContextLoader().Load(targetName);
        return new MetaSqlTargetInspector().Inspect(context);
    }

    private static MetaSqlTargetInspection? TryBuildInspection(string targetName)
    {
        try
        {
            return BuildInspection(targetName);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private static void WriteScriptSummary(MetaSqlTargetInspection inspection)
    {
        if (inspection.MatchingScripts.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Matching scripts:");
            foreach (var script in inspection.MatchingScripts)
            {
                Console.WriteLine($"    {script.Path}");
            }
        }

        if (inspection.StaleScripts.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Stale scripts:");
            foreach (var script in inspection.StaleScripts)
            {
                Console.WriteLine($"    {script.Path}");
            }

            Console.WriteLine();
            Console.WriteLine("Active stale scripts block normal deploy.");
        }
    }

    private static bool TryReadTarget(string[] args, string commandName, out string targetName, out string error)
    {
        if (args.Length == 1 || (args.Length >= 2 && IsHelpToken(args[1])))
        {
            PrintCommandHelp(commandName);
            targetName = string.Empty;
            error = string.Empty;
            return false;
        }

        if (args.Length != 2)
        {
            targetName = string.Empty;
            error = $"usage: meta-sql {commandName} <target>";
            return false;
        }

        targetName = args[1];
        error = string.Empty;
        return true;
    }

    private static void PrintCommandHelp(string commandName)
    {
        switch (commandName)
        {
            case "deploy-test":
                Presenter.WriteUsage("meta-sql deploy-test <target>");
                Presenter.WriteInfo("Runs a non-interactive inspection for the target and prints the deployment sheet.");
                Presenter.WriteInfo("Reports matching scripts and stale scripts from deploy/migrate/baseline and deploy/migrate/target/<target>.");
                break;
            case "resolve":
                Presenter.WriteUsage("meta-sql resolve <target>");
                Presenter.WriteInfo("Runs an interactive session for the target.");
                Presenter.WriteInfo("Summarizes current issues, then walks through unresolved and stale items in one session.");
                break;
            case "deploy":
                Presenter.WriteUsage("meta-sql deploy <target>");
                Presenter.WriteInfo("Runs a non-interactive deployment for the target.");
                Presenter.WriteInfo("Applies matching scripts first, rechecks the target, then applies safe structural SQL.");
                break;
        }
    }

}
