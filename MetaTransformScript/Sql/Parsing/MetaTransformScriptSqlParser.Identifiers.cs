using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptSqlParser
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
            while (Match(MetaTransformScriptSqlTokenKind.Dot))
            {
                if (expectTrailingStar && Current.Kind == MetaTransformScriptSqlTokenKind.Star)
                {
                    break;
                }

                identifiers.Add(ParseIdentifier());
            }

            return identifiers;
        }

        private MetaTransformScriptSqlToken ParseIdentifierToken()
        {
            var token = Current;
            if (token.Kind != MetaTransformScriptSqlTokenKind.Identifier)
            {
                throw ParseError($"Expected an identifier but found '{token.Text}'.");
            }

            Advance();
            return token;
        }

        private List<MetaTransformScriptSqlToken> ParseIdentifierTokenChain(bool expectTrailingStar = false)
        {
            var identifiers = new List<MetaTransformScriptSqlToken> { ParseIdentifierToken() };
            while (Match(MetaTransformScriptSqlTokenKind.Dot))
            {
                if (expectTrailingStar && Current.Kind == MetaTransformScriptSqlTokenKind.Star)
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
            if (PeekToken(probe).Kind != MetaTransformScriptSqlTokenKind.Identifier)
            {
                return false;
            }

            probe++;
            while (PeekToken(probe).Kind == MetaTransformScriptSqlTokenKind.Dot)
            {
                probe++;
                if (PeekToken(probe).Kind == MetaTransformScriptSqlTokenKind.Star)
                {
                    return true;
                }

                if (PeekToken(probe).Kind != MetaTransformScriptSqlTokenKind.Identifier)
                {
                    return false;
                }

                probe++;
            }

            return false;
        }

        private bool CanStartAlias()
        {
            if (Current.Kind != MetaTransformScriptSqlTokenKind.Identifier)
            {
                return false;
            }

            return !IsKeyword(Current, ClauseKeywords);
        }
    }
}
