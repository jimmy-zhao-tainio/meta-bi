using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private IReadOnlySet<string> FindConjunctiveNonNullableColumns(BooleanExpression? expression)
    {
        var columnIds = new HashSet<string>(StringComparer.Ordinal);
        CollectConjunctiveNonNullableColumns(expression, columnIds);
        return columnIds;
    }

    private void CollectConjunctiveNonNullableColumns(
        BooleanExpression? expression,
        HashSet<string> columnIds)
    {
        if (expression is null)
        {
            return;
        }

        var parenthesisExpression = navigator.TryGetBooleanParenthesisExpression(expression);
        if (parenthesisExpression is not null)
        {
            CollectConjunctiveNonNullableColumns(
                navigator.TryGetBooleanParenthesisExpressionOperand(parenthesisExpression),
                columnIds);
            return;
        }

        var binaryExpression = navigator.TryGetBooleanBinaryExpression(expression);
        if (binaryExpression is not null)
        {
            if (!string.Equals(binaryExpression.BinaryExpressionType, "And", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var children = navigator.TryGetBooleanBinaryExpressionChildren(binaryExpression);
            if (children is not null)
            {
                CollectConjunctiveNonNullableColumns(children.Value.First, columnIds);
                CollectConjunctiveNonNullableColumns(children.Value.Second, columnIds);
            }

            return;
        }

        var isNullExpression = navigator.TryGetBooleanIsNullExpression(expression);
        if (isNullExpression is null ||
            !string.Equals(isNullExpression.IsNot, "true", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var operand = navigator.TryGetBooleanIsNullExpressionOperand(isNullExpression);
        var directColumnReference = operand is null
            ? null
            : navigator.TryGetDirectColumnReference(operand);
        if (directColumnReference is null)
        {
            return;
        }

        var boundColumnReference = boundColumnReferences.LastOrDefault(item => string.Equals(
            item.SyntaxColumnReferenceId,
            directColumnReference.Id,
            StringComparison.Ordinal));
        if (boundColumnReference is not null)
        {
            columnIds.Add(boundColumnReference.ResolvedColumn.Id);
        }
    }
}
