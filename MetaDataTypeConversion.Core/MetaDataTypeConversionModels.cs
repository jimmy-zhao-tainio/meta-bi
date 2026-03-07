using System.Xml.Linq;
using Meta.Core.Domain;
using Meta.Core.Serialization;

namespace MetaDataTypeConversion.Core;

public static class MetaDataTypeConversionModels
{
    public const string MetaDataTypeConversionModelName = "MetaDataTypeConversion";
    private const string MetaDataTypeConversionModelResourceName = "MetaDataTypeConversion.Core.Models.MetaDataTypeConversion.model.xml";

    public static GenericModel CreateMetaDataTypeConversionModel()
    {
        return LoadModel(MetaDataTypeConversionModelResourceName, MetaDataTypeConversionModelName);
    }

    private static GenericModel LoadModel(string resourceName, string expectedModelName)
    {
        var assembly = typeof(MetaDataTypeConversionModels).Assembly;
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
