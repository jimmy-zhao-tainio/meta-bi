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
            SelectElement = selectElement.GetRef<SelectElement>(nameof(SelectElement))
        };
        model.SelectScalarExpressionList.Add(row);
        model.SelectScalarExpressionExpressionLinkList.Add(new SelectScalarExpressionExpressionLink
        {
            Id = NextId(nameof(SelectScalarExpressionExpressionLink)),
            SelectScalarExpression = row,
            ScalarExpression = expression.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });

        if (columnName is not null)
        {
            model.SelectScalarExpressionColumnNameLinkList.Add(new SelectScalarExpressionColumnNameLink
            {
                Id = NextId(nameof(SelectScalarExpressionColumnNameLink)),
                SelectScalarExpression = row,
                IdentifierOrValueExpression = columnName.GetRef<IdentifierOrValueExpression>(nameof(IdentifierOrValueExpression))
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
            SelectElement = selectElement.GetRef<SelectElement>(nameof(SelectElement))
        };
        model.SelectStarExpressionList.Add(row);

        if (qualifier is not null)
        {
            model.SelectStarExpressionQualifierLinkList.Add(new SelectStarExpressionQualifierLink
            {
                Id = NextId(nameof(SelectStarExpressionQualifierLink)),
                SelectStarExpression = row,
                MultiPartIdentifier = qualifier.GetRef<MultiPartIdentifier>(nameof(MultiPartIdentifier))
            });
        }

        return BuiltNode.Create(
            (nameof(SelectElement), selectElement.GetId(nameof(SelectElement))),
            (nameof(SelectStarExpression), row.Id));
    }
}
