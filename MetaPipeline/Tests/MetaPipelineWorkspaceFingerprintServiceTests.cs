namespace MetaPipeline.Tests;

public sealed class MetaPipelineWorkspaceFingerprintServiceTests
{
    [Fact]
    public void CreateWorkspaceFingerprint_IsStableForSameXmlWorkspaceContent()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaPipeline.Tests", Guid.NewGuid().ToString("N"));
        var firstWorkspace = Path.Combine(tempRoot, "first");
        var secondWorkspace = Path.Combine(tempRoot, "second");

        try
        {
            WriteWorkspace(firstWorkspace, "A");
            WriteWorkspace(secondWorkspace, "A");

            var service = new MetaPipelineWorkspaceFingerprintService();
            var first = service.CreateWorkspaceFingerprint("TransformWorkspace", "TransformScript:1", firstWorkspace);
            var second = service.CreateWorkspaceFingerprint("TransformWorkspace", "TransformScript:1", secondWorkspace);

            Assert.Equal("SHA256", first.Algorithm);
            Assert.Equal(first.FingerprintValue, second.FingerprintValue);
            Assert.Equal(64, first.FingerprintValue.Length);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [Fact]
    public void CreateWorkspaceFingerprint_ChangesWhenXmlWorkspaceContentChanges()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaPipeline.Tests", Guid.NewGuid().ToString("N"));
        var firstWorkspace = Path.Combine(tempRoot, "first");
        var secondWorkspace = Path.Combine(tempRoot, "second");

        try
        {
            WriteWorkspace(firstWorkspace, "A");
            WriteWorkspace(secondWorkspace, "B");

            var service = new MetaPipelineWorkspaceFingerprintService();
            var first = service.CreateWorkspaceFingerprint("TransformWorkspace", "TransformScript:1", firstWorkspace);
            var second = service.CreateWorkspaceFingerprint("TransformWorkspace", "TransformScript:1", secondWorkspace);

            Assert.NotEqual(first.FingerprintValue, second.FingerprintValue);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    private static void WriteWorkspace(string workspacePath, string marker)
    {
        Directory.CreateDirectory(Path.Combine(workspacePath, "instances"));
        File.WriteAllText(
            Path.Combine(workspacePath, "workspace.meta"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<MetaWorkspace representation=\"xml\" location=\".\" />\n");
        File.WriteAllText(Path.Combine(workspacePath, "model.xml"), "<Model />");
        File.WriteAllText(Path.Combine(workspacePath, "instances", "Thing.xml"), "<ThingList><Thing Id=\"" + marker + "\" /></ThingList>");
    }
}
