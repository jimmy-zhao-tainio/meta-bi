using System.Text;
using MetaSql.Workflow;

namespace MetaSql.App;

public sealed class MetaSqlDeployTestOutputRenderer
{
    public string Render(MetaSqlTargetInspection inspection)
    {
        ArgumentNullException.ThrowIfNull(inspection);

        if (inspection.ActionableIssues.Count == 0)
        {
            return inspection.RenderedPlan;
        }

        var builder = new StringBuilder();
        builder.AppendLine("Action needed:");

        foreach (var issue in inspection.ActionableIssues)
        {
            AppendIssue(builder, issue);
        }

        builder.AppendLine("Run:");
        builder.AppendLine($"    meta-sql resolve {inspection.Context.Name}");
        return builder.ToString().TrimEnd();
    }

    private static void AppendIssue(StringBuilder builder, MetaSqlIssue issue)
    {
        builder.Append("    ");
        builder.AppendLine(issue.ObjectName);
        foreach (var detail in issue.Details)
        {
            builder.Append("        ");
            builder.AppendLine(detail);
        }

        builder.AppendLine();
    }
}
