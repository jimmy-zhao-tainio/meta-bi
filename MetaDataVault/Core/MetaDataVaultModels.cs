using System.Xml.Linq;
using Meta.Core.Domain;
using Meta.Core.Serialization;

namespace MetaDataVault.Core;

public static class MetaDataVaultModels
{
    public const string MetaRawDataVaultModelName = "MetaRawDataVault";
    public const string MetaBusinessDataVaultModelName = "MetaBusinessDataVault";

    private const string MetaRawDataVaultModelResourceName = "MetaDataVault.Core.Models.MetaRawDataVault.model.xml";
    private const string MetaBusinessDataVaultModelResourceName = "MetaDataVault.Core.Models.MetaBusinessDataVault.model.xml";

    public static GenericModel CreateMetaRawDataVaultModel()
    {
        return LoadModel(MetaRawDataVaultModelResourceName, MetaRawDataVaultModelName);
    }

    public static GenericModel CreateMetaBusinessDataVaultModel()
    {
        return LoadModel(MetaBusinessDataVaultModelResourceName, MetaBusinessDataVaultModelName);
    }

    private static GenericModel LoadModel(string resourceName, string expectedModelName)
    {
        var assembly = typeof(MetaDataVaultModels).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException(
                               $"Could not load embedded sanctioned model resource '{resourceName}'.");
        var document = XDocument.Load(stream, LoadOptions.None);
        var model = ModelXmlCodec.Load(document);
        if (!string.Equals(model.Name, expectedModelName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Sanctioned model name '{model.Name}' from resource '{resourceName}' does not match expected '{expectedModelName}'.");
        }

        return model;
    }
}
