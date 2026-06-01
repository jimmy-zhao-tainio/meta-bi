using System.Text;

namespace MetaOrchestration.Core;

public sealed class OrchestrationDiagnosticLogBuffer
{
    private readonly StringBuilder text = new();
    private readonly int maxBytes;
    private readonly int maxLineLength;
    private int capturedBytes;

    public OrchestrationDiagnosticLogBuffer(
        OrchestrationLogCapturePolicy policy,
        string? artifactPath = null)
    {
        ArgumentNullException.ThrowIfNull(policy);
        maxBytes = Math.Max(0, policy.MaxBytesPerWorkerStream);
        maxLineLength = Math.Max(1, policy.MaxLineLength);
        ArtifactPath = string.IsNullOrWhiteSpace(artifactPath) ? null : Path.GetFullPath(artifactPath);
        if (!string.IsNullOrWhiteSpace(ArtifactPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
            File.WriteAllText(ArtifactPath, string.Empty, Encoding.UTF8);
        }
    }

    public string? ArtifactPath { get; }

    public bool WasTruncated { get; private set; }

    public long DroppedBytes { get; private set; }

    public int DroppedLines { get; private set; }

    public int CapturedBytes => capturedBytes;

    public void AppendLine(string? line)
    {
        line ??= string.Empty;
        if (maxBytes == 0)
        {
            DroppedLines++;
            DroppedBytes += Encoding.UTF8.GetByteCount(line);
            WasTruncated = true;
            return;
        }

        if (line.Length > maxLineLength)
        {
            DroppedBytes += Encoding.UTF8.GetByteCount(line.AsSpan(maxLineLength));
            line = line[..maxLineLength] + "... [line truncated]";
            WasTruncated = true;
        }

        var bytes = Encoding.UTF8.GetByteCount(line) + Encoding.UTF8.GetByteCount(Environment.NewLine);
        if (capturedBytes + bytes > maxBytes)
        {
            DroppedLines++;
            DroppedBytes += bytes;
            WasTruncated = true;
            return;
        }

        text.AppendLine(line);
        if (!string.IsNullOrWhiteSpace(ArtifactPath))
        {
            File.AppendAllText(ArtifactPath, line + Environment.NewLine, Encoding.UTF8);
        }

        capturedBytes += bytes;
    }

    public override string ToString()
    {
        if (!WasTruncated)
        {
            return text.ToString();
        }

        var suffix = $"[diagnostics truncated: dropped {DroppedLines.ToString(System.Globalization.CultureInfo.InvariantCulture)} line(s), {DroppedBytes.ToString(System.Globalization.CultureInfo.InvariantCulture)} byte(s)]";
        return text.Length == 0
            ? suffix + Environment.NewLine
            : text.ToString() + suffix + Environment.NewLine;
    }
}

public sealed record OrchestrationLogCapturePolicy(
    int MaxLineLength = 4096,
    int MaxBytesPerWorkerStream = 262144,
    int MaxBytesPerTaskAttempt = 65536,
    int MaxBytesPerRun = 16777216)
{
    public static OrchestrationLogCapturePolicy Default { get; } = new();
}
