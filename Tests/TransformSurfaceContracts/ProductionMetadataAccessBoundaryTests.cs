using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

/// <summary>Enforces the production boundary on compiled calls, including calls emitted by generated code.</summary>
public sealed class ProductionMetadataAccessBoundaryTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public void ProductionAssemblies_DoNotDiscoverOrAccessModeledStructureThroughClrReflection()
    {
        var root = FindRoot();
        var configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
        var violations = new List<string>();
        var checkedAssemblies = 0;
        foreach (var project in Directory.GetDirectories(root)
                     .Where(path => Path.GetFileName(path).StartsWith("Meta", StringComparison.Ordinal))
                     .SelectMany(path => Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories))
                     .Where(path => !Path.GetRelativePath(root, path).Split(Path.DirectorySeparatorChar)
                         .Any(segment => segment is "Tests" or "bin" or "obj"))
                     .Order(StringComparer.Ordinal))
        {
            var definition = XDocument.Load(project);
            var assemblyName = definition.Descendants("AssemblyName").Select(item => item.Value).FirstOrDefault()
                ?? Path.GetFileNameWithoutExtension(project);
            var targetFramework = definition.Descendants("TargetFramework").Select(item => item.Value).FirstOrDefault()
                ?? throw new InvalidOperationException($"Missing TargetFramework in '{project}'.");
            var assembly = Path.Combine(Path.GetDirectoryName(project)!, "bin", configuration, targetFramework, assemblyName + ".dll");
            Assert.True(File.Exists(assembly), $"Build production projects before running architecture integration tests. Missing '{assembly}'.");
            using var stream = File.OpenRead(assembly);
            using var pe = new PEReader(stream);
            var metadata = pe.GetMetadataReader();
            foreach (var handle in metadata.MemberReferences)
            {
                var reference = metadata.GetMemberReference(handle);
                if (reference.Parent.Kind != HandleKind.TypeReference) continue;
                var type = metadata.GetTypeReference((TypeReferenceHandle)reference.Parent);
                var typeName = metadata.GetString(type.Namespace) + "." + metadata.GetString(type.Name);
                var member = metadata.GetString(reference.Name);
                if (ViolatesBoundary(typeName, member))
                    violations.Add($"{Path.GetRelativePath(root, project)}: {typeName}.{member}");
            }
            checkedAssemblies++;
        }

        Assert.True(checkedAssemblies > 0, "No production assemblies were inspected.");
        Assert.True(violations.Count == 0,
            "Modeled structure must come from Meta metadata or compiled access, not CLR discovery/access:" +
            Environment.NewLine + string.Join(Environment.NewLine, violations.Distinct().Order(StringComparer.Ordinal)));
    }

    private static bool ViolatesBoundary(string type, string member)
    {
        if (type.StartsWith("System.Reflection.Emit.", StringComparison.Ordinal)) return true;
        if (type == "System.Activator" && member.StartsWith("CreateInstance", StringComparison.Ordinal)) return true;
        if (type == "System.Type")
            return member is "GetProperty" or "GetProperties" or "GetField" or "GetFields" or
                "GetMethod" or "GetMethods" or "GetMember" or "GetMembers" or "GetConstructor" or
                "GetConstructors" or "GetNestedType" or "GetNestedTypes" or "InvokeMember" or
                "GetGenericArguments" or "MakeGenericType" or "GetInterfaces" or "GetInterface" or
                "get_BaseType" or "get_IsClass" or "get_IsGenericType" or "GetGenericTypeDefinition";
        if (type == "System.Reflection.Assembly")
            return true;
        if (type == "System.Reflection.AssemblyName") return true;
        if (type == "System.Delegate" && member is "DynamicInvoke" or "CreateDelegate") return true;
        if (type.StartsWith("System.Reflection.", StringComparison.Ordinal))
            return member is "GetValue" or "SetValue" or "Invoke" or "CreateDelegate" or
                "GetCustomAttribute" or "GetCustomAttributes" or "GetCustomAttributesData";
        return false;
    }

    private static string FindRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Directory.Build.props")) &&
                Directory.Exists(Path.Combine(current.FullName, "MetaDataType"))) return current.FullName;
            current = current.Parent;
        }
        throw new InvalidOperationException("Could not locate meta-bi repository root.");
    }
}
