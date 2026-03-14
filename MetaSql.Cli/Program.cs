using Meta.Core.Presentation;

internal static partial class Program
{
    private static readonly ConsolePresenter Presenter = new();

    static int Main(string[] args)
    {
        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        return args[0].ToLowerInvariant() switch
        {
            "deploy-test" => RunDeployTest(args),
            "deploy" => RunDeploy(args),
            "resolve" => RunResolve(args),
            _ => Fail($"unknown command '{args[0]}'.", "meta-sql help")
        };
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        Presenter.WriteUsage("meta-sql <command> <target>");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog(
            "Commands:",
            [
                ("deploy-test", "Inspect a target and print the deployment sheet."),
                ("resolve", "Walk through current issues and create or archive SQL files."),
                ("deploy", "Apply matching scripts and safe structural SQL to a target.")
            ]);
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-sql deploy-test <target>");
    }

    private static int Fail(string message, string next, int exitCode = 1)
    {
        Presenter.WriteFailure(message, [$"Next: {next}"]);
        return exitCode;
    }
}
