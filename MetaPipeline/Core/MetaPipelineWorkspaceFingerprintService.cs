using System.Security.Cryptography;
using System.Text;

namespace MetaPipeline;

public sealed class MetaPipelineWorkspaceFingerprintService
{
    private const string Algorithm = "SHA256";

    private readonly Dictionary<string, string> fingerprintsByPath = new(StringComparer.OrdinalIgnoreCase);

    public MetaPipelineOperationalFingerprint CreateWorkspaceFingerprint(
        string fingerprintKind,
        string? subjectId,
        string workspacePath,
        string? taskName = null,
        string? taskKind = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fingerprintKind);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);

        var fullPath = Path.GetFullPath(workspacePath);
        var fingerprint = ResolveWorkspaceFingerprint(fullPath);
        return new MetaPipelineOperationalFingerprint(
            fingerprintKind.Trim(),
            Normalize(subjectId),
            fullPath,
            Algorithm,
            fingerprint,
            Normalize(taskName),
            Normalize(taskKind));
    }

    private string ResolveWorkspaceFingerprint(string fullPath)
    {
        if (fingerprintsByPath.TryGetValue(fullPath, out var cached))
        {
            return cached;
        }

        if (!Directory.Exists(fullPath))
        {
            throw new MetaPipelineConfigurationException($"Cannot fingerprint missing workspace '{fullPath}'.");
        }

        var fingerprint = ComputeWorkspaceFingerprint(fullPath);
        fingerprintsByPath.Add(fullPath, fingerprint);
        return fingerprint;
    }

    private static string ComputeWorkspaceFingerprint(string fullPath)
    {
        var files = Directory
            .EnumerateFiles(fullPath, "*.xml", SearchOption.AllDirectories)
            .Select(path => new
            {
                FullPath = Path.GetFullPath(path),
                RelativePath = Path.GetRelativePath(fullPath, path).Replace('\\', '/'),
            })
            .OrderBy(item => item.RelativePath, StringComparer.Ordinal)
            .ToArray();

        if (files.Length == 0)
        {
            throw new MetaPipelineConfigurationException($"Cannot fingerprint workspace '{fullPath}' because it contains no XML files.");
        }

        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (var file in files)
        {
            AppendUtf8(hash, file.RelativePath);
            AppendByte(hash, 0);

            var bytes = File.ReadAllBytes(file.FullPath);
            AppendUtf8(hash, bytes.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
            AppendByte(hash, 0);
            hash.AppendData(bytes);
            AppendByte(hash, 0);
        }

        return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
    }

    private static void AppendUtf8(IncrementalHash hash, string value) =>
        hash.AppendData(Encoding.UTF8.GetBytes(value));

    private static void AppendByte(IncrementalHash hash, byte value) =>
        hash.AppendData([value]);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
