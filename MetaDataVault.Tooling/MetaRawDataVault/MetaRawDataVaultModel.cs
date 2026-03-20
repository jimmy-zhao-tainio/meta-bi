using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meta.Core.Domain;

namespace MetaRawDataVault
{
    public sealed partial class MetaRawDataVaultModel
    {
        internal MetaRawDataVaultModel(
            List<RawHub> rawHubList,
            List<RawHubKeyPart> rawHubKeyPartList,
            List<RawHubSatellite> rawHubSatelliteList,
            List<RawHubSatelliteAttribute> rawHubSatelliteAttributeList,
            List<RawLink> rawLinkList,
            List<RawLinkHub> rawLinkHubList,
            List<RawLinkSatellite> rawLinkSatelliteList,
            List<RawLinkSatelliteAttribute> rawLinkSatelliteAttributeList,
            List<SourceField> sourceFieldList,
            List<SourceFieldDataTypeDetail> sourceFieldDataTypeDetailList,
            List<SourceSchema> sourceSchemaList,
            List<SourceSystem> sourceSystemList,
            List<SourceTable> sourceTableList,
            List<SourceTableRelationship> sourceTableRelationshipList,
            List<SourceTableRelationshipField> sourceTableRelationshipFieldList
        )
        {
            RawHubList = rawHubList;
            RawHubKeyPartList = rawHubKeyPartList;
            RawHubSatelliteList = rawHubSatelliteList;
            RawHubSatelliteAttributeList = rawHubSatelliteAttributeList;
            RawLinkList = rawLinkList;
            RawLinkHubList = rawLinkHubList;
            RawLinkSatelliteList = rawLinkSatelliteList;
            RawLinkSatelliteAttributeList = rawLinkSatelliteAttributeList;
            SourceFieldList = sourceFieldList;
            SourceFieldDataTypeDetailList = sourceFieldDataTypeDetailList;
            SourceSchemaList = sourceSchemaList;
            SourceSystemList = sourceSystemList;
            SourceTableList = sourceTableList;
            SourceTableRelationshipList = sourceTableRelationshipList;
            SourceTableRelationshipFieldList = sourceTableRelationshipFieldList;
        }

        public static string Signature => "6d6f64656c7c4d657461526177446174615661756c740a656e746974797c5261774875627c5261774875624c6973740a70726f70657274797c5261774875627c4e616d657c737472696e677c72657175697265640a72656c6174696f6e736869707c5261774875627c536f757263655461626c657c536f757263655461626c6549640a656e746974797c5261774875624b6579506172747c5261774875624b6579506172744c6973740a70726f70657274797c5261774875624b6579506172747c4e616d657c737472696e677c72657175697265640a70726f70657274797c5261774875624b6579506172747c4f7264696e616c7c737472696e677c72657175697265640a72656c6174696f6e736869707c5261774875624b6579506172747c5261774875627c52617748756249640a72656c6174696f6e736869707c5261774875624b6579506172747c536f757263654669656c647c536f757263654669656c6449640a656e746974797c526177487562536174656c6c6974657c526177487562536174656c6c6974654c6973740a70726f70657274797c526177487562536174656c6c6974657c4e616d657c737472696e677c72657175697265640a70726f70657274797c526177487562536174656c6c6974657c536174656c6c6974654b696e647c737472696e677c72657175697265640a72656c6174696f6e736869707c526177487562536174656c6c6974657c5261774875627c52617748756249640a72656c6174696f6e736869707c526177487562536174656c6c6974657c536f757263655461626c657c536f757263655461626c6549640a656e746974797c526177487562536174656c6c6974654174747269627574657c526177487562536174656c6c6974654174747269627574654c6973740a70726f70657274797c526177487562536174656c6c6974654174747269627574657c4e616d657c737472696e677c72657175697265640a70726f70657274797c526177487562536174656c6c6974654174747269627574657c4f7264696e616c7c737472696e677c72657175697265640a72656c6174696f6e736869707c526177487562536174656c6c6974654174747269627574657c526177487562536174656c6c6974657c526177487562536174656c6c69746549640a72656c6174696f6e736869707c526177487562536174656c6c6974654174747269627574657c536f757263654669656c647c536f757263654669656c6449640a656e746974797c5261774c696e6b7c5261774c696e6b4c6973740a70726f70657274797c5261774c696e6b7c4c696e6b4b696e647c737472696e677c72657175697265640a70726f70657274797c5261774c696e6b7c4e616d657c737472696e677c72657175697265640a72656c6174696f6e736869707c5261774c696e6b7c536f757263655461626c6552656c6174696f6e736869707c536f757263655461626c6552656c6174696f6e7368697049640a656e746974797c5261774c696e6b4875627c5261774c696e6b4875624c6973740a70726f70657274797c5261774c696e6b4875627c4f7264696e616c7c737472696e677c72657175697265640a70726f70657274797c5261774c696e6b4875627c526f6c654e616d657c737472696e677c6e756c6c61626c650a72656c6174696f6e736869707c5261774c696e6b4875627c5261774875627c52617748756249640a72656c6174696f6e736869707c5261774c696e6b4875627c5261774c696e6b7c5261774c696e6b49640a656e746974797c5261774c696e6b536174656c6c6974657c5261774c696e6b536174656c6c6974654c6973740a70726f70657274797c5261774c696e6b536174656c6c6974657c4e616d657c737472696e677c72657175697265640a70726f70657274797c5261774c696e6b536174656c6c6974657c536174656c6c6974654b696e647c737472696e677c72657175697265640a72656c6174696f6e736869707c5261774c696e6b536174656c6c6974657c5261774c696e6b7c5261774c696e6b49640a72656c6174696f6e736869707c5261774c696e6b536174656c6c6974657c536f757263655461626c657c536f757263655461626c6549640a656e746974797c5261774c696e6b536174656c6c6974654174747269627574657c5261774c696e6b536174656c6c6974654174747269627574654c6973740a70726f70657274797c5261774c696e6b536174656c6c6974654174747269627574657c4e616d657c737472696e677c72657175697265640a70726f70657274797c5261774c696e6b536174656c6c6974654174747269627574657c4f7264696e616c7c737472696e677c72657175697265640a72656c6174696f6e736869707c5261774c696e6b536174656c6c6974654174747269627574657c5261774c696e6b536174656c6c6974657c5261774c696e6b536174656c6c69746549640a72656c6174696f6e736869707c5261774c696e6b536174656c6c6974654174747269627574657c536f757263654669656c647c536f757263654669656c6449640a656e746974797c536f757263654669656c647c536f757263654669656c644c6973740a70726f70657274797c536f757263654669656c647c446174615479706549647c737472696e677c72657175697265640a70726f70657274797c536f757263654669656c647c49734e756c6c61626c657c737472696e677c6e756c6c61626c650a70726f70657274797c536f757263654669656c647c4e616d657c737472696e677c72657175697265640a70726f70657274797c536f757263654669656c647c4f7264696e616c7c737472696e677c6e756c6c61626c650a72656c6174696f6e736869707c536f757263654669656c647c536f757263655461626c657c536f757263655461626c6549640a656e746974797c536f757263654669656c64446174615479706544657461696c7c536f757263654669656c64446174615479706544657461696c4c6973740a70726f70657274797c536f757263654669656c64446174615479706544657461696c7c4e616d657c737472696e677c72657175697265640a70726f70657274797c536f757263654669656c64446174615479706544657461696c7c56616c75657c737472696e677c72657175697265640a72656c6174696f6e736869707c536f757263654669656c64446174615479706544657461696c7c536f757263654669656c647c536f757263654669656c6449640a656e746974797c536f75726365536368656d617c536f75726365536368656d614c6973740a70726f70657274797c536f75726365536368656d617c4e616d657c737472696e677c72657175697265640a72656c6174696f6e736869707c536f75726365536368656d617c536f7572636553797374656d7c536f7572636553797374656d49640a656e746974797c536f7572636553797374656d7c536f7572636553797374656d4c6973740a70726f70657274797c536f7572636553797374656d7c4465736372697074696f6e7c737472696e677c6e756c6c61626c650a70726f70657274797c536f7572636553797374656d7c4e616d657c737472696e677c72657175697265640a656e746974797c536f757263655461626c657c536f757263655461626c654c6973740a70726f70657274797c536f757263655461626c657c4e616d657c737472696e677c72657175697265640a72656c6174696f6e736869707c536f757263655461626c657c536f75726365536368656d617c536f75726365536368656d6149640a656e746974797c536f757263655461626c6552656c6174696f6e736869707c536f757263655461626c6552656c6174696f6e736869704c6973740a70726f70657274797c536f757263655461626c6552656c6174696f6e736869707c4e616d657c737472696e677c72657175697265640a72656c6174696f6e736869707c536f757263655461626c6552656c6174696f6e736869707c536f757263655461626c657c536f757263655461626c6549640a72656c6174696f6e736869707c536f757263655461626c6552656c6174696f6e736869707c536f757263655461626c657c5461726765745461626c6549640a656e746974797c536f757263655461626c6552656c6174696f6e736869704669656c647c536f757263655461626c6552656c6174696f6e736869704669656c644c6973740a70726f70657274797c536f757263655461626c6552656c6174696f6e736869704669656c647c4f7264696e616c7c737472696e677c72657175697265640a72656c6174696f6e736869707c536f757263655461626c6552656c6174696f6e736869704669656c647c536f757263654669656c647c536f757263654669656c6449640a72656c6174696f6e736869707c536f757263655461626c6552656c6174696f6e736869704669656c647c536f757263655461626c6552656c6174696f6e736869707c536f757263655461626c6552656c6174696f6e7368697049640a72656c6174696f6e736869707c536f757263655461626c6552656c6174696f6e736869704669656c647c536f757263654669656c647c5461726765744669656c644964";

        public static MetaRawDataVaultModel CreateEmpty()
        {
            return new MetaRawDataVaultModel(
                new List<RawHub>(),
                new List<RawHubKeyPart>(),
                new List<RawHubSatellite>(),
                new List<RawHubSatelliteAttribute>(),
                new List<RawLink>(),
                new List<RawLinkHub>(),
                new List<RawLinkSatellite>(),
                new List<RawLinkSatelliteAttribute>(),
                new List<SourceField>(),
                new List<SourceFieldDataTypeDetail>(),
                new List<SourceSchema>(),
                new List<SourceSystem>(),
                new List<SourceTable>(),
                new List<SourceTableRelationship>(),
                new List<SourceTableRelationshipField>()
            );
        }

        public List<RawHub> RawHubList { get; }
        public List<RawHubKeyPart> RawHubKeyPartList { get; }
        public List<RawHubSatellite> RawHubSatelliteList { get; }
        public List<RawHubSatelliteAttribute> RawHubSatelliteAttributeList { get; }
        public List<RawLink> RawLinkList { get; }
        public List<RawLinkHub> RawLinkHubList { get; }
        public List<RawLinkSatellite> RawLinkSatelliteList { get; }
        public List<RawLinkSatelliteAttribute> RawLinkSatelliteAttributeList { get; }
        public List<SourceField> SourceFieldList { get; }
        public List<SourceFieldDataTypeDetail> SourceFieldDataTypeDetailList { get; }
        public List<SourceSchema> SourceSchemaList { get; }
        public List<SourceSystem> SourceSystemList { get; }
        public List<SourceTable> SourceTableList { get; }
        public List<SourceTableRelationship> SourceTableRelationshipList { get; }
        public List<SourceTableRelationshipField> SourceTableRelationshipFieldList { get; }

        public Workspace ToXmlWorkspace(string workspacePath)
        {
            if (string.IsNullOrWhiteSpace(workspacePath))
            {
                throw new global::System.ArgumentException("Workspace path is required.", nameof(workspacePath));
            }

            var rootPath = global::System.IO.Path.GetFullPath(workspacePath);
            var metadataRootPath = global::System.IO.Path.Combine(rootPath, "metadata");
            var model = CreateGenericModelDefinition();
            var workspace = new Workspace
            {
                WorkspaceRootPath = rootPath,
                MetadataRootPath = metadataRootPath,
                WorkspaceConfig = global::Meta.Core.WorkspaceConfig.Generated.MetaWorkspace.CreateDefault(),
                Model = model,
                Instance = new GenericInstance
                {
                    ModelName = model.Name,
                },
                IsDirty = true,
            };

            foreach (var row in RawHubList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawHub.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceTableId))
                {
                    record.RelationshipIds["SourceTableId"] = row.SourceTableId;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawHub").Add(record);
            }

            foreach (var row in RawHubKeyPartList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawHubKeyPart.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.RawHubId))
                {
                    record.RelationshipIds["RawHubId"] = row.RawHubId;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceFieldId))
                {
                    record.RelationshipIds["SourceFieldId"] = row.SourceFieldId;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawHubKeyPart").Add(record);
            }

            foreach (var row in RawHubSatelliteList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawHubSatellite.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.SatelliteKind))
                {
                    record.Values["SatelliteKind"] = row.SatelliteKind;
                }
                if (!string.IsNullOrWhiteSpace(row.RawHubId))
                {
                    record.RelationshipIds["RawHubId"] = row.RawHubId;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceTableId))
                {
                    record.RelationshipIds["SourceTableId"] = row.SourceTableId;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawHubSatellite").Add(record);
            }

            foreach (var row in RawHubSatelliteAttributeList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawHubSatelliteAttribute.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.RawHubSatelliteId))
                {
                    record.RelationshipIds["RawHubSatelliteId"] = row.RawHubSatelliteId;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceFieldId))
                {
                    record.RelationshipIds["SourceFieldId"] = row.SourceFieldId;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawHubSatelliteAttribute").Add(record);
            }

            foreach (var row in RawLinkList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawLink.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.LinkKind))
                {
                    record.Values["LinkKind"] = row.LinkKind;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceTableRelationshipId))
                {
                    record.RelationshipIds["SourceTableRelationshipId"] = row.SourceTableRelationshipId;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawLink").Add(record);
            }

            foreach (var row in RawLinkHubList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawLinkHub.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.RoleName))
                {
                    record.Values["RoleName"] = row.RoleName;
                }
                if (!string.IsNullOrWhiteSpace(row.RawHubId))
                {
                    record.RelationshipIds["RawHubId"] = row.RawHubId;
                }
                if (!string.IsNullOrWhiteSpace(row.RawLinkId))
                {
                    record.RelationshipIds["RawLinkId"] = row.RawLinkId;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawLinkHub").Add(record);
            }

            foreach (var row in RawLinkSatelliteList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawLinkSatellite.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.SatelliteKind))
                {
                    record.Values["SatelliteKind"] = row.SatelliteKind;
                }
                if (!string.IsNullOrWhiteSpace(row.RawLinkId))
                {
                    record.RelationshipIds["RawLinkId"] = row.RawLinkId;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceTableId))
                {
                    record.RelationshipIds["SourceTableId"] = row.SourceTableId;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawLinkSatellite").Add(record);
            }

            foreach (var row in RawLinkSatelliteAttributeList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawLinkSatelliteAttribute.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.RawLinkSatelliteId))
                {
                    record.RelationshipIds["RawLinkSatelliteId"] = row.RawLinkSatelliteId;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceFieldId))
                {
                    record.RelationshipIds["SourceFieldId"] = row.SourceFieldId;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawLinkSatelliteAttribute").Add(record);
            }

            foreach (var row in SourceFieldList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "SourceField.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.DataTypeId))
                {
                    record.Values["DataTypeId"] = row.DataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.IsNullable))
                {
                    record.Values["IsNullable"] = row.IsNullable;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceTableId))
                {
                    record.RelationshipIds["SourceTableId"] = row.SourceTableId;
                }
                workspace.Instance.GetOrCreateEntityRecords("SourceField").Add(record);
            }

            foreach (var row in SourceFieldDataTypeDetailList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "SourceFieldDataTypeDetail.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.Value))
                {
                    record.Values["Value"] = row.Value;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceFieldId))
                {
                    record.RelationshipIds["SourceFieldId"] = row.SourceFieldId;
                }
                workspace.Instance.GetOrCreateEntityRecords("SourceFieldDataTypeDetail").Add(record);
            }

            foreach (var row in SourceSchemaList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "SourceSchema.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceSystemId))
                {
                    record.RelationshipIds["SourceSystemId"] = row.SourceSystemId;
                }
                workspace.Instance.GetOrCreateEntityRecords("SourceSchema").Add(record);
            }

            foreach (var row in SourceSystemList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "SourceSystem.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Description))
                {
                    record.Values["Description"] = row.Description;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                workspace.Instance.GetOrCreateEntityRecords("SourceSystem").Add(record);
            }

            foreach (var row in SourceTableList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "SourceTable.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceSchemaId))
                {
                    record.RelationshipIds["SourceSchemaId"] = row.SourceSchemaId;
                }
                workspace.Instance.GetOrCreateEntityRecords("SourceTable").Add(record);
            }

            foreach (var row in SourceTableRelationshipList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "SourceTableRelationship.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceTableId))
                {
                    record.RelationshipIds["SourceTableId"] = row.SourceTableId;
                }
                if (!string.IsNullOrWhiteSpace(row.TargetTableId))
                {
                    record.RelationshipIds["TargetTableId"] = row.TargetTableId;
                }
                workspace.Instance.GetOrCreateEntityRecords("SourceTableRelationship").Add(record);
            }

            foreach (var row in SourceTableRelationshipFieldList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "SourceTableRelationshipField.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceFieldId))
                {
                    record.RelationshipIds["SourceFieldId"] = row.SourceFieldId;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceTableRelationshipId))
                {
                    record.RelationshipIds["SourceTableRelationshipId"] = row.SourceTableRelationshipId;
                }
                if (!string.IsNullOrWhiteSpace(row.TargetFieldId))
                {
                    record.RelationshipIds["TargetFieldId"] = row.TargetFieldId;
                }
                workspace.Instance.GetOrCreateEntityRecords("SourceTableRelationshipField").Add(record);
            }

            return workspace;
        }

        public Task SaveToXmlWorkspaceAsync(
            string workspacePath,
            CancellationToken cancellationToken = default)
        {
            var workspace = ToXmlWorkspace(workspacePath);
            return MetaRawDataVaultTooling.SaveWorkspaceAsync(workspace, cancellationToken);
        }

        private static GenericModel CreateGenericModelDefinition()
        {
            var model = new GenericModel
            {
                Name = "MetaRawDataVault",
            };

            model.Entities.Add(new GenericEntity
            {
                Name = "RawHub",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "SourceTable",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "RawHubKeyPart",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "Ordinal",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "RawHub",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "SourceField",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "RawHubSatellite",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "SatelliteKind",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "RawHub",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "SourceTable",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "RawHubSatelliteAttribute",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "Ordinal",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "RawHubSatellite",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "SourceField",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "RawLink",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "LinkKind",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "SourceTableRelationship",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "RawLinkHub",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Ordinal",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RoleName",
                        DataType = "string",
                        IsNullable = true,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "RawHub",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "RawLink",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "RawLinkSatellite",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "SatelliteKind",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "RawLink",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "SourceTable",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "RawLinkSatelliteAttribute",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "Ordinal",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "RawLinkSatellite",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "SourceField",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "SourceField",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "DataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "IsNullable",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "Ordinal",
                        DataType = "string",
                        IsNullable = true,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "SourceTable",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "SourceFieldDataTypeDetail",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "Value",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "SourceField",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "SourceSchema",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "SourceSystem",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "SourceSystem",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Description",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "SourceTable",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "SourceSchema",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "SourceTableRelationship",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "SourceTable",
                        Role = "SourceTable",
                    },
                    new GenericRelationship
                    {
                        Entity = "SourceTable",
                        Role = "TargetTable",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "SourceTableRelationshipField",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Ordinal",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "SourceField",
                        Role = "SourceField",
                    },
                    new GenericRelationship
                    {
                        Entity = "SourceTableRelationship",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "SourceField",
                        Role = "TargetField",
                    },
                },
            });

            return model;
        }
    }

    internal static class MetaRawDataVaultModelFactory
    {
        internal static MetaRawDataVaultModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var rawHubList = new List<RawHub>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHub", out var rawHubListRecords))
            {
                foreach (var record in rawHubListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubList.Add(new RawHub
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubKeyPartList = new List<RawHubKeyPart>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHubKeyPart", out var rawHubKeyPartListRecords))
            {
                foreach (var record in rawHubKeyPartListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubKeyPartList.Add(new RawHubKeyPart
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        RawHubId = record.RelationshipIds.TryGetValue("RawHubId", out var rawHubRelationshipId) ? rawHubRelationshipId ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubSatelliteList = new List<RawHubSatellite>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHubSatellite", out var rawHubSatelliteListRecords))
            {
                foreach (var record in rawHubSatelliteListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubSatelliteList.Add(new RawHubSatellite
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SatelliteKind = record.Values.TryGetValue("SatelliteKind", out var satelliteKindValue) ? satelliteKindValue ?? string.Empty : string.Empty,
                        RawHubId = record.RelationshipIds.TryGetValue("RawHubId", out var rawHubRelationshipId) ? rawHubRelationshipId ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubSatelliteAttributeList = new List<RawHubSatelliteAttribute>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHubSatelliteAttribute", out var rawHubSatelliteAttributeListRecords))
            {
                foreach (var record in rawHubSatelliteAttributeListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubSatelliteAttributeList.Add(new RawHubSatelliteAttribute
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        RawHubSatelliteId = record.RelationshipIds.TryGetValue("RawHubSatelliteId", out var rawHubSatelliteRelationshipId) ? rawHubSatelliteRelationshipId ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkList = new List<RawLink>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLink", out var rawLinkListRecords))
            {
                foreach (var record in rawLinkListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkList.Add(new RawLink
                    {
                        Id = record.Id ?? string.Empty,
                        LinkKind = record.Values.TryGetValue("LinkKind", out var linkKindValue) ? linkKindValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceTableRelationshipId = record.RelationshipIds.TryGetValue("SourceTableRelationshipId", out var sourceTableRelationshipRelationshipId) ? sourceTableRelationshipRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkHubList = new List<RawLinkHub>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLinkHub", out var rawLinkHubListRecords))
            {
                foreach (var record in rawLinkHubListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkHubList.Add(new RawLinkHub
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        RoleName = record.Values.TryGetValue("RoleName", out var roleNameValue) ? roleNameValue ?? string.Empty : string.Empty,
                        RawHubId = record.RelationshipIds.TryGetValue("RawHubId", out var rawHubRelationshipId) ? rawHubRelationshipId ?? string.Empty : string.Empty,
                        RawLinkId = record.RelationshipIds.TryGetValue("RawLinkId", out var rawLinkRelationshipId) ? rawLinkRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkSatelliteList = new List<RawLinkSatellite>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLinkSatellite", out var rawLinkSatelliteListRecords))
            {
                foreach (var record in rawLinkSatelliteListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkSatelliteList.Add(new RawLinkSatellite
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SatelliteKind = record.Values.TryGetValue("SatelliteKind", out var satelliteKindValue) ? satelliteKindValue ?? string.Empty : string.Empty,
                        RawLinkId = record.RelationshipIds.TryGetValue("RawLinkId", out var rawLinkRelationshipId) ? rawLinkRelationshipId ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkSatelliteAttributeList = new List<RawLinkSatelliteAttribute>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLinkSatelliteAttribute", out var rawLinkSatelliteAttributeListRecords))
            {
                foreach (var record in rawLinkSatelliteAttributeListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkSatelliteAttributeList.Add(new RawLinkSatelliteAttribute
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        RawLinkSatelliteId = record.RelationshipIds.TryGetValue("RawLinkSatelliteId", out var rawLinkSatelliteRelationshipId) ? rawLinkSatelliteRelationshipId ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceFieldList = new List<SourceField>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceField", out var sourceFieldListRecords))
            {
                foreach (var record in sourceFieldListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceFieldList.Add(new SourceField
                    {
                        Id = record.Id ?? string.Empty,
                        DataTypeId = record.Values.TryGetValue("DataTypeId", out var dataTypeIdValue) ? dataTypeIdValue ?? string.Empty : string.Empty,
                        IsNullable = record.Values.TryGetValue("IsNullable", out var isNullableValue) ? isNullableValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceFieldDataTypeDetailList = new List<SourceFieldDataTypeDetail>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceFieldDataTypeDetail", out var sourceFieldDataTypeDetailListRecords))
            {
                foreach (var record in sourceFieldDataTypeDetailListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceFieldDataTypeDetailList.Add(new SourceFieldDataTypeDetail
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Value = record.Values.TryGetValue("Value", out var valueValue) ? valueValue ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceSchemaList = new List<SourceSchema>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceSchema", out var sourceSchemaListRecords))
            {
                foreach (var record in sourceSchemaListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceSchemaList.Add(new SourceSchema
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceSystemId = record.RelationshipIds.TryGetValue("SourceSystemId", out var sourceSystemRelationshipId) ? sourceSystemRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceSystemList = new List<SourceSystem>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceSystem", out var sourceSystemListRecords))
            {
                foreach (var record in sourceSystemListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceSystemList.Add(new SourceSystem
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceTableList = new List<SourceTable>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceTable", out var sourceTableListRecords))
            {
                foreach (var record in sourceTableListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceTableList.Add(new SourceTable
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceSchemaId = record.RelationshipIds.TryGetValue("SourceSchemaId", out var sourceSchemaRelationshipId) ? sourceSchemaRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceTableRelationshipList = new List<SourceTableRelationship>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceTableRelationship", out var sourceTableRelationshipListRecords))
            {
                foreach (var record in sourceTableRelationshipListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceTableRelationshipList.Add(new SourceTableRelationship
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                        TargetTableId = record.RelationshipIds.TryGetValue("TargetTableId", out var targetTableRelationshipId) ? targetTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceTableRelationshipFieldList = new List<SourceTableRelationshipField>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceTableRelationshipField", out var sourceTableRelationshipFieldListRecords))
            {
                foreach (var record in sourceTableRelationshipFieldListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceTableRelationshipFieldList.Add(new SourceTableRelationshipField
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                        SourceTableRelationshipId = record.RelationshipIds.TryGetValue("SourceTableRelationshipId", out var sourceTableRelationshipRelationshipId) ? sourceTableRelationshipRelationshipId ?? string.Empty : string.Empty,
                        TargetFieldId = record.RelationshipIds.TryGetValue("TargetFieldId", out var targetFieldRelationshipId) ? targetFieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubListById = new Dictionary<string, RawHub>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubList)
            {
                rawHubListById[row.Id] = row;
            }

            var rawHubKeyPartListById = new Dictionary<string, RawHubKeyPart>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubKeyPartList)
            {
                rawHubKeyPartListById[row.Id] = row;
            }

            var rawHubSatelliteListById = new Dictionary<string, RawHubSatellite>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubSatelliteList)
            {
                rawHubSatelliteListById[row.Id] = row;
            }

            var rawHubSatelliteAttributeListById = new Dictionary<string, RawHubSatelliteAttribute>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubSatelliteAttributeList)
            {
                rawHubSatelliteAttributeListById[row.Id] = row;
            }

            var rawLinkListById = new Dictionary<string, RawLink>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkList)
            {
                rawLinkListById[row.Id] = row;
            }

            var rawLinkHubListById = new Dictionary<string, RawLinkHub>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkHubList)
            {
                rawLinkHubListById[row.Id] = row;
            }

            var rawLinkSatelliteListById = new Dictionary<string, RawLinkSatellite>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkSatelliteList)
            {
                rawLinkSatelliteListById[row.Id] = row;
            }

            var rawLinkSatelliteAttributeListById = new Dictionary<string, RawLinkSatelliteAttribute>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkSatelliteAttributeList)
            {
                rawLinkSatelliteAttributeListById[row.Id] = row;
            }

            var sourceFieldListById = new Dictionary<string, SourceField>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceFieldList)
            {
                sourceFieldListById[row.Id] = row;
            }

            var sourceFieldDataTypeDetailListById = new Dictionary<string, SourceFieldDataTypeDetail>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceFieldDataTypeDetailList)
            {
                sourceFieldDataTypeDetailListById[row.Id] = row;
            }

            var sourceSchemaListById = new Dictionary<string, SourceSchema>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceSchemaList)
            {
                sourceSchemaListById[row.Id] = row;
            }

            var sourceSystemListById = new Dictionary<string, SourceSystem>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceSystemList)
            {
                sourceSystemListById[row.Id] = row;
            }

            var sourceTableListById = new Dictionary<string, SourceTable>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceTableList)
            {
                sourceTableListById[row.Id] = row;
            }

            var sourceTableRelationshipListById = new Dictionary<string, SourceTableRelationship>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceTableRelationshipList)
            {
                sourceTableRelationshipListById[row.Id] = row;
            }

            var sourceTableRelationshipFieldListById = new Dictionary<string, SourceTableRelationshipField>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceTableRelationshipFieldList)
            {
                sourceTableRelationshipFieldListById[row.Id] = row;
            }

            foreach (var row in rawHubList)
            {
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "RawHub",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in rawHubKeyPartList)
            {
                row.RawHub = RequireTarget(
                    rawHubListById,
                    row.RawHubId,
                    "RawHubKeyPart",
                    row.Id,
                    "RawHubId");
            }

            foreach (var row in rawHubKeyPartList)
            {
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "RawHubKeyPart",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in rawHubSatelliteList)
            {
                row.RawHub = RequireTarget(
                    rawHubListById,
                    row.RawHubId,
                    "RawHubSatellite",
                    row.Id,
                    "RawHubId");
            }

            foreach (var row in rawHubSatelliteList)
            {
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "RawHubSatellite",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in rawHubSatelliteAttributeList)
            {
                row.RawHubSatellite = RequireTarget(
                    rawHubSatelliteListById,
                    row.RawHubSatelliteId,
                    "RawHubSatelliteAttribute",
                    row.Id,
                    "RawHubSatelliteId");
            }

            foreach (var row in rawHubSatelliteAttributeList)
            {
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "RawHubSatelliteAttribute",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in rawLinkList)
            {
                row.SourceTableRelationship = RequireTarget(
                    sourceTableRelationshipListById,
                    row.SourceTableRelationshipId,
                    "RawLink",
                    row.Id,
                    "SourceTableRelationshipId");
            }

            foreach (var row in rawLinkHubList)
            {
                row.RawHub = RequireTarget(
                    rawHubListById,
                    row.RawHubId,
                    "RawLinkHub",
                    row.Id,
                    "RawHubId");
            }

            foreach (var row in rawLinkHubList)
            {
                row.RawLink = RequireTarget(
                    rawLinkListById,
                    row.RawLinkId,
                    "RawLinkHub",
                    row.Id,
                    "RawLinkId");
            }

            foreach (var row in rawLinkSatelliteList)
            {
                row.RawLink = RequireTarget(
                    rawLinkListById,
                    row.RawLinkId,
                    "RawLinkSatellite",
                    row.Id,
                    "RawLinkId");
            }

            foreach (var row in rawLinkSatelliteList)
            {
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "RawLinkSatellite",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in rawLinkSatelliteAttributeList)
            {
                row.RawLinkSatellite = RequireTarget(
                    rawLinkSatelliteListById,
                    row.RawLinkSatelliteId,
                    "RawLinkSatelliteAttribute",
                    row.Id,
                    "RawLinkSatelliteId");
            }

            foreach (var row in rawLinkSatelliteAttributeList)
            {
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "RawLinkSatelliteAttribute",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in sourceFieldList)
            {
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "SourceField",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in sourceFieldDataTypeDetailList)
            {
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "SourceFieldDataTypeDetail",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in sourceSchemaList)
            {
                row.SourceSystem = RequireTarget(
                    sourceSystemListById,
                    row.SourceSystemId,
                    "SourceSchema",
                    row.Id,
                    "SourceSystemId");
            }

            foreach (var row in sourceTableList)
            {
                row.SourceSchema = RequireTarget(
                    sourceSchemaListById,
                    row.SourceSchemaId,
                    "SourceTable",
                    row.Id,
                    "SourceSchemaId");
            }

            foreach (var row in sourceTableRelationshipList)
            {
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "SourceTableRelationship",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in sourceTableRelationshipList)
            {
                row.TargetTable = RequireTarget(
                    sourceTableListById,
                    row.TargetTableId,
                    "SourceTableRelationship",
                    row.Id,
                    "TargetTableId");
            }

            foreach (var row in sourceTableRelationshipFieldList)
            {
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "SourceTableRelationshipField",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in sourceTableRelationshipFieldList)
            {
                row.SourceTableRelationship = RequireTarget(
                    sourceTableRelationshipListById,
                    row.SourceTableRelationshipId,
                    "SourceTableRelationshipField",
                    row.Id,
                    "SourceTableRelationshipId");
            }

            foreach (var row in sourceTableRelationshipFieldList)
            {
                row.TargetField = RequireTarget(
                    sourceFieldListById,
                    row.TargetFieldId,
                    "SourceTableRelationshipField",
                    row.Id,
                    "TargetFieldId");
            }

            return new MetaRawDataVaultModel(
                rawHubList,
                rawHubKeyPartList,
                rawHubSatelliteList,
                rawHubSatelliteAttributeList,
                rawLinkList,
                rawLinkHubList,
                rawLinkSatelliteList,
                rawLinkSatelliteAttributeList,
                sourceFieldList,
                sourceFieldDataTypeDetailList,
                sourceSchemaList,
                sourceSystemList,
                sourceTableList,
                sourceTableRelationshipList,
                sourceTableRelationshipFieldList
            );
        }

        private static T RequireTarget<T>(
            Dictionary<string, T> rowsById,
            string targetId,
            string sourceEntityName,
            string sourceId,
            string relationshipName)
            where T : class
        {
            if (string.IsNullOrEmpty(targetId))
            {
                throw new global::System.InvalidOperationException(
                    $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' is empty."
                );
            }

            if (!rowsById.TryGetValue(targetId, out var target))
            {
                throw new global::System.InvalidOperationException(
                    $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' points to missing Id '{targetId}'."
                );
            }

            return target;
        }
    }
}
