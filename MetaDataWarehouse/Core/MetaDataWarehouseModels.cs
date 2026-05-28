using System.Xml.Linq;
using Meta.Core.Domain;
using Meta.Core.Serialization;

namespace MetaDataWarehouse.Core;

public static class MetaDataWarehouseModels
{
    public const string MetaDataWarehouseModelName = "MetaDataWarehouse";
    public const string MetaDataWarehouseImplementationModelName = "MetaDataWarehouseImplementation";

    private const string MetaDataWarehouseModelResourceName = "MetaDataWarehouse.Core.Models.MetaDataWarehouse.model.xml";
    private const string MetaDataWarehouseImplementationModelResourceName = "MetaDataWarehouse.Core.Models.MetaDataWarehouseImplementation.model.xml";

    public static GenericModel CreateMetaDataWarehouseModel()
    {
        return LoadModel(MetaDataWarehouseModelResourceName, MetaDataWarehouseModelName);
    }

    public static GenericModel CreateMetaDataWarehouseImplementationModel()
    {
        return LoadModel(MetaDataWarehouseImplementationModelResourceName, MetaDataWarehouseImplementationModelName);
    }

    private static GenericModel LoadModel(string resourceName, string expectedModelName)
    {
        var assembly = typeof(MetaDataWarehouseModels).Assembly;
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
