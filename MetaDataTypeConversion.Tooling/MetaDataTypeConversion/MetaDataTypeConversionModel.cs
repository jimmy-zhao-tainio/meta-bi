using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Meta.Core.Serialization;

namespace MetaDataTypeConversion
{
    [XmlRoot("MetaDataTypeConversion")]
    public sealed partial class MetaDataTypeConversionModel
    {
        public static MetaDataTypeConversionModel CreateEmpty() => new();

        [XmlArray("ConversionImplementationList")]
        [XmlArrayItem("ConversionImplementation")]
        public List<ConversionImplementation> ConversionImplementationList { get; set; } = new();
        public bool ShouldSerializeConversionImplementationList() => ConversionImplementationList.Count > 0;

        [XmlArray("DataTypeMappingList")]
        [XmlArrayItem("DataTypeMapping")]
        public List<DataTypeMapping> DataTypeMappingList { get; set; } = new();
        public bool ShouldSerializeDataTypeMappingList() => DataTypeMappingList.Count > 0;

        public static MetaDataTypeConversionModel LoadFromXmlWorkspace(
            string workspacePath,
            bool searchUpward = true)
        {
            var model = TypedWorkspaceXmlSerializer.Load<MetaDataTypeConversionModel>(workspacePath, searchUpward);
            MetaDataTypeConversionModelFactory.Bind(model);
            return model;
        }

        public static Task<MetaDataTypeConversionModel> LoadFromXmlWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(LoadFromXmlWorkspace(workspacePath, searchUpward));
        }

        public void SaveToXmlWorkspace(string workspacePath)
        {
            MetaDataTypeConversionModelFactory.Bind(this);
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
            var assemblyDirectory = Path.GetDirectoryName(typeof(MetaDataTypeConversionModel).Assembly.Location);
            if (string.IsNullOrWhiteSpace(assemblyDirectory))
            {
                return null;
            }

            var directPath = Path.Combine(assemblyDirectory, "model.xml");
            if (File.Exists(directPath))
            {
                return directPath;
            }

            var namespacedPath = Path.Combine(assemblyDirectory, "MetaDataTypeConversion", "model.xml");
            return File.Exists(namespacedPath) ? namespacedPath : null;
        }
    }

    internal static class MetaDataTypeConversionModelFactory
    {
        internal static void Bind(MetaDataTypeConversionModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            model.ConversionImplementationList ??= new List<ConversionImplementation>();
            model.DataTypeMappingList ??= new List<DataTypeMapping>();

            NormalizeConversionImplementationList(model);
            NormalizeDataTypeMappingList(model);

            var conversionImplementationListById = BuildById(model.ConversionImplementationList, row => row.Id, "ConversionImplementation");
            var dataTypeMappingListById = BuildById(model.DataTypeMappingList, row => row.Id, "DataTypeMapping");

            foreach (var row in model.DataTypeMappingList)
            {
                row.ConversionImplementationId = ResolveRelationshipId(
                    row.ConversionImplementationId,
                    row.ConversionImplementation?.Id,
                    "DataTypeMapping",
                    row.Id,
                    "ConversionImplementationId");
                row.ConversionImplementation = RequireTarget(
                    conversionImplementationListById,
                    row.ConversionImplementationId,
                    "DataTypeMapping",
                    row.Id,
                    "ConversionImplementationId");
            }

        }

        private static void NormalizeConversionImplementationList(MetaDataTypeConversionModel model)
        {
            foreach (var row in model.ConversionImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ConversionImplementation' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'ConversionImplementation' row '{row.Id}' is missing required property 'Name'.");
            }
        }

        private static void NormalizeDataTypeMappingList(MetaDataTypeConversionModel model)
        {
            foreach (var row in model.DataTypeMappingList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DataTypeMapping' contains a row with empty Id.");
                row.Notes ??= string.Empty;
                row.SourceDataTypeId = RequireText(row.SourceDataTypeId, $"Entity 'DataTypeMapping' row '{row.Id}' is missing required property 'SourceDataTypeId'.");
                row.TargetDataTypeId = RequireText(row.TargetDataTypeId, $"Entity 'DataTypeMapping' row '{row.Id}' is missing required property 'TargetDataTypeId'.");
                row.ConversionImplementationId ??= string.Empty;
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
