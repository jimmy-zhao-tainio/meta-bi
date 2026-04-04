using System.Text;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private readonly MetaTransformScriptModel model;

    public MetaTransformScriptSqlEmitter(MetaTransformScriptModel model)
    {
        this.model = model;
    }

    public string Render(SelectStatement root)
    {
        var builder = new StringBuilder();

        var statementBase = GetById(model.StatementWithCtesAndXmlNamespacesList, root.BaseId, "SelectStatement.Base");
        var withCtesLink = FindOwnerLink(model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList, statementBase.Id);
        if (withCtesLink is not null)
        {
            builder.Append(RenderWithClause(withCtesLink.Value));
            builder.AppendLine();
        }

        var queryExpressionLink = GetOwnerLink(model.SelectStatementQueryExpressionLinkList, root.Id, "SelectStatement.QueryExpression");
        builder.Append(RenderQueryExpression(queryExpressionLink.Value));
        return builder.ToString();
    }
}
