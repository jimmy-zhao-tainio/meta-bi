using System.Text;

namespace MetaTransform.Binding;

public sealed class TransformBindingPartialReportService
{
    public string? Write(
        string partialReportPath,
        IReadOnlyList<BindWorkspaceObjectIssue> objectIssues)
    {
        if (string.IsNullOrWhiteSpace(partialReportPath))
        {
            return null;
        }

        var reportFullPath = Path.GetFullPath(partialReportPath);
        var directory = Path.GetDirectoryName(reportFullPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var writer = new StreamWriter(
            reportFullPath,
            append: false,
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        writer.WriteLine("TransformScriptId\tTransformScriptName\tStage\tCode\tMessage");
        foreach (var issue in objectIssues)
        {
            writer.Write(Tsv(issue.TransformScriptId));
            writer.Write('\t');
            writer.Write(Tsv(issue.TransformScriptName));
            writer.Write('\t');
            writer.Write(Tsv(issue.Stage));
            writer.Write('\t');
            writer.Write(Tsv(issue.Code));
            writer.Write('\t');
            writer.WriteLine(Tsv(issue.Message));
        }

        return reportFullPath;
    }

    private static string Tsv(string value) =>
        (value ?? string.Empty)
            .Replace('\t', ' ')
            .Replace('\r', ' ')
            .Replace('\n', ' ');
}
