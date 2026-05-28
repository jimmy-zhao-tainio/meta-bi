using System.Xml.Linq;
using Meta.Core.Domain;
using Meta.Core.Serialization;

namespace MetaMultiDimensional.Core;

public static class MetaMultiDimensionalModels
{
    public const string MetaMultiDimensionalModelName = "MetaMultiDimensional";

    private const string MetaMultiDimensionalModelResourceName = "MetaMultiDimensional.Core.Models.MetaMultiDimensional.model.xml";

    public static GenericModel CreateMetaMultiDimensionalModel()
    {
        var assembly = typeof(MetaMultiDimensionalModels).Assembly;
        using var stream = assembly.GetManifestResourceStream(MetaMultiDimensionalModelResourceName)
                           ?? throw new InvalidOperationException(
                               $"Could not load embedded sanctioned model resource '{MetaMultiDimensionalModelResourceName}'.");
        var document = XDocument.Load(stream, LoadOptions.None);
        var model = ModelXmlCodec.Load(document);
        if (!string.Equals(model.Name, MetaMultiDimensionalModelName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Sanctioned model name '{model.Name}' from resource '{MetaMultiDimensionalModelResourceName}' does not match expected '{MetaMultiDimensionalModelName}'.");
        }

        return model;
    }
}
