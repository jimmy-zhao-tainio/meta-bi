using System.Xml.Linq;
using Meta.Core.Domain;
using Meta.Core.Serialization;

namespace MetaTabular.Core;

public static class MetaTabularModels
{
    public const string MetaTabularModelName = "MetaTabular";

    private const string MetaTabularModelResourceName = "MetaTabular.Core.Models.MetaTabular.model.xml";

    public static GenericModel CreateMetaTabularModel()
    {
        var assembly = typeof(MetaTabularModels).Assembly;
        using var stream = assembly.GetManifestResourceStream(MetaTabularModelResourceName)
                           ?? throw new InvalidOperationException(
                               $"Could not load embedded sanctioned model resource '{MetaTabularModelResourceName}'.");
        var document = XDocument.Load(stream, LoadOptions.None);
        var model = ModelXmlCodec.Load(document);
        if (!string.Equals(model.Name, MetaTabularModelName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Sanctioned model name '{model.Name}' from resource '{MetaTabularModelResourceName}' does not match expected '{MetaTabularModelName}'.");
        }

        return model;
    }
}
