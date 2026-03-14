using MetaSql.App;
using MetaSql.Workflow;

internal sealed class ResolveConsoleInteraction
{
    public ResolveBlockerAction HandleBlocker(MetaSqlResolutionSession session, MetaSqlTargetInspection inspection, Blocker blocker)
    {
        Console.WriteLine($"Issue: {blocker.ObjectName}");
        foreach (var reason in blocker.Reasons)
        {
            Console.WriteLine($"  {reason}");
        }

        Console.WriteLine("What do you want to do with this?");
        Console.WriteLine("  1. Create a general SQL script for the normal upgrade path");
        Console.WriteLine($"     Path: {inspection.Context.BaselinePath}");
        Console.WriteLine($"  2. Create a SQL script only for target {inspection.Context.Name}");
        Console.WriteLine($"     Path: {inspection.Context.TargetPath}");
        Console.WriteLine("  3. Skip for now");
        while (true)
        {
            Console.Write("> ");
            var input = (Console.ReadLine() ?? string.Empty).Trim();
            switch (input)
            {
                case "1":
                {
                    var path = session.CreateBaselineStub(inspection, blocker);
                    Console.WriteLine($"Created {path}");
                    Console.WriteLine();
                    return ResolveBlockerAction.CreatedBaselineScript;
                }
                case "2":
                {
                    var path = session.CreateTargetStub(inspection, blocker);
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

    public ResolveStaleScriptAction HandleStaleScript(MetaSqlResolutionSession session, MetaSqlTargetInspection inspection, BlockerScriptFile staleScript)
    {
        Console.WriteLine($"Stale script: {staleScript.Path}");
        Console.WriteLine($"  Header object: {staleScript.Header.ObjectName}");
        Console.WriteLine("What do you want to do with this?");
        Console.WriteLine("  1. Move it to archive");
        Console.WriteLine("  2. Keep it for now");
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
