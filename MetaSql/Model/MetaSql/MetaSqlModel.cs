#nullable enable

using System.Collections.Generic;

namespace MetaSql
{
    public sealed partial class MetaSqlModel
    {
        public static MetaSqlModel CreateEmpty() => new();

        public List<Database> DatabaseList { get; set; } = new();
        public List<ForeignKey> ForeignKeyList { get; set; } = new();
        public List<ForeignKeyColumn> ForeignKeyColumnList { get; set; } = new();
        public List<Function> FunctionList { get; set; } = new();
        public List<Index> IndexList { get; set; } = new();
        public List<IndexColumn> IndexColumnList { get; set; } = new();
        public List<PrimaryKey> PrimaryKeyList { get; set; } = new();
        public List<PrimaryKeyColumn> PrimaryKeyColumnList { get; set; } = new();
        public List<Schema> SchemaList { get; set; } = new();
        public List<StoredProcedure> StoredProcedureList { get; set; } = new();
        public List<Table> TableList { get; set; } = new();
        public List<TableColumn> TableColumnList { get; set; } = new();
        public List<TableColumnDataTypeDetail> TableColumnDataTypeDetailList { get; set; } = new();
        public List<View> ViewList { get; set; } = new();
    }
}
