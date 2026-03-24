using System.Xml.Linq;
using Meta.Core.Domain;
using Meta.Core.Serialization;

namespace MetaDataType.Core;

public static class MetaDataTypeModels
{
    public const string MetaDataTypeModelName = "MetaDataType";
    private const string MetaDataTypeModelResourceName = "MetaDataType.Core.Models.MetaDataType.model.xml";

    public static GenericModel CreateMetaDataTypeModel()
    {
        return LoadModel(MetaDataTypeModelResourceName, MetaDataTypeModelName);
    }

    private static GenericModel LoadModel(string resourceName, string expectedModelName)
    {
        var assembly = typeof(MetaDataTypeModels).Assembly;
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
