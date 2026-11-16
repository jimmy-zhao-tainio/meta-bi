using System.Text;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private readonly MetaTransformScriptModel model;
    private readonly MetaTransformScriptSqlModelIndex index = new();

    public MetaTransformScriptSqlEmitter(MetaTransformScriptModel model)
    {
        this.model = model;
    }

    public string Render(TSqlStatement root)
    {
        var statementWithCtes = FindByBaseId(model.StatementWithCtesAndXmlNamespacesList, static row => row.TSqlStatement?.Id, nameof(StatementWithCtesAndXmlNamespaces.TSqlStatement), root.Id);
        if (statementWithCtes is not null)
        {
            var selectStatement = FindByBaseId(model.SelectStatementList, static row => row.StatementWithCtesAndXmlNamespaces?.Id, nameof(SelectStatement.StatementWithCtesAndXmlNamespaces), statementWithCtes.Id);
            if (selectStatement is not null)
            {
                return RenderStatementWithCtesAndXmlNamespaces(
                    statementWithCtes,
                    RenderSelectStatementBody(selectStatement));
            }

            var insertStatement = FindByBaseId(model.InsertStatementList, static row => row.StatementWithCtesAndXmlNamespaces?.Id, nameof(InsertStatement.StatementWithCtesAndXmlNamespaces), statementWithCtes.Id);
            if (insertStatement is not null)
            {
                return RenderStatementWithCtesAndXmlNamespaces(
                    statementWithCtes,
                    RenderInsertStatementBody(insertStatement));
            }

            var updateStatement = FindByBaseId(model.UpdateStatementList, static row => row.StatementWithCtesAndXmlNamespaces?.Id, nameof(UpdateStatement.StatementWithCtesAndXmlNamespaces), statementWithCtes.Id);
            if (updateStatement is not null)
            {
                return RenderStatementWithCtesAndXmlNamespaces(
                    statementWithCtes,
                    RenderUpdateStatementBody(updateStatement));
            }

            var deleteStatement = FindByBaseId(model.DeleteStatementList, static row => row.StatementWithCtesAndXmlNamespaces?.Id, nameof(DeleteStatement.StatementWithCtesAndXmlNamespaces), statementWithCtes.Id);
            if (deleteStatement is not null)
            {
                return RenderStatementWithCtesAndXmlNamespaces(
                    statementWithCtes,
                    RenderDeleteStatementBody(deleteStatement));
            }

            var mergeStatement = FindByBaseId(model.MergeStatementList, static row => row.StatementWithCtesAndXmlNamespaces?.Id, nameof(MergeStatement.StatementWithCtesAndXmlNamespaces), statementWithCtes.Id);
            if (mergeStatement is not null)
            {
                return RenderStatementWithCtesAndXmlNamespaces(
                    statementWithCtes,
                    RenderMergeStatementBody(mergeStatement));
            }

            throw new InvalidOperationException($"Unsupported MetaTransformScript statement-with-CTEs id '{statementWithCtes.Id}'.");
        }

        var truncateStatement = FindByBaseId(model.TruncateStatementList, static row => row.TSqlStatement?.Id, nameof(TruncateStatement.TSqlStatement), root.Id);
        if (truncateStatement is not null)
        {
            return RenderTruncateStatementBody(truncateStatement);
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript TSqlStatement id '{root.Id}'.");
    }

    public string Render(SelectStatement root)
    {
        var statementBase = GetById(model.StatementWithCtesAndXmlNamespacesList, static row => row.Id, root.StatementWithCtesAndXmlNamespaces.Id, "SelectStatement.Base");
        return RenderStatementWithCtesAndXmlNamespaces(statementBase, RenderSelectStatementBody(root));
    }

    private string RenderStatementWithCtesAndXmlNamespaces(
        StatementWithCtesAndXmlNamespaces statementBase,
        string body)
    {
        var builder = new StringBuilder();

        var withCtesLink = FindOwnerLink(model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList, static row => row.StatementWithCtesAndXmlNamespaces?.Id, nameof(StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink.StatementWithCtesAndXmlNamespaces), statementBase.Id);
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
        var queryExpressionLink = GetOwnerLink(model.SelectStatementQueryExpressionLinkList, static row => row.SelectStatement?.Id, nameof(SelectStatementQueryExpressionLink.SelectStatement), root.Id, "SelectStatement.QueryExpression");
        return RenderQueryExpression(queryExpressionLink.QueryExpression);
    }
}
