using System.Text;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderSelectElement(SelectElement selectElement)
    {
        var selectStarExpression = FindByBaseId(model.SelectStarExpressionList, static row => row.SelectElement?.Id, nameof(SelectStarExpression.SelectElement), selectElement.Id);
        if (selectStarExpression is not null)
        {
            var qualifierLink = FindOwnerLink(model.SelectStarExpressionQualifierLinkList, static row => row.SelectStarExpression?.Id, nameof(SelectStarExpressionQualifierLink.SelectStarExpression), selectStarExpression.Id);
            return qualifierLink is null
                ? "*"
                : RenderMultiPartIdentifier(qualifierLink.MultiPartIdentifier) + ".*";
        }

        var selectScalarExpression = FindByBaseId(model.SelectScalarExpressionList, static row => row.SelectElement?.Id, nameof(SelectScalarExpression.SelectElement), selectElement.Id)
            ?? throw new InvalidOperationException($"Unsupported MetaTransformScript SelectElement id '{selectElement.Id}'.");

        var expression = RenderScalarExpression(GetOwnerLink(
            model.SelectScalarExpressionExpressionLinkList, static row => row.SelectScalarExpression?.Id, nameof(SelectScalarExpressionExpressionLink.SelectScalarExpression),
            selectScalarExpression.Id,
            "SelectScalarExpression.Expression").ScalarExpression);

        var columnNameLink = FindOwnerLink(model.SelectScalarExpressionColumnNameLinkList, static row => row.SelectScalarExpression?.Id, nameof(SelectScalarExpressionColumnNameLink.SelectScalarExpression), selectScalarExpression.Id);
        if (columnNameLink is not null)
        {
            return $"{expression} AS {RenderIdentifierOrValueExpression(columnNameLink.IdentifierOrValueExpression)}";
        }

        return expression;
    }

    private string RenderRowValue(RowValue rowValue)
    {
        var values = GetOrderedItems(model.RowValueColumnValuesItemList, static row => row.RowValue?.Id, nameof(RowValueColumnValuesItem.RowValue), static row => row.Ordinal, rowValue.Id)
            .Select(row => RenderScalarExpression(row.ScalarExpression))
            .ToArray();
        return "(" + string.Join(", ", values) + ")";
    }

    private string RenderTableSampleClause(TableSampleClause tableSampleClause)
    {
        var sampleNumber = RenderScalarExpression(GetOwnerLink(
            model.TableSampleClauseSampleNumberLinkList, static row => row.TableSampleClause?.Id, nameof(TableSampleClauseSampleNumberLink.TableSampleClause),
            tableSampleClause.Id,
            "TableSampleClause.SampleNumber").ScalarExpression);
        var option = tableSampleClause.TableSampleClauseOption switch
        {
            "Percent" => "PERCENT",
            "Rows" => "ROWS",
            _ => throw new InvalidOperationException(
                $"Unsupported MetaTransformScript TableSampleClause.TableSampleClauseOption '{tableSampleClause.TableSampleClauseOption}'.")
        };

        var builder = new StringBuilder();
        builder.Append("TABLESAMPLE ");
        if (IsTrue(tableSampleClause.System))
        {
            builder.Append("SYSTEM ");
        }

        builder.Append('(');
        builder.Append(sampleNumber);
        builder.Append(' ');
        builder.Append(option);
        builder.Append(')');

        var repeatSeedLink = FindOwnerLink(model.TableSampleClauseRepeatSeedLinkList, static row => row.TableSampleClause?.Id, nameof(TableSampleClauseRepeatSeedLink.TableSampleClause), tableSampleClause.Id);
        if (repeatSeedLink is not null)
        {
            builder.Append(" REPEATABLE (");
            builder.Append(RenderScalarExpression(repeatSeedLink.ScalarExpression));
            builder.Append(')');
        }

        return builder.ToString();
    }

    private string RenderSchemaObjectName(SchemaObjectName schemaObjectName)
    {
        var multiPartIdentifier = GetById(model.MultiPartIdentifierList, static row => row.Id, schemaObjectName.MultiPartIdentifier.Id, "SchemaObjectName.Base");
        return RenderMultiPartIdentifier(multiPartIdentifier);
    }

    private string RenderIdentifierOrValueExpression(IdentifierOrValueExpression value)
    {
        var identifierLink = FindOwnerLink(model.IdentifierOrValueExpressionIdentifierLinkList, static row => row.IdentifierOrValueExpression?.Id, nameof(IdentifierOrValueExpressionIdentifierLink.IdentifierOrValueExpression), value.Id);
        if (identifierLink is not null)
        {
            return RenderIdentifier(identifierLink.Identifier);
        }

        if (!string.IsNullOrWhiteSpace(value.Value))
        {
            return value.Value;
        }

        throw new InvalidOperationException($"IdentifierOrValueExpression '{value.Id}' was empty.");
    }

    private string RenderMultiPartIdentifier(MultiPartIdentifier multiPartIdentifier)
    {
        var parts = GetOrderedItems(model.MultiPartIdentifierIdentifiersItemList, static row => row.MultiPartIdentifier?.Id, nameof(MultiPartIdentifierIdentifiersItem.MultiPartIdentifier), static row => row.Ordinal, multiPartIdentifier.Id)
            .Select(row => RenderIdentifier(row.Identifier))
            .ToArray();
        return string.Join(".", parts);
    }

    private static string RenderFullTextColumns(IReadOnlyList<string> columns) =>
        columns.Count switch
        {
            0 => throw new InvalidOperationException("Full-text syntax requires at least one column."),
            1 => columns[0],
            _ => "(" + string.Join(", ", columns) + ")"
        };

    private string RenderXmlNamespacesElement(XmlNamespacesElement element)
    {
        var literal = GetOwnerLink(model.XmlNamespacesElementStringLinkList, static row => row.XmlNamespacesElement?.Id, nameof(XmlNamespacesElementStringLink.XmlNamespacesElement), element.Id, "XmlNamespacesElement.String").StringLiteral;
        var literalBase = GetById(model.LiteralList, static row => row.Id, literal.Literal.Id, "XmlNamespacesElement.StringLiteralBase");
        var renderedLiteral = RenderLiteral(literalBase);

        var defaultElement = FindByBaseId(model.XmlNamespacesDefaultElementList, static row => row.XmlNamespacesElement?.Id, nameof(XmlNamespacesDefaultElement.XmlNamespacesElement), element.Id);
        if (defaultElement is not null)
        {
            return "DEFAULT " + renderedLiteral;
        }

        var aliasElement = FindByBaseId(model.XmlNamespacesAliasElementList, static row => row.XmlNamespacesElement?.Id, nameof(XmlNamespacesAliasElement.XmlNamespacesElement), element.Id);
        if (aliasElement is not null)
        {
            var alias = RenderIdentifier(GetOwnerLink(model.XmlNamespacesAliasElementIdentifierLinkList, static row => row.XmlNamespacesAliasElement?.Id, nameof(XmlNamespacesAliasElementIdentifierLink.XmlNamespacesAliasElement), aliasElement.Id, "XmlNamespacesAliasElement.Identifier").Identifier);
            return $"{renderedLiteral} AS {alias}";
        }

        return renderedLiteral;
    }

    private string RenderAliasAndColumns(TableReferenceWithAliasAndColumns aliasAndColumns)
    {
        return RenderAliasAndColumns(aliasAndColumns, requireAlias: true);
    }

    private string RenderAliasAndColumns(TableReferenceWithAliasAndColumns aliasAndColumns, bool requireAlias)
    {
        var aliasBase = GetById(model.TableReferenceWithAliasList, static row => row.Id, aliasAndColumns.TableReferenceWithAlias.Id, "TableReferenceWithAliasAndColumns.Base");
        var aliasLink = FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, static row => row.TableReferenceWithAlias?.Id, nameof(TableReferenceWithAliasAliasLink.TableReferenceWithAlias), aliasBase.Id);
        if (aliasLink is null)
        {
            if (requireAlias)
            {
                throw new InvalidOperationException(
                    $"TableReferenceWithAliasAndColumns '{aliasAndColumns.Id}' was missing an alias.");
            }

            var orphanColumns = GetOrderedItems(model.TableReferenceWithAliasAndColumnsColumnsItemList, static row => row.TableReferenceWithAliasAndColumns?.Id, nameof(TableReferenceWithAliasAndColumnsColumnsItem.TableReferenceWithAliasAndColumns), static row => row.Ordinal, aliasAndColumns.Id);
            if (orphanColumns.Any())
            {
                throw new InvalidOperationException(
                    $"TableReferenceWithAliasAndColumns '{aliasAndColumns.Id}' contained column aliases without a table alias.");
            }

            return string.Empty;
        }

        var columns = GetOrderedItems(model.TableReferenceWithAliasAndColumnsColumnsItemList, static row => row.TableReferenceWithAliasAndColumns?.Id, nameof(TableReferenceWithAliasAndColumnsColumnsItem.TableReferenceWithAliasAndColumns), static row => row.Ordinal, aliasAndColumns.Id)
            .Select(row => RenderIdentifier(row.Identifier))
            .ToArray();

        return columns.Length == 0
            ? " AS " + RenderIdentifier(aliasLink.Identifier)
            : " AS " + RenderIdentifier(aliasLink.Identifier) + "(" + string.Join(", ", columns) + ")";
    }
}
