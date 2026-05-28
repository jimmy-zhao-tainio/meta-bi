using System.Xml.Linq;
using Meta.Core.Domain;
using Meta.Core.Serialization;

namespace MetaAnalytics.Core;

public static class MetaAnalyticsModels
{
    public const string MetaAnalyticsModelName = "MetaAnalytics";

    private const string MetaAnalyticsModelResourceName = "MetaAnalytics.Core.Models.MetaAnalytics.model.xml";

    public static GenericModel CreateMetaAnalyticsModel()
    {
        var assembly = typeof(MetaAnalyticsModels).Assembly;
        using var stream = assembly.GetManifestResourceStream(MetaAnalyticsModelResourceName)
                           ?? throw new InvalidOperationException(
                               $"Could not load embedded sanctioned model resource '{MetaAnalyticsModelResourceName}'.");
        var document = XDocument.Load(stream, LoadOptions.None);
        var model = ModelXmlCodec.Load(document);
        if (!string.Equals(model.Name, MetaAnalyticsModelName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Sanctioned model name '{model.Name}' from resource '{MetaAnalyticsModelResourceName}' does not match expected '{MetaAnalyticsModelName}'.");
        }

        return model;
    }
}
