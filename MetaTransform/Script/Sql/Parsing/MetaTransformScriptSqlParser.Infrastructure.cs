namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptSqlParser
{
    private sealed partial class Parser
    {
        private MetaTransformScriptSqlToken Expect(MetaTransformScriptSqlTokenKind kind)
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

        private bool Match(MetaTransformScriptSqlTokenKind kind)
        {
            if (Current.Kind != kind)
            {
                return false;
            }

            Advance();
            return true;
        }

        private bool Peek(MetaTransformScriptSqlTokenKind kind) =>
            Peek().Kind == kind;

        private void SkipSemicolons()
        {
            while (Match(MetaTransformScriptSqlTokenKind.Semicolon))
            {
            }
        }

        private void ExpectEndOfFile()
        {
            if (Current.Kind != MetaTransformScriptSqlTokenKind.EndOfFile)
            {
                if (PeekKeyword("GO"))
                {
                    throw Unsupported("GO-separated batches are not supported in direct parser input.");
                }

                throw ParseError($"Unexpected trailing token '{Current.Text}'.");
            }
        }

        private MetaTransformScriptSqlToken Advance()
        {
            var token = Current;
            if (position < tokens.Count - 1)
            {
                position++;
            }

            return token;
        }

        private MetaTransformScriptSqlToken Peek() => PeekToken(position + 1);

        private MetaTransformScriptSqlToken PeekToken(int absolutePosition) =>
            absolutePosition < tokens.Count ? tokens[absolutePosition] : tokens[^1];

        private MetaTransformScriptSqlToken Current => tokens[position];

        private static bool IsKeyword(MetaTransformScriptSqlToken token, string keyword) =>
            token.Kind == MetaTransformScriptSqlTokenKind.Identifier
            && string.Equals(token.QuoteType, "NotQuoted", StringComparison.Ordinal)
            && string.Equals(token.Value, keyword, StringComparison.OrdinalIgnoreCase);

        private static bool IsKeyword(MetaTransformScriptSqlToken token, IReadOnlySet<string> keywords) =>
            token.Kind == MetaTransformScriptSqlTokenKind.Identifier
            && string.Equals(token.QuoteType, "NotQuoted", StringComparison.Ordinal)
            && keywords.Contains(token.Value);

        private static string RenderIdentifier(MetaTransformScriptSqlToken token) =>
            token.QuoteType switch
            {
                "SquareBracket" => "[" + token.Value.Replace("]", "]]", StringComparison.Ordinal) + "]",
                "DoubleQuote" => "\"" + token.Value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"",
                "Backtick" => "`" + token.Value.Replace("`", "``", StringComparison.Ordinal) + "`",
                _ => token.Value
            };

        private MetaTransformScriptSqlParserException ParseError(string message) =>
            new(MetaTransformScriptSqlParserFailureKind.ParseError, message, Current.Line, Current.Column, Current.Offset);

        private MetaTransformScriptSqlParserException Unsupported(string message) =>
            new(MetaTransformScriptSqlParserFailureKind.UnsupportedSyntax, message, Current.Line, Current.Column, Current.Offset);

        private sealed record ParsedIdentifier(MetaTransformScriptSqlToken Token, MetaTransformScriptSqlModelBuilder.BuiltNode Node);
    }
}
