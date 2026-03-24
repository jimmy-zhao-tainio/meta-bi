using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Meta.Core.Serialization;

namespace MetaDataType
{
    [XmlRoot("MetaDataType")]
    public sealed partial class MetaDataTypeModel
    {
        public static MetaDataTypeModel CreateEmpty() => new();

        [XmlArray("DataTypeList")]
        [XmlArrayItem("DataType")]
        public List<DataType> DataTypeList { get; set; } = new();
        public bool ShouldSerializeDataTypeList() => DataTypeList.Count > 0;

        [XmlArray("DataTypeSystemList")]
        [XmlArrayItem("DataTypeSystem")]
        public List<DataTypeSystem> DataTypeSystemList { get; set; } = new();
        public bool ShouldSerializeDataTypeSystemList() => DataTypeSystemList.Count > 0;

        public static MetaDataTypeModel LoadFromXmlWorkspace(
            string workspacePath,
            bool searchUpward = true)
        {
            var model = TypedWorkspaceXmlSerializer.Load<MetaDataTypeModel>(workspacePath, searchUpward);
            MetaDataTypeModelFactory.Bind(model);
            return model;
        }

        public static Task<MetaDataTypeModel> LoadFromXmlWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(LoadFromXmlWorkspace(workspacePath, searchUpward));
        }

        public void SaveToXmlWorkspace(string workspacePath)
        {
            MetaDataTypeModelFactory.Bind(this);
            TypedWorkspaceXmlSerializer.Save(this, workspacePath, ResolveBundledModelXmlPath());
        }

        public Task SaveToXmlWorkspaceAsync(
            string workspacePath,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SaveToXmlWorkspace(workspacePath);
            return Task.CompletedTask;
        }

        private static string? ResolveBundledModelXmlPath()
        {
            var assemblyDirectory = Path.GetDirectoryName(typeof(MetaDataTypeModel).Assembly.Location);
            if (string.IsNullOrWhiteSpace(assemblyDirectory))
            {
                return null;
            }

            var directPath = Path.Combine(assemblyDirectory, "model.xml");
            if (File.Exists(directPath))
            {
                return directPath;
            }

            var namespacedPath = Path.Combine(assemblyDirectory, "MetaDataType", "model.xml");
            return File.Exists(namespacedPath) ? namespacedPath : null;
        }
    }

    internal static class MetaDataTypeModelFactory
    {
        internal static void Bind(MetaDataTypeModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            model.DataTypeList ??= new List<DataType>();
            model.DataTypeSystemList ??= new List<DataTypeSystem>();

            NormalizeDataTypeList(model);
            NormalizeDataTypeSystemList(model);

            var dataTypeListById = BuildById(model.DataTypeList, row => row.Id, "DataType");
            var dataTypeSystemListById = BuildById(model.DataTypeSystemList, row => row.Id, "DataTypeSystem");

            foreach (var row in model.DataTypeList)
            {
                row.DataTypeSystemId = ResolveRelationshipId(
                    row.DataTypeSystemId,
                    row.DataTypeSystem?.Id,
                    "DataType",
                    row.Id,
                    "DataTypeSystemId");
                row.DataTypeSystem = RequireTarget(
                    dataTypeSystemListById,
                    row.DataTypeSystemId,
                    "DataType",
                    row.Id,
                    "DataTypeSystemId");
            }

        }

        private static void NormalizeDataTypeList(MetaDataTypeModel model)
        {
            foreach (var row in model.DataTypeList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DataType' contains a row with empty Id.");
                row.Category ??= string.Empty;
                row.Description ??= string.Empty;
                row.IsCanonical ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'DataType' row '{row.Id}' is missing required property 'Name'.");
                row.DataTypeSystemId ??= string.Empty;
            }
        }

        private static void NormalizeDataTypeSystemList(MetaDataTypeModel model)
        {
            foreach (var row in model.DataTypeSystemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DataTypeSystem' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'DataTypeSystem' row '{row.Id}' is missing required property 'Name'.");
            }
        }

        private static Dictionary<string, T> BuildById<T>(
            IEnumerable<T> rows,
            Func<T, string> getId,
            string entityName)
            where T : class
        {
            var rowsById = new Dictionary<string, T>(StringComparer.Ordinal);
            foreach (var row in rows)
            {
                ArgumentNullException.ThrowIfNull(row);
                var id = RequireIdentity(getId(row), $"Entity '{entityName}' contains a row with empty Id.");
                if (!rowsById.TryAdd(id, row))
                {
                    throw new InvalidOperationException($"Entity '{entityName}' contains duplicate Id '{id}'.");
                }
            }

            return rowsById;
        }

        private static T RequireTarget<T>(
            Dictionary<string, T> rowsById,
            string targetId,
            string sourceEntityName,
            string sourceId,
            string relationshipName)
            where T : class
        {
            var normalizedTargetId = RequireIdentity(targetId, $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' is empty.");
            if (!rowsById.TryGetValue(normalizedTargetId, out var target))
            {
                throw new InvalidOperationException($"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' points to missing Id '{normalizedTargetId}'.");
            }

            return target;
        }

        private static string ResolveRelationshipId(
            string relationshipId,
            string? navigationId,
            string sourceEntityName,
            string sourceId,
            string relationshipName)
        {
            var normalizedRelationshipId = NormalizeIdentity(relationshipId);
            var normalizedNavigationId = NormalizeIdentity(navigationId);
            if (!string.IsNullOrEmpty(normalizedRelationshipId) &&
                !string.IsNullOrEmpty(normalizedNavigationId) &&
                !string.Equals(normalizedRelationshipId, normalizedNavigationId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' conflicts between '{normalizedRelationshipId}' and '{normalizedNavigationId}'.");
            }

            var resolvedTargetId = string.IsNullOrEmpty(normalizedRelationshipId)
                ? normalizedNavigationId
                : normalizedRelationshipId;
            return RequireIdentity(resolvedTargetId, $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' is empty.");
        }

        private static string RequireIdentity(string? value, string errorMessage)
        {
            var normalizedValue = NormalizeIdentity(value);
            if (string.IsNullOrEmpty(normalizedValue))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return normalizedValue;
        }

        private static string RequireText(string? value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return value;
        }

        private static string NormalizeIdentity(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }
    }
}
