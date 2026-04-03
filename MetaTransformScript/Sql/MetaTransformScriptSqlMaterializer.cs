using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using MTS = global::MetaTransformScript;
using MSDOM = global::Microsoft.SqlServer.TransactSql.ScriptDom;

namespace MetaTransformScript.Sql;

internal sealed class MetaTransformScriptSqlMaterializer
{
    private static readonly HashSet<string> TriviaPropertyNames = new(StringComparer.Ordinal)
    {
        "FirstTokenIndex",
        "FragmentLength",
        "LastTokenIndex",
        "ScriptTokenStream",
        "StartColumn",
        "StartLine",
        "StartOffset"
    };

    private readonly MTS.MetaTransformScriptModel model = MTS.MetaTransformScriptModel.CreateEmpty();
    private readonly Dictionary<MSDOM.TSqlFragment, MaterializedNode> materializedNodes = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<string, EntityRegistration> entityRegistrations;
    private readonly Dictionary<string, int> nextOrdinalByEntityName = new(StringComparer.Ordinal);

    public MetaTransformScriptSqlMaterializer()
    {
        entityRegistrations = BuildEntityRegistrations();
    }

    public MTS.MetaTransformScriptModel Materialize(IEnumerable<MetaTransformScriptSqlDocument> scripts)
    {
        ArgumentNullException.ThrowIfNull(scripts);

        var any = false;
        foreach (var script in scripts)
        {
            AddScript(script);
            any = true;
        }

        if (!any)
        {
            throw new InvalidOperationException("No supported SQL transform scripts were found to materialize.");
        }

        return model;
    }

    public void AddScript(MetaTransformScriptSqlDocument script)
    {
        ArgumentNullException.ThrowIfNull(script);

        if (string.IsNullOrWhiteSpace(script.Name))
        {
            throw new InvalidOperationException("Transform script name cannot be empty.");
        }

        if (model.TransformScriptList.Any(existing => string.Equals(existing.Name, script.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Transform script '{script.Name}' already exists in this workspace.");
        }

        var rootNode = EnsureMaterialized(script.SelectStatement);
        var selectStatementId = rootNode.IdByEntityName[nameof(MTS.SelectStatement)];

        var transformScript = new MTS.TransformScript
        {
            Id = NextId(nameof(MTS.TransformScript)),
            Name = script.Name,
            SourcePath = script.SourcePath ?? string.Empty
        };
        model.TransformScriptList.Add(transformScript);

        if (script.SchemaIdentifier is not null)
        {
            var schemaIdentifierNode = EnsureMaterialized(script.SchemaIdentifier);
            model.TransformScriptSchemaIdentifierLinkList.Add(new MTS.TransformScriptSchemaIdentifierLink
            {
                Id = NextId(nameof(MTS.TransformScriptSchemaIdentifierLink)),
                OwnerId = transformScript.Id,
                ValueId = schemaIdentifierNode.IdByEntityName[nameof(MTS.Identifier)]
            });
        }

        if (script.ObjectIdentifier is not null)
        {
            var objectIdentifierNode = EnsureMaterialized(script.ObjectIdentifier);
            model.TransformScriptObjectIdentifierLinkList.Add(new MTS.TransformScriptObjectIdentifierLink
            {
                Id = NextId(nameof(MTS.TransformScriptObjectIdentifierLink)),
                OwnerId = transformScript.Id,
                ValueId = objectIdentifierNode.IdByEntityName[nameof(MTS.Identifier)]
            });
        }

        for (var ordinal = 0; ordinal < script.ViewColumns.Count; ordinal++)
        {
            var columnIdentifierNode = EnsureMaterialized(script.ViewColumns[ordinal]);
            model.TransformScriptViewColumnsItemList.Add(new MTS.TransformScriptViewColumnsItem
            {
                Id = NextId(nameof(MTS.TransformScriptViewColumnsItem)),
                OwnerId = transformScript.Id,
                ValueId = columnIdentifierNode.IdByEntityName[nameof(MTS.Identifier)],
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        model.TransformScriptSelectStatementLinkList.Add(new MTS.TransformScriptSelectStatementLink
        {
            Id = NextId(nameof(MTS.TransformScriptSelectStatementLink)),
            OwnerId = transformScript.Id,
            ValueId = selectStatementId
        });
    }

    private static Dictionary<string, EntityRegistration> BuildEntityRegistrations()
    {
        var registrations = new Dictionary<string, EntityRegistration>(StringComparer.Ordinal);
        var modelType = typeof(MTS.MetaTransformScriptModel);

        foreach (var listProperty in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(static property => property.CanRead && property.PropertyType.IsGenericType)
                     .Where(static property =>
                     {
                         var genericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                         return genericTypeDefinition == typeof(List<>)
                             || genericTypeDefinition == typeof(IReadOnlyList<>);
                     }))
        {
            var entityType = listProperty.PropertyType.GetGenericArguments()[0];
            registrations[entityType.Name] = new EntityRegistration(entityType, listProperty);
        }

        return registrations;
    }

    private MaterializedNode EnsureMaterialized(MSDOM.TSqlFragment fragment)
    {
        if (materializedNodes.TryGetValue(fragment, out var existing))
        {
            return existing;
        }

        var chain = GetFragmentTypeChain(fragment.GetType());
        var idsByEntityName = new Dictionary<string, string>(StringComparer.Ordinal);
        var instancesByEntityName = new Dictionary<string, object>(StringComparer.Ordinal);

        string? previousBaseId = null;
        foreach (var type in chain)
        {
            var entityName = type.Name;
            if (!entityRegistrations.TryGetValue(entityName, out var registration))
            {
                throw new InvalidOperationException(
                    $"No generated MetaTransformScript entity registration was found for ScriptDOM type '{type.FullName}'.");
            }

            var instance = Activator.CreateInstance(registration.EntityType)
                ?? throw new InvalidOperationException($"Could not create generated entity '{registration.EntityType.FullName}'.");
            var id = NextId(entityName);

            SetStringProperty(instance, "Id", id);
            if (!string.IsNullOrWhiteSpace(previousBaseId) && HasWritableProperty(instance, "BaseId"))
            {
                SetStringProperty(instance, "BaseId", previousBaseId);
            }

            AddEntityInstance(registration, instance);
            idsByEntityName[entityName] = id;
            instancesByEntityName[entityName] = instance;
            previousBaseId = id;
        }

        var materialized = new MaterializedNode(idsByEntityName, instancesByEntityName);
        materializedNodes[fragment] = materialized;

        foreach (var type in chain)
        {
            PopulateDeclaredProperties(fragment, type, materialized);
        }

        return materialized;
    }

    private static IReadOnlyList<Type> GetFragmentTypeChain(Type runtimeType)
    {
        var chain = new Stack<Type>();
        var current = runtimeType;
        while (typeof(MSDOM.TSqlFragment).IsAssignableFrom(current) && current != typeof(MSDOM.TSqlFragment))
        {
            chain.Push(current);

            if (current.BaseType is null || !typeof(MSDOM.TSqlFragment).IsAssignableFrom(current.BaseType))
            {
                break;
            }

            current = current.BaseType;
        }

        return chain.ToArray();
    }

    private void PopulateDeclaredProperties(MSDOM.TSqlFragment fragment, Type declaredType, MaterializedNode materialized)
    {
        var entityName = declaredType.Name;
        var entity = materialized.InstanceByEntityName[entityName];

        foreach (var property in GetDeclaredReadableInstanceProperties(declaredType))
        {
            if (IsTriviaProperty(property))
            {
                continue;
            }

            var value = property.GetValue(fragment);
            if (value is null)
            {
                continue;
            }

            if (value is MSDOM.TSqlFragment childFragment)
            {
                var linkEntityName = entityName + property.Name + "Link";
                if (!entityRegistrations.TryGetValue(linkEntityName, out var linkRegistration))
                {
                    throw new InvalidOperationException(
                        $"Generated link entity '{linkEntityName}' was not found for '{declaredType.FullName}.{property.Name}'.");
                }

                var childNode = EnsureMaterialized(childFragment);
                var declaredTargetEntityName = property.PropertyType.Name;
                if (!childNode.IdByEntityName.TryGetValue(declaredTargetEntityName, out var childId))
                {
                    throw new InvalidOperationException(
                        $"Materialized child '{childFragment.GetType().FullName}' does not expose base id '{declaredTargetEntityName}' required by '{declaredType.FullName}.{property.Name}'.");
                }

                var linkEntity = Activator.CreateInstance(linkRegistration.EntityType)
                    ?? throw new InvalidOperationException($"Could not create generated entity '{linkRegistration.EntityType.FullName}'.");
                SetStringProperty(linkEntity, "Id", NextId(linkEntityName));
                SetStringProperty(linkEntity, "OwnerId", materialized.IdByEntityName[entityName]);
                SetStringProperty(linkEntity, "ValueId", childId);
                AddEntityInstance(linkRegistration, linkEntity);
                continue;
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                var elementType = GetCollectionElementType(property.PropertyType);
                if (elementType is not null && typeof(MSDOM.TSqlFragment).IsAssignableFrom(elementType))
                {
                    var fragmentItems = enumerable.OfType<MSDOM.TSqlFragment>().ToArray();
                    if (fragmentItems.Length == 0)
                    {
                        continue;
                    }

                    var itemEntityName = entityName + property.Name + "Item";
                    if (!entityRegistrations.TryGetValue(itemEntityName, out var itemRegistration))
                    {
                        throw new InvalidOperationException(
                            $"Generated item entity '{itemEntityName}' was not found for '{declaredType.FullName}.{property.Name}'.");
                    }

                    var ordinal = 0;
                    foreach (var childItem in fragmentItems)
                    {
                        var childNode = EnsureMaterialized(childItem);
                        var declaredElementEntityName = elementType.Name;
                        if (!childNode.IdByEntityName.TryGetValue(declaredElementEntityName, out var valueId))
                        {
                            throw new InvalidOperationException(
                                $"Materialized collection child '{childItem.GetType().FullName}' does not expose base id '{declaredElementEntityName}' required by '{declaredType.FullName}.{property.Name}'.");
                        }

                        var itemEntity = Activator.CreateInstance(itemRegistration.EntityType)
                            ?? throw new InvalidOperationException($"Could not create generated entity '{itemRegistration.EntityType.FullName}'.");
                        SetStringProperty(itemEntity, "Id", NextId(itemEntityName));
                        SetStringProperty(itemEntity, "OwnerId", materialized.IdByEntityName[entityName]);
                        SetStringProperty(itemEntity, "ValueId", valueId);
                        SetStringProperty(itemEntity, "Ordinal", ordinal.ToString(CultureInfo.InvariantCulture));
                        AddEntityInstance(itemRegistration, itemEntity);
                        ordinal++;
                    }

                    continue;
                }
            }

            if (HasWritableProperty(entity, property.Name))
            {
                SetStringProperty(entity, property.Name, ConvertScalarValue(value));
            }
        }
    }

    private void AddEntityInstance(EntityRegistration registration, object instance)
    {
        var list = (IList?)registration.ListProperty.GetValue(model)
            ?? throw new InvalidOperationException($"Generated model list '{registration.ListProperty.Name}' was null.");
        list.Add(instance);
    }

    private string NextId(string entityName)
    {
        nextOrdinalByEntityName.TryGetValue(entityName, out var current);
        current++;
        nextOrdinalByEntityName[entityName] = current;
        return $"{entityName}:{current}";
    }

    private static void SetStringProperty(object target, string propertyName, string value)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException($"Property '{target.GetType().FullName}.{propertyName}' was not found.");
        property.SetValue(target, value);
    }

    private static bool HasWritableProperty(object target, string propertyName)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        return property is not null && property.CanWrite;
    }

    private static IEnumerable<PropertyInfo> GetDeclaredReadableInstanceProperties(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static property => property.CanRead && property.GetIndexParameters().Length == 0)
            .OrderBy(static property => property.Name, StringComparer.Ordinal);

    private static bool IsTriviaProperty(PropertyInfo property) =>
        TriviaPropertyNames.Contains(property.Name);

    private static Type? GetCollectionElementType(Type propertyType)
    {
        if (propertyType.IsArray)
        {
            return propertyType.GetElementType();
        }

        if (propertyType.IsGenericType)
        {
            return propertyType.GetGenericArguments().FirstOrDefault();
        }

        var enumerableInterface = propertyType
            .GetInterfaces()
            .FirstOrDefault(static iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        return enumerableInterface?.GetGenericArguments().FirstOrDefault();
    }

    private static string ConvertScalarValue(object value) =>
        value switch
        {
            bool boolValue => boolValue ? "true" : "false",
            Enum enumValue => enumValue.ToString(),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };

    private sealed record EntityRegistration(Type EntityType, PropertyInfo ListProperty);

    private sealed class MaterializedNode(
        Dictionary<string, string> idByEntityName,
        Dictionary<string, object> instanceByEntityName)
    {
        public IReadOnlyDictionary<string, string> IdByEntityName { get; } = idByEntityName;
        public IReadOnlyDictionary<string, object> InstanceByEntityName { get; } = instanceByEntityName;
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>, IEqualityComparer<MSDOM.TSqlFragment>
    {
        public static ReferenceEqualityComparer Instance { get; } = new();

        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);
        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        public bool Equals(MSDOM.TSqlFragment? x, MSDOM.TSqlFragment? y) => ReferenceEquals(x, y);
        public int GetHashCode(MSDOM.TSqlFragment obj) => RuntimeHelpers.GetHashCode(obj);
    }
}

internal sealed record MetaTransformScriptSqlDocument(
    string Name,
    string? SourcePath,
    MSDOM.SelectStatement SelectStatement,
    MSDOM.Identifier? SchemaIdentifier,
    MSDOM.Identifier? ObjectIdentifier,
    IReadOnlyList<MSDOM.Identifier> ViewColumns);
