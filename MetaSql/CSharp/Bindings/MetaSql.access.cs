// Derived typed member access. Keep outside authored workspace source sets.
#nullable enable
namespace MetaSql;
public interface IMetaSqlCollectionVisitor
{
    void Visit<TEntity>(string entityName, global::System.Func<global::MetaSql.MetaSqlModel, global::System.Collections.Generic.IList<TEntity>> rows, global::System.Func<TEntity, string> readId, global::System.Action<TEntity, string> writeId)
        where TEntity : class;
}

public interface IMetaSqlRelationshipVisitor
{
    void Visit<TRow, TTarget>(string entityName, string navigationName, string targetEntityName, bool isNullable, global::System.Func<global::MetaSql.MetaSqlModel, global::System.Collections.Generic.IList<TRow>> rows, global::System.Func<TRow, TTarget?> read, global::System.Action<TRow, TTarget?> write, global::System.Func<TTarget, string> readTargetId)
        where TRow : class where TTarget : class;
}

public static class MetaSqlModelAccess
{
    public static void VisitCollections(IMetaSqlCollectionVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
        visitor.Visit<global::MetaSql.Database>("Database", static model => model.DatabaseList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.ForeignKey>("ForeignKey", static model => model.ForeignKeyList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.ForeignKeyColumn>("ForeignKeyColumn", static model => model.ForeignKeyColumnList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.Function>("Function", static model => model.FunctionList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.Index>("Index", static model => model.IndexList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.IndexColumn>("IndexColumn", static model => model.IndexColumnList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.PrimaryKey>("PrimaryKey", static model => model.PrimaryKeyList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.PrimaryKeyColumn>("PrimaryKeyColumn", static model => model.PrimaryKeyColumnList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.Schema>("Schema", static model => model.SchemaList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.StoredProcedure>("StoredProcedure", static model => model.StoredProcedureList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.Table>("Table", static model => model.TableList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.TableColumn>("TableColumn", static model => model.TableColumnList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.TableColumnDataTypeDetail>("TableColumnDataTypeDetail", static model => model.TableColumnDataTypeDetailList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaSql.View>("View", static model => model.ViewList, static row => row.Id, static (row, id) => row.Id = id);
    }

    public static void VisitRelationships(IMetaSqlRelationshipVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
        visitor.Visit<global::MetaSql.ForeignKey, global::MetaSql.Table>("ForeignKey", "SourceTable", "Table", false, static model => model.ForeignKeyList, static row => row.SourceTable, static (row, target) => row.SourceTable = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.ForeignKey, global::MetaSql.Table>("ForeignKey", "TargetTable", "Table", false, static model => model.ForeignKeyList, static row => row.TargetTable, static (row, target) => row.TargetTable = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.ForeignKeyColumn, global::MetaSql.ForeignKey>("ForeignKeyColumn", "ForeignKey", "ForeignKey", false, static model => model.ForeignKeyColumnList, static row => row.ForeignKey, static (row, target) => row.ForeignKey = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.ForeignKeyColumn, global::MetaSql.TableColumn>("ForeignKeyColumn", "SourceColumn", "TableColumn", false, static model => model.ForeignKeyColumnList, static row => row.SourceColumn, static (row, target) => row.SourceColumn = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.ForeignKeyColumn, global::MetaSql.TableColumn>("ForeignKeyColumn", "TargetColumn", "TableColumn", false, static model => model.ForeignKeyColumnList, static row => row.TargetColumn, static (row, target) => row.TargetColumn = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.Function, global::MetaSql.Schema>("Function", "Schema", "Schema", false, static model => model.FunctionList, static row => row.Schema, static (row, target) => row.Schema = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.Index, global::MetaSql.Table>("Index", "Table", "Table", false, static model => model.IndexList, static row => row.Table, static (row, target) => row.Table = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.IndexColumn, global::MetaSql.Index>("IndexColumn", "Index", "Index", false, static model => model.IndexColumnList, static row => row.Index, static (row, target) => row.Index = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.IndexColumn, global::MetaSql.TableColumn>("IndexColumn", "TableColumn", "TableColumn", false, static model => model.IndexColumnList, static row => row.TableColumn, static (row, target) => row.TableColumn = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.PrimaryKey, global::MetaSql.Table>("PrimaryKey", "Table", "Table", false, static model => model.PrimaryKeyList, static row => row.Table, static (row, target) => row.Table = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.PrimaryKeyColumn, global::MetaSql.PrimaryKey>("PrimaryKeyColumn", "PrimaryKey", "PrimaryKey", false, static model => model.PrimaryKeyColumnList, static row => row.PrimaryKey, static (row, target) => row.PrimaryKey = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.PrimaryKeyColumn, global::MetaSql.TableColumn>("PrimaryKeyColumn", "TableColumn", "TableColumn", false, static model => model.PrimaryKeyColumnList, static row => row.TableColumn, static (row, target) => row.TableColumn = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.Schema, global::MetaSql.Database>("Schema", "Database", "Database", false, static model => model.SchemaList, static row => row.Database, static (row, target) => row.Database = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.StoredProcedure, global::MetaSql.Schema>("StoredProcedure", "Schema", "Schema", false, static model => model.StoredProcedureList, static row => row.Schema, static (row, target) => row.Schema = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.Table, global::MetaSql.Schema>("Table", "Schema", "Schema", false, static model => model.TableList, static row => row.Schema, static (row, target) => row.Schema = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.TableColumn, global::MetaSql.Table>("TableColumn", "Table", "Table", false, static model => model.TableColumnList, static row => row.Table, static (row, target) => row.Table = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.TableColumnDataTypeDetail, global::MetaSql.TableColumn>("TableColumnDataTypeDetail", "TableColumn", "TableColumn", false, static model => model.TableColumnDataTypeDetailList, static row => row.TableColumn, static (row, target) => row.TableColumn = target!, static target => target.Id);
        visitor.Visit<global::MetaSql.View, global::MetaSql.Schema>("View", "Schema", "Schema", false, static model => model.ViewList, static row => row.Schema, static (row, target) => row.Schema = target!, static target => target.Id);
    }
}