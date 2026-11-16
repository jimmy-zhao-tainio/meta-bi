// Derived typed member access. Keep outside authored workspace source sets.
#nullable enable
namespace MetaSchemaToRawDataVaultOptions;
public interface IMetaSchemaToRawDataVaultOptionsCollectionVisitor
{
    void Visit<TEntity>(string entityName, global::System.Func<global::MetaSchemaToRawDataVaultOptions.MetaSchemaToRawDataVaultOptionsModel, global::System.Collections.Generic.IList<TEntity>> rows, global::System.Func<TEntity, string> readId, global::System.Action<TEntity, string> writeId)
        where TEntity : class;
}

public interface IMetaSchemaToRawDataVaultOptionsRelationshipVisitor
{
    void Visit<TRow, TTarget>(string entityName, string navigationName, string targetEntityName, bool isNullable, global::System.Func<global::MetaSchemaToRawDataVaultOptions.MetaSchemaToRawDataVaultOptionsModel, global::System.Collections.Generic.IList<TRow>> rows, global::System.Func<TRow, TTarget?> read, global::System.Action<TRow, TTarget?> write, global::System.Func<TTarget, string> readTargetId)
        where TRow : class where TTarget : class;
}

public static class MetaSchemaToRawDataVaultOptionsModelAccess
{
    public static void VisitCollections(IMetaSchemaToRawDataVaultOptionsCollectionVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
        visitor.Visit<global::MetaSchemaToRawDataVaultOptions.ConversionOptions>("ConversionOptions", static model => model.ConversionOptionsList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSchemaToRawDataVaultOptions.IgnoredFieldName>("IgnoredFieldName", static model => model.IgnoredFieldNameList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSchemaToRawDataVaultOptions.IgnoredFieldSuffix>("IgnoredFieldSuffix", static model => model.IgnoredFieldSuffixList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSchemaToRawDataVaultOptions.IncludeViewsOption>("IncludeViewsOption", static model => model.IncludeViewsOptionList, static row => row.Id, static (row, id) => row.Id = id);
    }

    public static void VisitRelationships(IMetaSchemaToRawDataVaultOptionsRelationshipVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
        visitor.Visit<global::MetaSchemaToRawDataVaultOptions.IgnoredFieldName, global::MetaSchemaToRawDataVaultOptions.ConversionOptions>("IgnoredFieldName", "ConversionOptions", "ConversionOptions", false, static model => model.IgnoredFieldNameList, static row => row.ConversionOptions, static (row, target) => row.ConversionOptions = target!, static target => target.Id);
        visitor.Visit<global::MetaSchemaToRawDataVaultOptions.IgnoredFieldSuffix, global::MetaSchemaToRawDataVaultOptions.ConversionOptions>("IgnoredFieldSuffix", "ConversionOptions", "ConversionOptions", false, static model => model.IgnoredFieldSuffixList, static row => row.ConversionOptions, static (row, target) => row.ConversionOptions = target!, static target => target.Id);
        visitor.Visit<global::MetaSchemaToRawDataVaultOptions.IncludeViewsOption, global::MetaSchemaToRawDataVaultOptions.ConversionOptions>("IncludeViewsOption", "ConversionOptions", "ConversionOptions", false, static model => model.IncludeViewsOptionList, static row => row.ConversionOptions, static (row, target) => row.ConversionOptions = target!, static target => target.Id);
    }
}