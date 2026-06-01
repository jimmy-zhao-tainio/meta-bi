using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace MetaOrchestration.Core;

public sealed class OrchestrationWorkspaceExecutionLease : IDisposable
{
    private static readonly object ActiveLeaseGate = new();
    private static readonly HashSet<string> ActiveWorkspaceKeys = new(StringComparer.Ordinal);

    private readonly FileStream lockStream;
    private readonly string workspaceKey;
    private readonly string leaseRecordPath;
    private bool disposed;

    private OrchestrationWorkspaceExecutionLease(
        string workspacePath,
        Guid runId,
        string workspaceKey,
        FileStream lockStream,
        string leaseRecordPath)
    {
        WorkspacePath = workspacePath;
        RunId = runId;
        this.workspaceKey = workspaceKey;
        this.lockStream = lockStream;
        this.leaseRecordPath = leaseRecordPath;
    }

    public string WorkspacePath { get; }

    public Guid RunId { get; }

    public string LeaseRecordPath => leaseRecordPath;

    public static OrchestrationWorkspaceExecutionLease Acquire(
        string workspacePath,
        Guid runId,
        string? runArtifactsRootPath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);

        var canonicalWorkspacePath = Path.GetFullPath(workspacePath);
        var workspaceKey = ComputeStableHash(canonicalWorkspacePath.ToUpperInvariant());

        lock (ActiveLeaseGate)
        {
            if (!ActiveWorkspaceKeys.Add(workspaceKey))
            {
                var existingLease = ReadExistingLeaseRecord(workspaceKey, runArtifactsRootPath);
                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(existingLease)
                        ? $"Another orchestration execution is already using workspace '{canonicalWorkspacePath}'."
                        : $"Another orchestration execution is already using workspace '{canonicalWorkspacePath}'. Existing lease: {existingLease}");
            }
        }

        FileStream lockStream;
        var leaseDirectory = Path.Combine(ResolveOperationalRoot(runArtifactsRootPath), "leases");
        var lockFilePath = Path.Combine(leaseDirectory, workspaceKey + ".lock");
        try
        {
            Directory.CreateDirectory(leaseDirectory);
            lockStream = new FileStream(
                lockFilePath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None);
        }
        catch (IOException)
        {
            ReleaseInProcessLease(workspaceKey);
            var existingLease = ReadExistingLeaseRecord(workspaceKey, runArtifactsRootPath);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(existingLease)
                    ? $"Another orchestration execution is already using workspace '{canonicalWorkspacePath}'."
                    : $"Another orchestration execution is already using workspace '{canonicalWorkspacePath}'. Existing lease: {existingLease}");
        }
        catch
        {
            ReleaseInProcessLease(workspaceKey);
            throw;
        }

        try
        {
            var leaseRecordPath = WriteLeaseRecord(workspaceKey, canonicalWorkspacePath, runId, runArtifactsRootPath);
            return new OrchestrationWorkspaceExecutionLease(canonicalWorkspacePath, runId, workspaceKey, lockStream, leaseRecordPath);
        }
        catch
        {
            lockStream.Dispose();
            ReleaseInProcessLease(workspaceKey);
            throw;
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        try
        {
            MarkLeaseReleased();
        }
        finally
        {
            lockStream.Dispose();
            ReleaseInProcessLease(workspaceKey);
        }
    }

    internal static string ResolveOperationalRoot(string? runArtifactsRootPath)
    {
        if (!string.IsNullOrWhiteSpace(runArtifactsRootPath))
        {
            return Path.GetFullPath(runArtifactsRootPath);
        }

        var configuredRoot = Environment.GetEnvironmentVariable("META_ORCHESTRATION_RUN_ROOT");
        if (!string.IsNullOrWhiteSpace(configuredRoot))
        {
            return Path.GetFullPath(configuredRoot);
        }

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return string.IsNullOrWhiteSpace(localAppData)
            ? Path.Combine(Path.GetTempPath(), "meta", "orchestration")
            : Path.Combine(localAppData, "meta", "orchestration");
    }

    private void MarkLeaseReleased()
    {
        try
        {
            File.AppendAllText(
                leaseRecordPath,
                "ReleasedAtUtc\t" + DateTimeOffset.UtcNow.ToString("O") + Environment.NewLine,
                Encoding.UTF8);
        }
        catch
        {
            // Lease release is owned by the lock file. The record is diagnostic evidence only.
        }
    }

    private static string WriteLeaseRecord(
        string workspaceKey,
        string workspacePath,
        Guid runId,
        string? runArtifactsRootPath)
    {
        var leaseDirectory = Path.Combine(ResolveOperationalRoot(runArtifactsRootPath), "leases");
        Directory.CreateDirectory(leaseDirectory);
        var leaseRecordPath = Path.Combine(leaseDirectory, workspaceKey + ".tsv");
        var lines = new[]
        {
            "Field\tValue",
            "RunId\t" + runId.ToString("D"),
            "WorkspacePath\t" + Escape(workspacePath),
            "ProcessId\t" + Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            "ProcessName\t" + Escape(Process.GetCurrentProcess().ProcessName),
            "MachineName\t" + Escape(Environment.MachineName),
            "AcquiredAtUtc\t" + DateTimeOffset.UtcNow.ToString("O"),
        };
        File.WriteAllLines(leaseRecordPath, lines, Encoding.UTF8);
        return leaseRecordPath;
    }

    private static string ReadExistingLeaseRecord(string workspaceKey, string? runArtifactsRootPath)
    {
        var leaseRecordPath = Path.Combine(ResolveOperationalRoot(runArtifactsRootPath), "leases", workspaceKey + ".tsv");
        if (!File.Exists(leaseRecordPath))
        {
            return string.Empty;
        }

        try
        {
            var values = File.ReadLines(leaseRecordPath, Encoding.UTF8)
                .Skip(1)
                .Select(line => line.Split('\t', 2))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0], parts => parts[1], StringComparer.OrdinalIgnoreCase);
            var runId = values.GetValueOrDefault("RunId", "<unknown>");
            var processId = values.GetValueOrDefault("ProcessId", "<unknown>");
            var acquiredAt = values.GetValueOrDefault("AcquiredAtUtc", "<unknown>");
            return $"RunId={runId}; ProcessId={processId}; AcquiredAtUtc={acquiredAt}";
        }
        catch
        {
            return leaseRecordPath;
        }
    }

    private static string ComputeStableHash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static void ReleaseInProcessLease(string workspaceKey)
    {
        lock (ActiveLeaseGate)
        {
            ActiveWorkspaceKeys.Remove(workspaceKey);
        }
    }

    private static string Escape(string value) =>
        value.Replace("\t", " ", StringComparison.Ordinal).ReplaceLineEndings(" ");
}
