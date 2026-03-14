using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Meta.Core.Domain;

namespace MetaSqlDeploy
{
    public sealed partial class MetaSqlDeployModel
    {
        internal MetaSqlDeployModel(
            IReadOnlyList<DeployConfiguration> deployConfigurationList,
            IReadOnlyList<DeployTarget> deployTargetList
        )
        {
            DeployConfigurationList = deployConfigurationList;
            DeployTargetList = deployTargetList;
        }

        public IReadOnlyList<DeployConfiguration> DeployConfigurationList { get; }
        public IReadOnlyList<DeployTarget> DeployTargetList { get; }
    }

    internal static class MetaSqlDeployModelFactory
    {
        internal static MetaSqlDeployModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var deployConfigurationList = new List<DeployConfiguration>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("DeployConfiguration", out var deployConfigurationListRecords))
            {
                foreach (var record in deployConfigurationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    deployConfigurationList.Add(new DeployConfiguration
                    {
                        Id = record.Id ?? string.Empty,
                        MigrationRoot = record.Values.TryGetValue("MigrationRoot", out var migrationRootValue) ? migrationRootValue ?? string.Empty : string.Empty,
                        RootMode = record.Values.TryGetValue("RootMode", out var rootModeValue) ? rootModeValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var deployTargetList = new List<DeployTarget>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("DeployTarget", out var deployTargetListRecords))
            {
                foreach (var record in deployTargetListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    deployTargetList.Add(new DeployTarget
                    {
                        Id = record.Id ?? string.Empty,
                        ConnectionString = record.Values.TryGetValue("ConnectionString", out var connectionStringValue) ? connectionStringValue ?? string.Empty : string.Empty,
                        ConnectionStringEnvVar = record.Values.TryGetValue("ConnectionStringEnvVar", out var connectionStringEnvVarValue) ? connectionStringEnvVarValue ?? string.Empty : string.Empty,
                        DesiredSql = record.Values.TryGetValue("DesiredSql", out var desiredSqlValue) ? desiredSqlValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TraitsFile = record.Values.TryGetValue("TraitsFile", out var traitsFileValue) ? traitsFileValue ?? string.Empty : string.Empty,
                        DeployConfigurationId = record.RelationshipIds.TryGetValue("DeployConfigurationId", out var deployConfigurationRelationshipId) ? deployConfigurationRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var deployConfigurationListById = new Dictionary<string, DeployConfiguration>(global::System.StringComparer.Ordinal);
            foreach (var row in deployConfigurationList)
            {
                deployConfigurationListById[row.Id] = row;
            }

            var deployTargetListById = new Dictionary<string, DeployTarget>(global::System.StringComparer.Ordinal);
            foreach (var row in deployTargetList)
            {
                deployTargetListById[row.Id] = row;
            }

            foreach (var row in deployTargetList)
            {
                row.DeployConfiguration = RequireTarget(
                    deployConfigurationListById,
                    row.DeployConfigurationId,
                    "DeployTarget",
                    row.Id,
                    "DeployConfigurationId");
            }

            return new MetaSqlDeployModel(
                new ReadOnlyCollection<DeployConfiguration>(deployConfigurationList),
                new ReadOnlyCollection<DeployTarget>(deployTargetList)
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
