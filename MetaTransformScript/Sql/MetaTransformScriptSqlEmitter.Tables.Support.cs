using System.Text;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderSelectElement(SelectElement selectElement)
    {
        var selectStarExpression = FindByBaseId(model.SelectStarExpressionList, selectElement.Id);
        if (selectStarExpression is not null)
        {
            var qualifierLink = FindOwnerLink(model.SelectStarExpressionQualifierLinkList, selectStarExpression.Id);
            return qualifierLink is null
                ? "*"
                : RenderMultiPartIdentifier(qualifierLink.Value) + ".*";
        }

        var selectScalarExpression = FindByBaseId(model.SelectScalarExpressionList, selectElement.Id)
            ?? throw new InvalidOperationException($"Unsupported MetaTransformScript SelectElement id '{selectElement.Id}'.");

        var expression = RenderScalarExpression(GetOwnerLink(
            model.SelectScalarExpressionExpressionLinkList,
            selectScalarExpression.Id,
            "SelectScalarExpression.Expression").Value);

        var columnNameLink = FindOwnerLink(model.SelectScalarExpressionColumnNameLinkList, selectScalarExpression.Id);
        if (columnNameLink is not null)
        {
            return $"{expression} AS {RenderIdentifierOrValueExpression(columnNameLink.Value)}";
        }

        return expression;
    }

    private string RenderRowValue(RowValue rowValue)
    {
        var values = GetOrderedItems(model.RowValueColumnValuesItemList, rowValue.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        return "(" + string.Join(", ", values) + ")";
    }

    private string RenderTableSampleClause(TableSampleClause tableSampleClause)
    {
        var sampleNumber = RenderScalarExpression(GetOwnerLink(
            model.TableSampleClauseSampleNumberLinkList,
            tableSampleClause.Id,
            "TableSampleClause.SampleNumber").Value);
        var option = tableSampleClause.TableSampleClauseOption switch
        {
            "Percent" => "PERCENT",
            "Rows" => "ROWS",
            _ => throw new InvalidOperationException(
                $"Unsupported MetaTransformScript TableSampleClause.TableSampleClauseOption '{tableSampleClause.TableSampleClauseOption}'.")
        };

        var builder = new StringBuilder();
        builder.Append("TABLESAMPLE ");
        if (IsTrue(GetString(tableSampleClause, "System")))
        {
            builder.Append("SYSTEM ");
        }

        builder.Append('(');
        builder.Append(sampleNumber);
        builder.Append(' ');
        builder.Append(option);
        builder.Append(')');

        var repeatSeedLink = FindOwnerLink(model.TableSampleClauseRepeatSeedLinkList, tableSampleClause.Id);
        if (repeatSeedLink is not null)
        {
            builder.Append(" REPEATABLE (");
            builder.Append(RenderScalarExpression(repeatSeedLink.Value));
            builder.Append(')');
        }

        return builder.ToString();
    }

    private string RenderSchemaObjectName(SchemaObjectName schemaObjectName)
    {
        var parts = new List<string>();
        var schemaLink = FindOwnerLink(model.SchemaObjectNameSchemaIdentifierLinkList, schemaObjectName.Id);
        if (schemaLink is not null)
        {
            parts.Add(RenderIdentifier(schemaLink.Value));
        }

        parts.Add(RenderIdentifier(GetOwnerLink(model.SchemaObjectNameBaseIdentifierLinkList, schemaObjectName.Id, "SchemaObjectName.BaseIdentifier").Value));
        return string.Join(".", parts);
    }

    private string RenderIdentifierOrValueExpression(IdentifierOrValueExpression value)
    {
        var identifierLink = FindOwnerLink(model.IdentifierOrValueExpressionIdentifierLinkList, value.Id);
        if (identifierLink is not null)
        {
            return RenderIdentifier(identifierLink.Value);
        }

        if (!string.IsNullOrWhiteSpace(value.Value))
        {
            return value.Value;
        }

        throw new InvalidOperationException($"IdentifierOrValueExpression '{value.Id}' was empty.");
    }

    private string RenderMultiPartIdentifier(MultiPartIdentifier multiPartIdentifier)
    {
        var parts = GetOrderedItems(model.MultiPartIdentifierIdentifiersItemList, multiPartIdentifier.Id)
            .Select(row => RenderIdentifier(row.Value))
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
        var literal = GetOwnerLink(model.XmlNamespacesElementStringLinkList, element.Id, "XmlNamespacesElement.String").Value;
        var literalBase = GetById(model.LiteralList, literal.BaseId, "XmlNamespacesElement.StringLiteralBase");
        var renderedLiteral = RenderLiteral(literalBase);

        var defaultElement = FindByBaseId(model.XmlNamespacesDefaultElementList, element.Id);
        if (defaultElement is not null)
        {
            return "DEFAULT " + renderedLiteral;
        }

        var aliasElement = FindByBaseId(model.XmlNamespacesAliasElementList, element.Id);
        if (aliasElement is not null)
        {
            var alias = RenderIdentifier(GetOwnerLink(model.XmlNamespacesAliasElementIdentifierLinkList, aliasElement.Id, "XmlNamespacesAliasElement.Identifier").Value);
            return $"{renderedLiteral} AS {alias}";
        }

        return renderedLiteral;
    }

    private string RenderAliasAndColumns(TableReferenceWithAliasAndColumns aliasAndColumns)
    {
        var aliasBase = GetById(model.TableReferenceWithAliasList, aliasAndColumns.BaseId, "TableReferenceWithAliasAndColumns.Base");
        var aliasLink = FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, aliasBase.Id)
            ?? throw new InvalidOperationException(
                $"TableReferenceWithAliasAndColumns '{aliasAndColumns.Id}' was missing an alias.");

        var columns = GetOrderedItems(model.TableReferenceWithAliasAndColumnsColumnsItemList, aliasAndColumns.Id)
            .Select(row => RenderIdentifier(row.Value))
            .ToArray();

        return columns.Length == 0
            ? " AS " + RenderIdentifier(aliasLink.Value)
            : " AS " + RenderIdentifier(aliasLink.Value) + "(" + string.Join(", ", columns) + ")";
    }
}
