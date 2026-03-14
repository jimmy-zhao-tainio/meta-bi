using System.Text;

namespace MetaSql.Core;

public sealed class SqlServerPreflightPlanRenderer
{
    public string Render(SqlServerPreflightPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        var builder = new StringBuilder();
        AppendSqlSection(builder, "Add table:", plan.AddTables);
        AppendSqlSection(builder, "Add column:", plan.AddColumns);
        AppendSqlSection(builder, "Add index:", plan.AddIndexes);
        AppendSqlSection(builder, "Add constraint:", plan.AddConstraints);
        AppendSqlSection(builder, "Drop table:", plan.DropTables);
        AppendNoteSection(builder, "Manual required:", plan.ManualRequiredItems);
        AppendNoteSection(builder, "Blocked:", plan.BlockedItems);

        if (builder.Length == 0)
        {
            builder.AppendLine("No structural changes.");
        }

        return builder.ToString().TrimEnd();
    }

    private static void AppendSqlSection(StringBuilder builder, string heading, IReadOnlyList<string> items)
    {
        foreach (var item in items)
        {
            if (builder.Length > 0)
            {
                builder.AppendLine();
            }

            builder.AppendLine(heading);
            foreach (var line in item.Split(["\r\n", "\n"], StringSplitOptions.None))
            {
                builder.Append("    ");
                builder.AppendLine(line);
            }
        }
    }

    private static void AppendNoteSection(StringBuilder builder, string heading, IReadOnlyList<PlanNote> notes)
    {
        if (notes.Count == 0)
        {
            return;
        }

        if (builder.Length > 0)
        {
            builder.AppendLine();
        }

        builder.AppendLine(heading);
        foreach (var note in notes)
        {
            builder.Append("    ");
            builder.AppendLine(note.ObjectName);
            foreach (var reason in note.Reasons)
            {
                builder.Append("        ");
                builder.AppendLine(reason);
            }

            builder.AppendLine();
        }

        builder.Length -= Environment.NewLine.Length;
    }
}
