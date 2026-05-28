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

    public string Render(TSqlStatement root)
    {
        var statementWithCtes = FindByBaseId(model.StatementWithCtesAndXmlNamespacesList, root.Id);
        if (statementWithCtes is not null)
        {
            var selectStatement = FindByBaseId(model.SelectStatementList, statementWithCtes.Id);
            if (selectStatement is not null)
            {
                return RenderStatementWithCtesAndXmlNamespaces(
                    statementWithCtes,
                    RenderSelectStatementBody(selectStatement));
            }

            var insertStatement = FindByBaseId(model.InsertStatementList, statementWithCtes.Id);
            if (insertStatement is not null)
            {
                return RenderStatementWithCtesAndXmlNamespaces(
                    statementWithCtes,
                    RenderInsertStatementBody(insertStatement));
            }

            var updateStatement = FindByBaseId(model.UpdateStatementList, statementWithCtes.Id);
            if (updateStatement is not null)
            {
                return RenderStatementWithCtesAndXmlNamespaces(
                    statementWithCtes,
                    RenderUpdateStatementBody(updateStatement));
            }

            var deleteStatement = FindByBaseId(model.DeleteStatementList, statementWithCtes.Id);
            if (deleteStatement is not null)
            {
                return RenderStatementWithCtesAndXmlNamespaces(
                    statementWithCtes,
                    RenderDeleteStatementBody(deleteStatement));
            }

            var mergeStatement = FindByBaseId(model.MergeStatementList, statementWithCtes.Id);
            if (mergeStatement is not null)
            {
                return RenderStatementWithCtesAndXmlNamespaces(
                    statementWithCtes,
                    RenderMergeStatementBody(mergeStatement));
            }

            throw new InvalidOperationException($"Unsupported MetaTransformScript statement-with-CTEs id '{statementWithCtes.Id}'.");
        }

        var truncateStatement = FindByBaseId(model.TruncateStatementList, root.Id);
        if (truncateStatement is not null)
        {
            return RenderTruncateStatementBody(truncateStatement);
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript TSqlStatement id '{root.Id}'.");
    }

    public string Render(SelectStatement root)
    {
        var statementBase = GetById(model.StatementWithCtesAndXmlNamespacesList, root.StatementWithCtesAndXmlNamespaces.Id, "SelectStatement.Base");
        return RenderStatementWithCtesAndXmlNamespaces(statementBase, RenderSelectStatementBody(root));
    }

    private string RenderStatementWithCtesAndXmlNamespaces(
        StatementWithCtesAndXmlNamespaces statementBase,
        string body)
    {
        var builder = new StringBuilder();

        var withCtesLink = FindOwnerLink(model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList, statementBase.Id);
        if (withCtesLink is not null)
        {
            builder.Append(RenderWithClause(withCtesLink.WithCtesAndXmlNamespaces));
            builder.AppendLine();
        }

        builder.Append(body);
        return builder.ToString();
    }

    private string RenderSelectStatementBody(SelectStatement root)
    {
        var queryExpressionLink = GetOwnerLink(model.SelectStatementQueryExpressionLinkList, root.Id, "SelectStatement.QueryExpression");
        return RenderQueryExpression(queryExpressionLink.QueryExpression);
    }
}
