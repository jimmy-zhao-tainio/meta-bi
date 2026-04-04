using static MetaTransformScript.Sql.Parsing.MetaTransformScriptOwnedSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptOwnedSqlParser
{
    private sealed partial class Parser
    {
        private ParsedIdentifier ParseIdentifier()
        {
            var token = ParseIdentifierToken();
            return new ParsedIdentifier(token, builder.CreateIdentifier(token.Value, token.QuoteType));
        }

        private List<ParsedIdentifier> ParseIdentifierChain(bool expectTrailingStar = false)
        {
            var identifiers = new List<ParsedIdentifier> { ParseIdentifier() };
            while (Match(MetaTransformScriptOwnedSqlTokenKind.Dot))
            {
                if (expectTrailingStar && Current.Kind == MetaTransformScriptOwnedSqlTokenKind.Star)
                {
                    break;
                }

                identifiers.Add(ParseIdentifier());
            }

            return identifiers;
        }

        private MetaTransformScriptOwnedSqlToken ParseIdentifierToken()
        {
            var token = Current;
            if (token.Kind != MetaTransformScriptOwnedSqlTokenKind.Identifier)
            {
                throw ParseError($"Expected an identifier but found '{token.Text}'.");
            }

            Advance();
            return token;
        }

        private List<MetaTransformScriptOwnedSqlToken> ParseIdentifierTokenChain(bool expectTrailingStar = false)
        {
            var identifiers = new List<MetaTransformScriptOwnedSqlToken> { ParseIdentifierToken() };
            while (Match(MetaTransformScriptOwnedSqlTokenKind.Dot))
            {
                if (expectTrailingStar && Current.Kind == MetaTransformScriptOwnedSqlTokenKind.Star)
                {
                    break;
                }

                identifiers.Add(ParseIdentifierToken());
            }

            return identifiers;
        }

        private BuiltNode ParseMultiPartIdentifier(bool expectTrailingStar = false)
        {
            return builder.CreateMultiPartIdentifier(ParseIdentifierChain(expectTrailingStar).Select(static part => part.Node).ToArray());
        }

        private bool FormsQualifiedStar()
        {
            var probe = position;
            if (PeekToken(probe).Kind != MetaTransformScriptOwnedSqlTokenKind.Identifier)
            {
                return false;
            }

            probe++;
            while (PeekToken(probe).Kind == MetaTransformScriptOwnedSqlTokenKind.Dot)
            {
                probe++;
                if (PeekToken(probe).Kind == MetaTransformScriptOwnedSqlTokenKind.Star)
                {
                    return true;
                }

                if (PeekToken(probe).Kind != MetaTransformScriptOwnedSqlTokenKind.Identifier)
                {
                    return false;
                }

                probe++;
            }

            return false;
        }

        private bool CanStartAlias()
        {
            if (Current.Kind != MetaTransformScriptOwnedSqlTokenKind.Identifier)
            {
                return false;
            }

            return !IsKeyword(Current, ClauseKeywords);
        }
    }
}
