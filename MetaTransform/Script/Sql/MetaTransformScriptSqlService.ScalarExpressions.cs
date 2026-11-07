using System.Collections;
using MetaTransformScript.Sql.Parsing;
using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql;

public sealed record SqlScalarExpression(MetaTransformScriptModel Model, ScalarExpression Root);

public sealed partial class MetaTransformScriptSqlService
{
    /// <summary>Substitute one source-column occurrence structurally, including occurrences inside larger expressions.</summary>
    public void ReplaceColumnReference(MetaTransformScriptModel model, ColumnReferenceExpression reference, string sql, string outputName)
    {
        var original = reference.PrimaryExpression.ScalarExpression;
        var builder = new MetaTransformScriptSqlModelBuilder();
        var root = ParseScalarInput(sql, builder).GetRef<ScalarExpression>(nameof(ScalarExpression));
        var aliases = new List<(SelectScalarExpression Projection, IdentifierOrValueExpression Alias)>();
        foreach (var link in model.SelectScalarExpressionExpressionLinkList.Where(row => row.ScalarExpression.Id == original.Id))
        {
            if (!model.SelectScalarExpressionColumnNameLinkList.Any(row => row.SelectScalarExpression.Id == link.SelectScalarExpression.Id))
                aliases.Add((link.SelectScalarExpression, builder.CreateIdentifierOrValueExpression(builder.CreateIdentifier(outputName, "SquareBracket"))
                    .GetRef<IdentifierOrValueExpression>(nameof(IdentifierOrValueExpression))));
        }
        var fragment = builder.Build();
        RemapModelIds(fragment, ReadNextIdState(model));
        model.ColumnReferenceExpressionList.Remove(reference);
        model.PrimaryExpressionList.RemoveAll(row => row.Id == reference.PrimaryExpression.Id);
        model.ScalarExpressionList.RemoveAll(row => row.Id == original.Id);
        // Rewrite scalar edges, not SQL text. The detached identifier subgraph is discarded at normalization.
        foreach (var list in GetModelListProperties())
            foreach (var row in (IEnumerable)list.GetValue(model)!)
                foreach (var property in row.GetType().GetProperties().Where(property => property.CanWrite && property.PropertyType == typeof(ScalarExpression)))
                    if (property.GetValue(row) is ScalarExpression expression && expression.Id == original.Id)
                        property.SetValue(row, root);
        AppendModelRows(model, fragment);
        foreach (var alias in aliases)
            model.SelectScalarExpressionColumnNameLinkList.Add(new SelectScalarExpressionColumnNameLink
            {
                Id = "column-alias:" + alias.Projection.Id, SelectScalarExpression = alias.Projection, IdentifierOrValueExpression = alias.Alias
            });
    }

    public SqlScalarExpression ImportScalarExpression(string sql)
    {
        var builder = new MetaTransformScriptSqlModelBuilder();
        var expression = ParseScalarInput(sql, builder);
        return new(builder.Build(), expression.GetRef<ScalarExpression>(nameof(ScalarExpression)));
    }

    public string ExportScalarExpression(SqlScalarExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        return new MetaTransformScriptSqlEmitter(expression.Model).RenderScalarExpressionForScriptObject(expression.Root);
    }

    private static BuiltNode ParseScalarInput(string sql, MetaTransformScriptSqlModelBuilder builder)
    {
        try { return new MetaTransformScriptSqlParser().ParseScalarFragment(sql, builder); }
        catch (MetaTransformScriptSqlParserException exception) { throw CreateImportException(exception, sourcePath: null); }
    }
}
