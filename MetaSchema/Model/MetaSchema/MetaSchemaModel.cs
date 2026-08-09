#nullable enable

using System.Collections.Generic;

namespace MetaSchema
{
    public sealed partial class MetaSchemaModel
    {
        public static MetaSchemaModel CreateEmpty() => new();

        public List<Field> FieldList { get; set; } = new();
        public List<FieldDataTypeDetail> FieldDataTypeDetailList { get; set; } = new();
        public List<Key> KeyList { get; set; } = new();
        public List<KeyField> KeyFieldList { get; set; } = new();
        public List<PrimaryKey> PrimaryKeyList { get; set; } = new();
        public List<Schema> SchemaList { get; set; } = new();
        public List<SchemaObject> SchemaObjectList { get; set; } = new();
        public List<System> SystemList { get; set; } = new();
        public List<Table> TableList { get; set; } = new();
        public List<TableRelationship> TableRelationshipList { get; set; } = new();
        public List<TableRelationshipField> TableRelationshipFieldList { get; set; } = new();
        public List<UniqueKey> UniqueKeyList { get; set; } = new();
        public List<View> ViewList { get; set; } = new();
    }
}
