using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meta.Core.Domain;

namespace MetaDataVaultImplementation
{
    public sealed partial class MetaDataVaultImplementationModel
    {
        internal MetaDataVaultImplementationModel(
            List<BusinessBridgeImplementation> businessBridgeImplementationList,
            List<BusinessHierarchicalLinkImplementation> businessHierarchicalLinkImplementationList,
            List<BusinessHierarchicalLinkSatelliteImplementation> businessHierarchicalLinkSatelliteImplementationList,
            List<BusinessHubImplementation> businessHubImplementationList,
            List<BusinessHubSatelliteImplementation> businessHubSatelliteImplementationList,
            List<BusinessLinkImplementation> businessLinkImplementationList,
            List<BusinessLinkSatelliteImplementation> businessLinkSatelliteImplementationList,
            List<BusinessPointInTimeImplementation> businessPointInTimeImplementationList,
            List<BusinessReferenceImplementation> businessReferenceImplementationList,
            List<BusinessReferenceSatelliteImplementation> businessReferenceSatelliteImplementationList,
            List<BusinessSameAsLinkImplementation> businessSameAsLinkImplementationList,
            List<BusinessSameAsLinkSatelliteImplementation> businessSameAsLinkSatelliteImplementationList,
            List<RawHubImplementation> rawHubImplementationList,
            List<RawHubSatelliteImplementation> rawHubSatelliteImplementationList,
            List<RawLinkImplementation> rawLinkImplementationList,
            List<RawLinkSatelliteImplementation> rawLinkSatelliteImplementationList
        )
        {
            BusinessBridgeImplementationList = businessBridgeImplementationList;
            BusinessHierarchicalLinkImplementationList = businessHierarchicalLinkImplementationList;
            BusinessHierarchicalLinkSatelliteImplementationList = businessHierarchicalLinkSatelliteImplementationList;
            BusinessHubImplementationList = businessHubImplementationList;
            BusinessHubSatelliteImplementationList = businessHubSatelliteImplementationList;
            BusinessLinkImplementationList = businessLinkImplementationList;
            BusinessLinkSatelliteImplementationList = businessLinkSatelliteImplementationList;
            BusinessPointInTimeImplementationList = businessPointInTimeImplementationList;
            BusinessReferenceImplementationList = businessReferenceImplementationList;
            BusinessReferenceSatelliteImplementationList = businessReferenceSatelliteImplementationList;
            BusinessSameAsLinkImplementationList = businessSameAsLinkImplementationList;
            BusinessSameAsLinkSatelliteImplementationList = businessSameAsLinkSatelliteImplementationList;
            RawHubImplementationList = rawHubImplementationList;
            RawHubSatelliteImplementationList = rawHubSatelliteImplementationList;
            RawLinkImplementationList = rawLinkImplementationList;
            RawLinkSatelliteImplementationList = rawLinkSatelliteImplementationList;
        }

        public static MetaDataVaultImplementationModel CreateEmpty()
        {
            return new MetaDataVaultImplementationModel(
                new List<BusinessBridgeImplementation>(),
                new List<BusinessHierarchicalLinkImplementation>(),
                new List<BusinessHierarchicalLinkSatelliteImplementation>(),
                new List<BusinessHubImplementation>(),
                new List<BusinessHubSatelliteImplementation>(),
                new List<BusinessLinkImplementation>(),
                new List<BusinessLinkSatelliteImplementation>(),
                new List<BusinessPointInTimeImplementation>(),
                new List<BusinessReferenceImplementation>(),
                new List<BusinessReferenceSatelliteImplementation>(),
                new List<BusinessSameAsLinkImplementation>(),
                new List<BusinessSameAsLinkSatelliteImplementation>(),
                new List<RawHubImplementation>(),
                new List<RawHubSatelliteImplementation>(),
                new List<RawLinkImplementation>(),
                new List<RawLinkSatelliteImplementation>()
            );
        }

        public List<BusinessBridgeImplementation> BusinessBridgeImplementationList { get; }
        public List<BusinessHierarchicalLinkImplementation> BusinessHierarchicalLinkImplementationList { get; }
        public List<BusinessHierarchicalLinkSatelliteImplementation> BusinessHierarchicalLinkSatelliteImplementationList { get; }
        public List<BusinessHubImplementation> BusinessHubImplementationList { get; }
        public List<BusinessHubSatelliteImplementation> BusinessHubSatelliteImplementationList { get; }
        public List<BusinessLinkImplementation> BusinessLinkImplementationList { get; }
        public List<BusinessLinkSatelliteImplementation> BusinessLinkSatelliteImplementationList { get; }
        public List<BusinessPointInTimeImplementation> BusinessPointInTimeImplementationList { get; }
        public List<BusinessReferenceImplementation> BusinessReferenceImplementationList { get; }
        public List<BusinessReferenceSatelliteImplementation> BusinessReferenceSatelliteImplementationList { get; }
        public List<BusinessSameAsLinkImplementation> BusinessSameAsLinkImplementationList { get; }
        public List<BusinessSameAsLinkSatelliteImplementation> BusinessSameAsLinkSatelliteImplementationList { get; }
        public List<RawHubImplementation> RawHubImplementationList { get; }
        public List<RawHubSatelliteImplementation> RawHubSatelliteImplementationList { get; }
        public List<RawLinkImplementation> RawLinkImplementationList { get; }
        public List<RawLinkSatelliteImplementation> RawLinkSatelliteImplementationList { get; }

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

            foreach (var row in BusinessBridgeImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessBridgeImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.DepthColumnName))
                {
                    record.Values["DepthColumnName"] = row.DepthColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.DepthDataTypeId))
                {
                    record.Values["DepthDataTypeId"] = row.DepthDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.EffectiveFromColumnName))
                {
                    record.Values["EffectiveFromColumnName"] = row.EffectiveFromColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.EffectiveFromDataTypeId))
                {
                    record.Values["EffectiveFromDataTypeId"] = row.EffectiveFromDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.EffectiveFromPrecision))
                {
                    record.Values["EffectiveFromPrecision"] = row.EffectiveFromPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.EffectiveToColumnName))
                {
                    record.Values["EffectiveToColumnName"] = row.EffectiveToColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.EffectiveToDataTypeId))
                {
                    record.Values["EffectiveToDataTypeId"] = row.EffectiveToDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.EffectiveToPrecision))
                {
                    record.Values["EffectiveToPrecision"] = row.EffectiveToPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.PathColumnName))
                {
                    record.Values["PathColumnName"] = row.PathColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.PathDataTypeId))
                {
                    record.Values["PathDataTypeId"] = row.PathDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.PathLength))
                {
                    record.Values["PathLength"] = row.PathLength;
                }
                if (!string.IsNullOrWhiteSpace(row.RelatedHashKeyColumnName))
                {
                    record.Values["RelatedHashKeyColumnName"] = row.RelatedHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RelatedHashKeyDataTypeId))
                {
                    record.Values["RelatedHashKeyDataTypeId"] = row.RelatedHashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RelatedHashKeyLength))
                {
                    record.Values["RelatedHashKeyLength"] = row.RelatedHashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.RootHashKeyColumnName))
                {
                    record.Values["RootHashKeyColumnName"] = row.RootHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RootHashKeyDataTypeId))
                {
                    record.Values["RootHashKeyDataTypeId"] = row.RootHashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RootHashKeyLength))
                {
                    record.Values["RootHashKeyLength"] = row.RootHashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessBridgeImplementation").Add(record);
            }

            foreach (var row in BusinessHierarchicalLinkImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessHierarchicalLinkImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.ChildHashKeyColumnName))
                {
                    record.Values["ChildHashKeyColumnName"] = row.ChildHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyColumnName))
                {
                    record.Values["HashKeyColumnName"] = row.HashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyDataTypeId))
                {
                    record.Values["HashKeyDataTypeId"] = row.HashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyLength))
                {
                    record.Values["HashKeyLength"] = row.HashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyColumnName))
                {
                    record.Values["ParentHashKeyColumnName"] = row.ParentHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessHierarchicalLinkImplementation").Add(record);
            }

            foreach (var row in BusinessHierarchicalLinkSatelliteImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessHierarchicalLinkSatelliteImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffColumnName))
                {
                    record.Values["HashDiffColumnName"] = row.HashDiffColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffDataTypeId))
                {
                    record.Values["HashDiffDataTypeId"] = row.HashDiffDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffLength))
                {
                    record.Values["HashDiffLength"] = row.HashDiffLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyColumnName))
                {
                    record.Values["ParentHashKeyColumnName"] = row.ParentHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyDataTypeId))
                {
                    record.Values["ParentHashKeyDataTypeId"] = row.ParentHashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyLength))
                {
                    record.Values["ParentHashKeyLength"] = row.ParentHashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessHierarchicalLinkSatelliteImplementation").Add(record);
            }

            foreach (var row in BusinessHubImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessHubImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyColumnName))
                {
                    record.Values["HashKeyColumnName"] = row.HashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyDataTypeId))
                {
                    record.Values["HashKeyDataTypeId"] = row.HashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyLength))
                {
                    record.Values["HashKeyLength"] = row.HashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessHubImplementation").Add(record);
            }

            foreach (var row in BusinessHubSatelliteImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessHubSatelliteImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffColumnName))
                {
                    record.Values["HashDiffColumnName"] = row.HashDiffColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffDataTypeId))
                {
                    record.Values["HashDiffDataTypeId"] = row.HashDiffDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffLength))
                {
                    record.Values["HashDiffLength"] = row.HashDiffLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyColumnName))
                {
                    record.Values["ParentHashKeyColumnName"] = row.ParentHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyDataTypeId))
                {
                    record.Values["ParentHashKeyDataTypeId"] = row.ParentHashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyLength))
                {
                    record.Values["ParentHashKeyLength"] = row.ParentHashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessHubSatelliteImplementation").Add(record);
            }

            foreach (var row in BusinessLinkImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessLinkImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.EndHashKeyColumnPattern))
                {
                    record.Values["EndHashKeyColumnPattern"] = row.EndHashKeyColumnPattern;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyColumnName))
                {
                    record.Values["HashKeyColumnName"] = row.HashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyDataTypeId))
                {
                    record.Values["HashKeyDataTypeId"] = row.HashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyLength))
                {
                    record.Values["HashKeyLength"] = row.HashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessLinkImplementation").Add(record);
            }

            foreach (var row in BusinessLinkSatelliteImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessLinkSatelliteImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffColumnName))
                {
                    record.Values["HashDiffColumnName"] = row.HashDiffColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffDataTypeId))
                {
                    record.Values["HashDiffDataTypeId"] = row.HashDiffDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffLength))
                {
                    record.Values["HashDiffLength"] = row.HashDiffLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyColumnName))
                {
                    record.Values["ParentHashKeyColumnName"] = row.ParentHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyDataTypeId))
                {
                    record.Values["ParentHashKeyDataTypeId"] = row.ParentHashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyLength))
                {
                    record.Values["ParentHashKeyLength"] = row.ParentHashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessLinkSatelliteImplementation").Add(record);
            }

            foreach (var row in BusinessPointInTimeImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessPointInTimeImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyColumnName))
                {
                    record.Values["ParentHashKeyColumnName"] = row.ParentHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyDataTypeId))
                {
                    record.Values["ParentHashKeyDataTypeId"] = row.ParentHashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyLength))
                {
                    record.Values["ParentHashKeyLength"] = row.ParentHashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.SatelliteReferenceColumnNamePattern))
                {
                    record.Values["SatelliteReferenceColumnNamePattern"] = row.SatelliteReferenceColumnNamePattern;
                }
                if (!string.IsNullOrWhiteSpace(row.SatelliteReferenceDataTypeId))
                {
                    record.Values["SatelliteReferenceDataTypeId"] = row.SatelliteReferenceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.SatelliteReferencePrecision))
                {
                    record.Values["SatelliteReferencePrecision"] = row.SatelliteReferencePrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.SnapshotTimestampColumnName))
                {
                    record.Values["SnapshotTimestampColumnName"] = row.SnapshotTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.SnapshotTimestampDataTypeId))
                {
                    record.Values["SnapshotTimestampDataTypeId"] = row.SnapshotTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.SnapshotTimestampPrecision))
                {
                    record.Values["SnapshotTimestampPrecision"] = row.SnapshotTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessPointInTimeImplementation").Add(record);
            }

            foreach (var row in BusinessReferenceImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessReferenceImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyColumnName))
                {
                    record.Values["HashKeyColumnName"] = row.HashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyDataTypeId))
                {
                    record.Values["HashKeyDataTypeId"] = row.HashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyLength))
                {
                    record.Values["HashKeyLength"] = row.HashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessReferenceImplementation").Add(record);
            }

            foreach (var row in BusinessReferenceSatelliteImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessReferenceSatelliteImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffColumnName))
                {
                    record.Values["HashDiffColumnName"] = row.HashDiffColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffDataTypeId))
                {
                    record.Values["HashDiffDataTypeId"] = row.HashDiffDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffLength))
                {
                    record.Values["HashDiffLength"] = row.HashDiffLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyColumnName))
                {
                    record.Values["ParentHashKeyColumnName"] = row.ParentHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyDataTypeId))
                {
                    record.Values["ParentHashKeyDataTypeId"] = row.ParentHashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyLength))
                {
                    record.Values["ParentHashKeyLength"] = row.ParentHashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessReferenceSatelliteImplementation").Add(record);
            }

            foreach (var row in BusinessSameAsLinkImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessSameAsLinkImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.EquivalentHashKeyColumnName))
                {
                    record.Values["EquivalentHashKeyColumnName"] = row.EquivalentHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyColumnName))
                {
                    record.Values["HashKeyColumnName"] = row.HashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyDataTypeId))
                {
                    record.Values["HashKeyDataTypeId"] = row.HashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyLength))
                {
                    record.Values["HashKeyLength"] = row.HashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.PrimaryHashKeyColumnName))
                {
                    record.Values["PrimaryHashKeyColumnName"] = row.PrimaryHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessSameAsLinkImplementation").Add(record);
            }

            foreach (var row in BusinessSameAsLinkSatelliteImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "BusinessSameAsLinkSatelliteImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffColumnName))
                {
                    record.Values["HashDiffColumnName"] = row.HashDiffColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffDataTypeId))
                {
                    record.Values["HashDiffDataTypeId"] = row.HashDiffDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffLength))
                {
                    record.Values["HashDiffLength"] = row.HashDiffLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyColumnName))
                {
                    record.Values["ParentHashKeyColumnName"] = row.ParentHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyDataTypeId))
                {
                    record.Values["ParentHashKeyDataTypeId"] = row.ParentHashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyLength))
                {
                    record.Values["ParentHashKeyLength"] = row.ParentHashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("BusinessSameAsLinkSatelliteImplementation").Add(record);
            }

            foreach (var row in RawHubImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawHubImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyColumnName))
                {
                    record.Values["HashKeyColumnName"] = row.HashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyDataTypeId))
                {
                    record.Values["HashKeyDataTypeId"] = row.HashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyLength))
                {
                    record.Values["HashKeyLength"] = row.HashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawHubImplementation").Add(record);
            }

            foreach (var row in RawHubSatelliteImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawHubSatelliteImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffColumnName))
                {
                    record.Values["HashDiffColumnName"] = row.HashDiffColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffDataTypeId))
                {
                    record.Values["HashDiffDataTypeId"] = row.HashDiffDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffLength))
                {
                    record.Values["HashDiffLength"] = row.HashDiffLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyColumnName))
                {
                    record.Values["ParentHashKeyColumnName"] = row.ParentHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyDataTypeId))
                {
                    record.Values["ParentHashKeyDataTypeId"] = row.ParentHashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyLength))
                {
                    record.Values["ParentHashKeyLength"] = row.ParentHashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawHubSatelliteImplementation").Add(record);
            }

            foreach (var row in RawLinkImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawLinkImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.EndHashKeyColumnPattern))
                {
                    record.Values["EndHashKeyColumnPattern"] = row.EndHashKeyColumnPattern;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyColumnName))
                {
                    record.Values["HashKeyColumnName"] = row.HashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyDataTypeId))
                {
                    record.Values["HashKeyDataTypeId"] = row.HashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashKeyLength))
                {
                    record.Values["HashKeyLength"] = row.HashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawLinkImplementation").Add(record);
            }

            foreach (var row in RawLinkSatelliteImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "RawLinkSatelliteImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.AuditIdColumnName))
                {
                    record.Values["AuditIdColumnName"] = row.AuditIdColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.AuditIdDataTypeId))
                {
                    record.Values["AuditIdDataTypeId"] = row.AuditIdDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffColumnName))
                {
                    record.Values["HashDiffColumnName"] = row.HashDiffColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffDataTypeId))
                {
                    record.Values["HashDiffDataTypeId"] = row.HashDiffDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.HashDiffLength))
                {
                    record.Values["HashDiffLength"] = row.HashDiffLength;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampColumnName))
                {
                    record.Values["LoadTimestampColumnName"] = row.LoadTimestampColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampDataTypeId))
                {
                    record.Values["LoadTimestampDataTypeId"] = row.LoadTimestampDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.LoadTimestampPrecision))
                {
                    record.Values["LoadTimestampPrecision"] = row.LoadTimestampPrecision;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyColumnName))
                {
                    record.Values["ParentHashKeyColumnName"] = row.ParentHashKeyColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyDataTypeId))
                {
                    record.Values["ParentHashKeyDataTypeId"] = row.ParentHashKeyDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.ParentHashKeyLength))
                {
                    record.Values["ParentHashKeyLength"] = row.ParentHashKeyLength;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceColumnName))
                {
                    record.Values["RecordSourceColumnName"] = row.RecordSourceColumnName;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceDataTypeId))
                {
                    record.Values["RecordSourceDataTypeId"] = row.RecordSourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.RecordSourceLength))
                {
                    record.Values["RecordSourceLength"] = row.RecordSourceLength;
                }
                if (!string.IsNullOrWhiteSpace(row.TableNamePattern))
                {
                    record.Values["TableNamePattern"] = row.TableNamePattern;
                }
                workspace.Instance.GetOrCreateEntityRecords("RawLinkSatelliteImplementation").Add(record);
            }

            return workspace;
        }

        public Task SaveToXmlWorkspaceAsync(
            string workspacePath,
            CancellationToken cancellationToken = default)
        {
            var workspace = ToXmlWorkspace(workspacePath);
            return MetaDataVaultImplementationTooling.SaveWorkspaceAsync(workspace, cancellationToken);
        }

        private static GenericModel CreateGenericModelDefinition()
        {
            var model = new GenericModel
            {
                Name = "MetaDataVaultImplementation",
            };

            model.Entities.Add(new GenericEntity
            {
                Name = "BusinessBridgeImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "DepthColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "DepthDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "EffectiveFromColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "EffectiveFromDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "EffectiveFromPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "EffectiveToColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "EffectiveToDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "EffectiveToPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "PathColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "PathDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "PathLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RelatedHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RelatedHashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RelatedHashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RootHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RootHashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RootHashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "BusinessHierarchicalLinkImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ChildHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "BusinessHierarchicalLinkSatelliteImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "BusinessHubImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "BusinessHubSatelliteImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "BusinessLinkImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "EndHashKeyColumnPattern",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "BusinessLinkSatelliteImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "BusinessPointInTimeImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "SatelliteReferenceColumnNamePattern",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "SatelliteReferenceDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "SatelliteReferencePrecision",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "SnapshotTimestampColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "SnapshotTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "SnapshotTimestampPrecision",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "BusinessReferenceImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "BusinessReferenceSatelliteImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "BusinessSameAsLinkImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "EquivalentHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "PrimaryHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "BusinessSameAsLinkSatelliteImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "RawHubImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "RawHubSatelliteImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "RawLinkImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "EndHashKeyColumnPattern",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
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
                Name = "RawLinkSatelliteImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "AuditIdColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "AuditIdDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "HashDiffLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "LoadTimestampPrecision",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ParentHashKeyLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceColumnName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "RecordSourceLength",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "TableNamePattern",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                },
            });

            return model;
        }
    }

    internal static class MetaDataVaultImplementationModelFactory
    {
        internal static MetaDataVaultImplementationModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var businessBridgeImplementationList = new List<BusinessBridgeImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessBridgeImplementation", out var businessBridgeImplementationListRecords))
            {
                foreach (var record in businessBridgeImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessBridgeImplementationList.Add(new BusinessBridgeImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        DepthColumnName = record.Values.TryGetValue("DepthColumnName", out var depthColumnNameValue) ? depthColumnNameValue ?? string.Empty : string.Empty,
                        DepthDataTypeId = record.Values.TryGetValue("DepthDataTypeId", out var depthDataTypeIdValue) ? depthDataTypeIdValue ?? string.Empty : string.Empty,
                        EffectiveFromColumnName = record.Values.TryGetValue("EffectiveFromColumnName", out var effectiveFromColumnNameValue) ? effectiveFromColumnNameValue ?? string.Empty : string.Empty,
                        EffectiveFromDataTypeId = record.Values.TryGetValue("EffectiveFromDataTypeId", out var effectiveFromDataTypeIdValue) ? effectiveFromDataTypeIdValue ?? string.Empty : string.Empty,
                        EffectiveFromPrecision = record.Values.TryGetValue("EffectiveFromPrecision", out var effectiveFromPrecisionValue) ? effectiveFromPrecisionValue ?? string.Empty : string.Empty,
                        EffectiveToColumnName = record.Values.TryGetValue("EffectiveToColumnName", out var effectiveToColumnNameValue) ? effectiveToColumnNameValue ?? string.Empty : string.Empty,
                        EffectiveToDataTypeId = record.Values.TryGetValue("EffectiveToDataTypeId", out var effectiveToDataTypeIdValue) ? effectiveToDataTypeIdValue ?? string.Empty : string.Empty,
                        EffectiveToPrecision = record.Values.TryGetValue("EffectiveToPrecision", out var effectiveToPrecisionValue) ? effectiveToPrecisionValue ?? string.Empty : string.Empty,
                        PathColumnName = record.Values.TryGetValue("PathColumnName", out var pathColumnNameValue) ? pathColumnNameValue ?? string.Empty : string.Empty,
                        PathDataTypeId = record.Values.TryGetValue("PathDataTypeId", out var pathDataTypeIdValue) ? pathDataTypeIdValue ?? string.Empty : string.Empty,
                        PathLength = record.Values.TryGetValue("PathLength", out var pathLengthValue) ? pathLengthValue ?? string.Empty : string.Empty,
                        RelatedHashKeyColumnName = record.Values.TryGetValue("RelatedHashKeyColumnName", out var relatedHashKeyColumnNameValue) ? relatedHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        RelatedHashKeyDataTypeId = record.Values.TryGetValue("RelatedHashKeyDataTypeId", out var relatedHashKeyDataTypeIdValue) ? relatedHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        RelatedHashKeyLength = record.Values.TryGetValue("RelatedHashKeyLength", out var relatedHashKeyLengthValue) ? relatedHashKeyLengthValue ?? string.Empty : string.Empty,
                        RootHashKeyColumnName = record.Values.TryGetValue("RootHashKeyColumnName", out var rootHashKeyColumnNameValue) ? rootHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        RootHashKeyDataTypeId = record.Values.TryGetValue("RootHashKeyDataTypeId", out var rootHashKeyDataTypeIdValue) ? rootHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        RootHashKeyLength = record.Values.TryGetValue("RootHashKeyLength", out var rootHashKeyLengthValue) ? rootHashKeyLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHierarchicalLinkImplementationList = new List<BusinessHierarchicalLinkImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHierarchicalLinkImplementation", out var businessHierarchicalLinkImplementationListRecords))
            {
                foreach (var record in businessHierarchicalLinkImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHierarchicalLinkImplementationList.Add(new BusinessHierarchicalLinkImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        ChildHashKeyColumnName = record.Values.TryGetValue("ChildHashKeyColumnName", out var childHashKeyColumnNameValue) ? childHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHierarchicalLinkSatelliteImplementationList = new List<BusinessHierarchicalLinkSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHierarchicalLinkSatelliteImplementation", out var businessHierarchicalLinkSatelliteImplementationListRecords))
            {
                foreach (var record in businessHierarchicalLinkSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHierarchicalLinkSatelliteImplementationList.Add(new BusinessHierarchicalLinkSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubImplementationList = new List<BusinessHubImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHubImplementation", out var businessHubImplementationListRecords))
            {
                foreach (var record in businessHubImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubImplementationList.Add(new BusinessHubImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubSatelliteImplementationList = new List<BusinessHubSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHubSatelliteImplementation", out var businessHubSatelliteImplementationListRecords))
            {
                foreach (var record in businessHubSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubSatelliteImplementationList.Add(new BusinessHubSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessLinkImplementationList = new List<BusinessLinkImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessLinkImplementation", out var businessLinkImplementationListRecords))
            {
                foreach (var record in businessLinkImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessLinkImplementationList.Add(new BusinessLinkImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        EndHashKeyColumnPattern = record.Values.TryGetValue("EndHashKeyColumnPattern", out var endHashKeyColumnPatternValue) ? endHashKeyColumnPatternValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessLinkSatelliteImplementationList = new List<BusinessLinkSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessLinkSatelliteImplementation", out var businessLinkSatelliteImplementationListRecords))
            {
                foreach (var record in businessLinkSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessLinkSatelliteImplementationList.Add(new BusinessLinkSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessPointInTimeImplementationList = new List<BusinessPointInTimeImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessPointInTimeImplementation", out var businessPointInTimeImplementationListRecords))
            {
                foreach (var record in businessPointInTimeImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessPointInTimeImplementationList.Add(new BusinessPointInTimeImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        SatelliteReferenceColumnNamePattern = record.Values.TryGetValue("SatelliteReferenceColumnNamePattern", out var satelliteReferenceColumnNamePatternValue) ? satelliteReferenceColumnNamePatternValue ?? string.Empty : string.Empty,
                        SatelliteReferenceDataTypeId = record.Values.TryGetValue("SatelliteReferenceDataTypeId", out var satelliteReferenceDataTypeIdValue) ? satelliteReferenceDataTypeIdValue ?? string.Empty : string.Empty,
                        SatelliteReferencePrecision = record.Values.TryGetValue("SatelliteReferencePrecision", out var satelliteReferencePrecisionValue) ? satelliteReferencePrecisionValue ?? string.Empty : string.Empty,
                        SnapshotTimestampColumnName = record.Values.TryGetValue("SnapshotTimestampColumnName", out var snapshotTimestampColumnNameValue) ? snapshotTimestampColumnNameValue ?? string.Empty : string.Empty,
                        SnapshotTimestampDataTypeId = record.Values.TryGetValue("SnapshotTimestampDataTypeId", out var snapshotTimestampDataTypeIdValue) ? snapshotTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        SnapshotTimestampPrecision = record.Values.TryGetValue("SnapshotTimestampPrecision", out var snapshotTimestampPrecisionValue) ? snapshotTimestampPrecisionValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessReferenceImplementationList = new List<BusinessReferenceImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessReferenceImplementation", out var businessReferenceImplementationListRecords))
            {
                foreach (var record in businessReferenceImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessReferenceImplementationList.Add(new BusinessReferenceImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessReferenceSatelliteImplementationList = new List<BusinessReferenceSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessReferenceSatelliteImplementation", out var businessReferenceSatelliteImplementationListRecords))
            {
                foreach (var record in businessReferenceSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessReferenceSatelliteImplementationList.Add(new BusinessReferenceSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessSameAsLinkImplementationList = new List<BusinessSameAsLinkImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessSameAsLinkImplementation", out var businessSameAsLinkImplementationListRecords))
            {
                foreach (var record in businessSameAsLinkImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessSameAsLinkImplementationList.Add(new BusinessSameAsLinkImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        EquivalentHashKeyColumnName = record.Values.TryGetValue("EquivalentHashKeyColumnName", out var equivalentHashKeyColumnNameValue) ? equivalentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        PrimaryHashKeyColumnName = record.Values.TryGetValue("PrimaryHashKeyColumnName", out var primaryHashKeyColumnNameValue) ? primaryHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessSameAsLinkSatelliteImplementationList = new List<BusinessSameAsLinkSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessSameAsLinkSatelliteImplementation", out var businessSameAsLinkSatelliteImplementationListRecords))
            {
                foreach (var record in businessSameAsLinkSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessSameAsLinkSatelliteImplementationList.Add(new BusinessSameAsLinkSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubImplementationList = new List<RawHubImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHubImplementation", out var rawHubImplementationListRecords))
            {
                foreach (var record in rawHubImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubImplementationList.Add(new RawHubImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubSatelliteImplementationList = new List<RawHubSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHubSatelliteImplementation", out var rawHubSatelliteImplementationListRecords))
            {
                foreach (var record in rawHubSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubSatelliteImplementationList.Add(new RawHubSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkImplementationList = new List<RawLinkImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLinkImplementation", out var rawLinkImplementationListRecords))
            {
                foreach (var record in rawLinkImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkImplementationList.Add(new RawLinkImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        EndHashKeyColumnPattern = record.Values.TryGetValue("EndHashKeyColumnPattern", out var endHashKeyColumnPatternValue) ? endHashKeyColumnPatternValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkSatelliteImplementationList = new List<RawLinkSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLinkSatelliteImplementation", out var rawLinkSatelliteImplementationListRecords))
            {
                foreach (var record in rawLinkSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkSatelliteImplementationList.Add(new RawLinkSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessBridgeImplementationListById = new Dictionary<string, BusinessBridgeImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessBridgeImplementationList)
            {
                businessBridgeImplementationListById[row.Id] = row;
            }

            var businessHierarchicalLinkImplementationListById = new Dictionary<string, BusinessHierarchicalLinkImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHierarchicalLinkImplementationList)
            {
                businessHierarchicalLinkImplementationListById[row.Id] = row;
            }

            var businessHierarchicalLinkSatelliteImplementationListById = new Dictionary<string, BusinessHierarchicalLinkSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHierarchicalLinkSatelliteImplementationList)
            {
                businessHierarchicalLinkSatelliteImplementationListById[row.Id] = row;
            }

            var businessHubImplementationListById = new Dictionary<string, BusinessHubImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubImplementationList)
            {
                businessHubImplementationListById[row.Id] = row;
            }

            var businessHubSatelliteImplementationListById = new Dictionary<string, BusinessHubSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubSatelliteImplementationList)
            {
                businessHubSatelliteImplementationListById[row.Id] = row;
            }

            var businessLinkImplementationListById = new Dictionary<string, BusinessLinkImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessLinkImplementationList)
            {
                businessLinkImplementationListById[row.Id] = row;
            }

            var businessLinkSatelliteImplementationListById = new Dictionary<string, BusinessLinkSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessLinkSatelliteImplementationList)
            {
                businessLinkSatelliteImplementationListById[row.Id] = row;
            }

            var businessPointInTimeImplementationListById = new Dictionary<string, BusinessPointInTimeImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessPointInTimeImplementationList)
            {
                businessPointInTimeImplementationListById[row.Id] = row;
            }

            var businessReferenceImplementationListById = new Dictionary<string, BusinessReferenceImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessReferenceImplementationList)
            {
                businessReferenceImplementationListById[row.Id] = row;
            }

            var businessReferenceSatelliteImplementationListById = new Dictionary<string, BusinessReferenceSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessReferenceSatelliteImplementationList)
            {
                businessReferenceSatelliteImplementationListById[row.Id] = row;
            }

            var businessSameAsLinkImplementationListById = new Dictionary<string, BusinessSameAsLinkImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessSameAsLinkImplementationList)
            {
                businessSameAsLinkImplementationListById[row.Id] = row;
            }

            var businessSameAsLinkSatelliteImplementationListById = new Dictionary<string, BusinessSameAsLinkSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessSameAsLinkSatelliteImplementationList)
            {
                businessSameAsLinkSatelliteImplementationListById[row.Id] = row;
            }

            var rawHubImplementationListById = new Dictionary<string, RawHubImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubImplementationList)
            {
                rawHubImplementationListById[row.Id] = row;
            }

            var rawHubSatelliteImplementationListById = new Dictionary<string, RawHubSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubSatelliteImplementationList)
            {
                rawHubSatelliteImplementationListById[row.Id] = row;
            }

            var rawLinkImplementationListById = new Dictionary<string, RawLinkImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkImplementationList)
            {
                rawLinkImplementationListById[row.Id] = row;
            }

            var rawLinkSatelliteImplementationListById = new Dictionary<string, RawLinkSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkSatelliteImplementationList)
            {
                rawLinkSatelliteImplementationListById[row.Id] = row;
            }

            return new MetaDataVaultImplementationModel(
                businessBridgeImplementationList,
                businessHierarchicalLinkImplementationList,
                businessHierarchicalLinkSatelliteImplementationList,
                businessHubImplementationList,
                businessHubSatelliteImplementationList,
                businessLinkImplementationList,
                businessLinkSatelliteImplementationList,
                businessPointInTimeImplementationList,
                businessReferenceImplementationList,
                businessReferenceSatelliteImplementationList,
                businessSameAsLinkImplementationList,
                businessSameAsLinkSatelliteImplementationList,
                rawHubImplementationList,
                rawHubSatelliteImplementationList,
                rawLinkImplementationList,
                rawLinkSatelliteImplementationList
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
