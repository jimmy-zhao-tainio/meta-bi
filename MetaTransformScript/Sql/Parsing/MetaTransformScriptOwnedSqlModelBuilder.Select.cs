using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptOwnedSqlModelBuilder
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
            BaseId = selectElement.GetId(nameof(SelectElement))
        };
        model.SelectScalarExpressionList.Add(row);
        model.SelectScalarExpressionExpressionLinkList.Add(new SelectScalarExpressionExpressionLink
        {
            Id = NextId(nameof(SelectScalarExpressionExpressionLink)),
            OwnerId = row.Id,
            ValueId = expression.GetId(nameof(ScalarExpression))
        });

        if (columnName is not null)
        {
            model.SelectScalarExpressionColumnNameLinkList.Add(new SelectScalarExpressionColumnNameLink
            {
                Id = NextId(nameof(SelectScalarExpressionColumnNameLink)),
                OwnerId = row.Id,
                ValueId = columnName.GetId(nameof(IdentifierOrValueExpression))
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
            BaseId = selectElement.GetId(nameof(SelectElement))
        };
        model.SelectStarExpressionList.Add(row);

        if (qualifier is not null)
        {
            model.SelectStarExpressionQualifierLinkList.Add(new SelectStarExpressionQualifierLink
            {
                Id = NextId(nameof(SelectStarExpressionQualifierLink)),
                OwnerId = row.Id,
                ValueId = qualifier.GetId(nameof(MultiPartIdentifier))
            });
        }

        return BuiltNode.Create(
            (nameof(SelectElement), selectElement.GetId(nameof(SelectElement))),
            (nameof(SelectStarExpression), row.Id));
    }
}
