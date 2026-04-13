using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    private BuiltNode CreateSelectElementBase()
    {
        var row = new SelectElement
        {
            Id = NextId(nameof(SelectElement))
        };
        model.SelectElementList.Add(row);
        return BuiltNode.Create((nameof(SelectElement), row.Id));
    }

    public BuiltNode CreateSelectScalarExpression(BuiltNode expression, BuiltNode? columnName = null)
    {
        var selectElement = CreateSelectElementBase();

        var row = new SelectScalarExpression
        {
            Id = NextId(nameof(SelectScalarExpression)),
            SelectElementId = selectElement.GetId(nameof(SelectElement))
        };
        model.SelectScalarExpressionList.Add(row);
        model.SelectScalarExpressionExpressionLinkList.Add(new SelectScalarExpressionExpressionLink
        {
            Id = NextId(nameof(SelectScalarExpressionExpressionLink)),
            SelectScalarExpressionId = row.Id,
            ScalarExpressionId = expression.GetId(nameof(ScalarExpression))
        });

        if (columnName is not null)
        {
            model.SelectScalarExpressionColumnNameLinkList.Add(new SelectScalarExpressionColumnNameLink
            {
                Id = NextId(nameof(SelectScalarExpressionColumnNameLink)),
                SelectScalarExpressionId = row.Id,
                IdentifierOrValueExpressionId = columnName.GetId(nameof(IdentifierOrValueExpression))
            });
        }

        return BuiltNode.Create(
            (nameof(SelectElement), selectElement.GetId(nameof(SelectElement))),
            (nameof(SelectScalarExpression), row.Id));
    }

    public BuiltNode CreateSelectStarExpression(BuiltNode? qualifier = null)
    {
        var selectElement = CreateSelectElementBase();

        var row = new SelectStarExpression
        {
            Id = NextId(nameof(SelectStarExpression)),
            SelectElementId = selectElement.GetId(nameof(SelectElement))
        };
        model.SelectStarExpressionList.Add(row);

        if (qualifier is not null)
        {
            model.SelectStarExpressionQualifierLinkList.Add(new SelectStarExpressionQualifierLink
            {
                Id = NextId(nameof(SelectStarExpressionQualifierLink)),
                SelectStarExpressionId = row.Id,
                MultiPartIdentifierId = qualifier.GetId(nameof(MultiPartIdentifier))
            });
        }

        return BuiltNode.Create(
            (nameof(SelectElement), selectElement.GetId(nameof(SelectElement))),
            (nameof(SelectStarExpression), row.Id));
    }
}
