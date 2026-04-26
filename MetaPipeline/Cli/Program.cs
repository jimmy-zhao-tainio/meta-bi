using Meta.Core.Presentation;

internal static partial class Program
{
    private static readonly ConsolePresenter Presenter = new();

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        if (string.Equals(args[0], "execute", StringComparison.OrdinalIgnoreCase))
        {
            return await RunExecuteAsync(args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "execute-sqlserver", StringComparison.OrdinalIgnoreCase))
        {
            return await RunExecuteSqlServerAsync(args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "init", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 2 && IsHelpToken(args[1]))
            {
                PrintInitHelp();
                return 0;
            }

            return RunInit(args, startIndex: 1);
        }

        if (string.Equals(args[0], "add-pipeline", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 2 && IsHelpToken(args[1]))
            {
                PrintAddPipelineHelp();
                return 0;
            }

            return RunAddPipeline(args, startIndex: 1);
        }

        if (string.Equals(args[0], "add-transform", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 2 && IsHelpToken(args[1]))
            {
                PrintAddTransformHelp();
                return 0;
            }

            return RunAddTransform(args, startIndex: 1);
        }

        if (string.Equals(args[0], "inspect", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 2 && IsHelpToken(args[1]))
            {
                PrintInspectHelp();
                return 0;
            }

            return RunInspect(args, startIndex: 1);
        }

        return Fail($"unknown command '{args[0]}'.", "meta-pipeline help");
    }
}
