using static MetaTransformScript.Sql.Parsing.MetaTransformScriptOwnedSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptOwnedSqlParser
{
    private sealed partial class Parser
    {
        private BuiltNode ParseWindowClause()
        {
            var windowDefinitions = new List<BuiltNode> { ParseWindowDefinition() };
            while (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
            {
                windowDefinitions.Add(ParseWindowDefinition());
            }

            return builder.CreateWindowClause(windowDefinitions);
        }

        private BuiltNode ParseWindowDefinition()
        {
            var windowName = ParseIdentifier().Node;
            ExpectKeyword("AS");
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);

            BuiltNode? refWindowName = null;
            if (CanStartWindowReferenceName())
            {
                refWindowName = ParseIdentifier().Node;
            }

            var partitions = ParseOptionalWindowPartitions();
            BuiltNode? orderByClause = null;
            if (MatchKeyword("ORDER"))
            {
                ExpectKeyword("BY");
                orderByClause = ParseOrderByClause();
            }

            BuiltNode? windowFrameClause = null;
            if (PeekKeyword("ROWS") || PeekKeyword("RANGE"))
            {
                windowFrameClause = ParseWindowFrameClause();
            }

            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return builder.CreateWindowDefinition(windowName, refWindowName, partitions, orderByClause, windowFrameClause);
        }

        private BuiltNode ParseOverClause()
        {
            ExpectKeyword("OVER");

            if (Current.Kind == MetaTransformScriptOwnedSqlTokenKind.Identifier)
            {
                return builder.CreateOverClause(windowName: ParseIdentifier().Node);
            }

            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);

            var partitions = ParseOptionalWindowPartitions();
            BuiltNode? orderByClause = null;
            if (MatchKeyword("ORDER"))
            {
                ExpectKeyword("BY");
                orderByClause = ParseOrderByClause();
            }

            BuiltNode? windowFrameClause = null;
            if (PeekKeyword("ROWS") || PeekKeyword("RANGE"))
            {
                windowFrameClause = ParseWindowFrameClause();
            }

            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return builder.CreateOverClause(partitions: partitions, orderByClause: orderByClause, windowFrameClause: windowFrameClause);
        }

        private List<BuiltNode>? ParseOptionalWindowPartitions()
        {
            if (!MatchKeyword("PARTITION"))
            {
                return null;
            }

            ExpectKeyword("BY");
            var partitions = new List<BuiltNode> { ParseScalarExpression() };
            while (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
            {
                partitions.Add(ParseScalarExpression());
            }

            return partitions;
        }

        private BuiltNode ParseWindowFrameClause()
        {
            var windowFrameType =
                MatchKeyword("ROWS") ? "Rows" :
                MatchKeyword("RANGE") ? "Range" :
                throw ParseError($"Expected window frame type but found '{Current.Text}'.");

            if (MatchKeyword("BETWEEN"))
            {
                var top = ParseWindowDelimiter();
                ExpectKeyword("AND");
                var bottom = ParseWindowDelimiter();
                return builder.CreateWindowFrameClause(windowFrameType, top, bottom);
            }

            return builder.CreateWindowFrameClause(windowFrameType, ParseWindowDelimiter());
        }

        private BuiltNode ParseWindowDelimiter()
        {
            if (MatchKeyword("CURRENT"))
            {
                ExpectKeyword("ROW");
                return builder.CreateWindowDelimiter("CurrentRow");
            }

            if (MatchKeyword("UNBOUNDED"))
            {
                if (MatchKeyword("PRECEDING"))
                {
                    return builder.CreateWindowDelimiter("UnboundedPreceding");
                }

                if (MatchKeyword("FOLLOWING"))
                {
                    return builder.CreateWindowDelimiter("UnboundedFollowing");
                }
            }

            if (CanStartWindowOffsetExpression())
            {
                var offsetValue = ParseScalarExpression();
                if (MatchKeyword("PRECEDING"))
                {
                    return builder.CreateWindowDelimiter("ValuePreceding", offsetValue);
                }

                if (MatchKeyword("FOLLOWING"))
                {
                    return builder.CreateWindowDelimiter("ValueFollowing", offsetValue);
                }

                throw ParseError($"Expected PRECEDING or FOLLOWING but found '{Current.Text}'.");
            }

            throw ParseError($"Expected a window frame delimiter but found '{Current.Text}'.");
        }

        private bool CanStartWindowOffsetExpression() =>
            Current.Kind is MetaTransformScriptOwnedSqlTokenKind.NumberLiteral
                or MetaTransformScriptOwnedSqlTokenKind.Identifier
                or MetaTransformScriptOwnedSqlTokenKind.OpenParen
                or MetaTransformScriptOwnedSqlTokenKind.Plus;

        private bool CanStartWindowReferenceName()
        {
            if (Current.Kind != MetaTransformScriptOwnedSqlTokenKind.Identifier)
            {
                return false;
            }

            return !PeekKeyword("PARTITION")
                && !PeekKeyword("ORDER")
                && !PeekKeyword("ROWS")
                && !PeekKeyword("RANGE");
        }
    }
}
