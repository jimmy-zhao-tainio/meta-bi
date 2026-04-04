using static MetaTransformScript.Sql.Parsing.MetaTransformScriptOwnedSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptOwnedSqlParser
{
    private sealed partial class Parser
    {
        private BuiltNode ParseXmlNamespacesClause()
        {
            ExpectKeyword("XMLNAMESPACES");
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);

            var elements = new List<BuiltNode> { ParseXmlNamespacesElement() };
            while (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
            {
                elements.Add(ParseXmlNamespacesElement());
            }

            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return builder.CreateXmlNamespaces(elements);
        }

        private BuiltNode ParseXmlNamespacesElement()
        {
            if (MatchKeyword("DEFAULT"))
            {
                if (Current.Kind != MetaTransformScriptOwnedSqlTokenKind.StringLiteral)
                {
                    throw ParseError($"Expected an XML namespace string literal but found '{Current.Text}'.");
                }

                var defaultLiteralToken = Advance();
                return builder.CreateXmlNamespacesDefaultElement(builder.CreateStringLiteral(defaultLiteralToken.Value));
            }

            if (Current.Kind != MetaTransformScriptOwnedSqlTokenKind.StringLiteral)
            {
                throw ParseError($"Expected an XML namespace string literal but found '{Current.Text}'.");
            }

            var stringLiteralToken = Advance();
            var stringLiteral = builder.CreateStringLiteral(stringLiteralToken.Value);

            BuiltNode? alias = null;
            if (MatchKeyword("AS"))
            {
                alias = ParseIdentifier().Node;
            }

            return builder.CreateXmlNamespacesElement(stringLiteral, alias);
        }
    }
}
