namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptOwnedSqlParser
{
    private sealed partial class Parser
    {
        private MetaTransformScriptOwnedSqlToken Expect(MetaTransformScriptOwnedSqlTokenKind kind)
        {
            if (Current.Kind != kind)
            {
                throw ParseError($"Expected {kind} but found '{Current.Text}'.");
            }

            return Advance();
        }

        private void ExpectKeyword(string keyword)
        {
            if (!MatchKeyword(keyword))
            {
                throw ParseError($"Expected keyword '{keyword}' but found '{Current.Text}'.");
            }
        }

        private bool MatchKeyword(string keyword)
        {
            if (!PeekKeyword(keyword))
            {
                return false;
            }

            Advance();
            return true;
        }

        private bool PeekKeyword(string keyword) => IsKeyword(Current, keyword);

        private bool Match(MetaTransformScriptOwnedSqlTokenKind kind)
        {
            if (Current.Kind != kind)
            {
                return false;
            }

            Advance();
            return true;
        }

        private bool Peek(MetaTransformScriptOwnedSqlTokenKind kind) =>
            Peek().Kind == kind;

        private void SkipSemicolons()
        {
            while (Match(MetaTransformScriptOwnedSqlTokenKind.Semicolon))
            {
            }
        }

        private void ExpectEndOfFile()
        {
            if (Current.Kind != MetaTransformScriptOwnedSqlTokenKind.EndOfFile)
            {
                if (PeekKeyword("GO"))
                {
                    throw Unsupported("GO-separated batches are not supported in parser phase 1.");
                }

                throw ParseError($"Unexpected trailing token '{Current.Text}'.");
            }
        }

        private MetaTransformScriptOwnedSqlToken Advance()
        {
            var token = Current;
            if (position < tokens.Count - 1)
            {
                position++;
            }

            return token;
        }

        private MetaTransformScriptOwnedSqlToken Peek() => PeekToken(position + 1);

        private MetaTransformScriptOwnedSqlToken PeekToken(int absolutePosition) =>
            absolutePosition < tokens.Count ? tokens[absolutePosition] : tokens[^1];

        private MetaTransformScriptOwnedSqlToken Current => tokens[position];

        private static bool IsKeyword(MetaTransformScriptOwnedSqlToken token, string keyword) =>
            token.Kind == MetaTransformScriptOwnedSqlTokenKind.Identifier
            && string.Equals(token.QuoteType, "NotQuoted", StringComparison.Ordinal)
            && string.Equals(token.Value, keyword, StringComparison.OrdinalIgnoreCase);

        private static bool IsKeyword(MetaTransformScriptOwnedSqlToken token, IReadOnlySet<string> keywords) =>
            token.Kind == MetaTransformScriptOwnedSqlTokenKind.Identifier
            && string.Equals(token.QuoteType, "NotQuoted", StringComparison.Ordinal)
            && keywords.Contains(token.Value);

        private static string RenderIdentifier(MetaTransformScriptOwnedSqlToken token) =>
            token.QuoteType switch
            {
                "SquareBracket" => "[" + token.Value.Replace("]", "]]", StringComparison.Ordinal) + "]",
                "DoubleQuote" => "\"" + token.Value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"",
                _ => token.Value
            };

        private MetaTransformScriptOwnedSqlParserException ParseError(string message) =>
            new(MetaTransformScriptOwnedSqlParserFailureKind.ParseError, message, Current.Line, Current.Column, Current.Offset);

        private MetaTransformScriptOwnedSqlParserException Unsupported(string message) =>
            new(MetaTransformScriptOwnedSqlParserFailureKind.UnsupportedSyntax, message, Current.Line, Current.Column, Current.Offset);

        private sealed record ParsedIdentifier(MetaTransformScriptOwnedSqlToken Token, MetaTransformScriptOwnedSqlModelBuilder.BuiltNode Node);
    }
}
