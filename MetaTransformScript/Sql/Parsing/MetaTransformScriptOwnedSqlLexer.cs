namespace MetaTransformScript.Sql.Parsing;

internal enum MetaTransformScriptOwnedSqlTokenKind
{
    Identifier,
    StringLiteral,
    NumberLiteral,
    Comma,
    Dot,
    Star,
    OpenParen,
    CloseParen,
    Plus,
    Equals,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    NotEqual,
    Semicolon,
    EndOfFile
}

internal readonly record struct MetaTransformScriptOwnedSqlToken(
    MetaTransformScriptOwnedSqlTokenKind Kind,
    string Text,
    string Value,
    string QuoteType,
    int Offset,
    int Line,
    int Column);

internal sealed class MetaTransformScriptOwnedSqlLexer
{
    private readonly string text;
    private int index;
    private int line = 1;
    private int column = 1;

    public MetaTransformScriptOwnedSqlLexer(string text)
    {
        this.text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public IReadOnlyList<MetaTransformScriptOwnedSqlToken> Tokenize()
    {
        var tokens = new List<MetaTransformScriptOwnedSqlToken>();

        while (true)
        {
            SkipTrivia();

            if (IsEnd)
            {
                tokens.Add(new MetaTransformScriptOwnedSqlToken(
                    MetaTransformScriptOwnedSqlTokenKind.EndOfFile,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    index,
                    line,
                    column));
                return tokens;
            }

            tokens.Add(Current switch
            {
                '[' => ReadBracketIdentifier(),
                '"' => ReadDoubleQuotedIdentifier(),
                '\'' => ReadStringLiteral(),
                ',' => ReadSingleCharacterToken(MetaTransformScriptOwnedSqlTokenKind.Comma),
                '.' => ReadSingleCharacterToken(MetaTransformScriptOwnedSqlTokenKind.Dot),
                '*' => ReadSingleCharacterToken(MetaTransformScriptOwnedSqlTokenKind.Star),
                '(' => ReadSingleCharacterToken(MetaTransformScriptOwnedSqlTokenKind.OpenParen),
                ')' => ReadSingleCharacterToken(MetaTransformScriptOwnedSqlTokenKind.CloseParen),
                '+' => ReadSingleCharacterToken(MetaTransformScriptOwnedSqlTokenKind.Plus),
                ';' => ReadSingleCharacterToken(MetaTransformScriptOwnedSqlTokenKind.Semicolon),
                '=' => ReadSingleCharacterToken(MetaTransformScriptOwnedSqlTokenKind.Equals),
                '>' => ReadGreaterThan(),
                '<' => ReadLessThan(),
                _ when IsIdentifierStart(Current) => ReadIdentifier(),
                _ when char.IsDigit(Current) => ReadNumberLiteral(),
                _ => throw Error(
                    MetaTransformScriptOwnedSqlParserFailureKind.ParseError,
                    $"Unexpected character '{Current}'.")
            });
        }
    }

    private bool IsEnd => index >= text.Length;

    private char Current => IsEnd ? '\0' : text[index];

    private char Peek(int offset)
    {
        var target = index + offset;
        return target >= 0 && target < text.Length ? text[target] : '\0';
    }

    private void SkipTrivia()
    {
        while (!IsEnd)
        {
            if (char.IsWhiteSpace(Current))
            {
                Advance();
                continue;
            }

            if (Current == '-' && Peek(1) == '-')
            {
                Advance();
                Advance();
                while (!IsEnd && Current != '\r' && Current != '\n')
                {
                    Advance();
                }

                continue;
            }

            if (Current == '/' && Peek(1) == '*')
            {
                Advance();
                Advance();
                while (!IsEnd && !(Current == '*' && Peek(1) == '/'))
                {
                    Advance();
                }

                if (IsEnd)
                {
                    throw Error(
                        MetaTransformScriptOwnedSqlParserFailureKind.ParseError,
                        "Unterminated block comment.");
                }

                Advance();
                Advance();
                continue;
            }

            break;
        }
    }

    private MetaTransformScriptOwnedSqlToken ReadIdentifier()
    {
        var startOffset = index;
        var startLine = line;
        var startColumn = column;

        Advance();
        while (!IsEnd && IsIdentifierPart(Current))
        {
            Advance();
        }

        var value = text[startOffset..index];
        return new MetaTransformScriptOwnedSqlToken(
            MetaTransformScriptOwnedSqlTokenKind.Identifier,
            value,
            value,
            "NotQuoted",
            startOffset,
            startLine,
            startColumn);
    }

    private MetaTransformScriptOwnedSqlToken ReadBracketIdentifier()
    {
        var startOffset = index;
        var startLine = line;
        var startColumn = column;
        Advance();

        var builder = new System.Text.StringBuilder();
        while (!IsEnd)
        {
            if (Current == ']')
            {
                if (Peek(1) == ']')
                {
                    builder.Append(']');
                    Advance();
                    Advance();
                    continue;
                }

                Advance();
                return new MetaTransformScriptOwnedSqlToken(
                    MetaTransformScriptOwnedSqlTokenKind.Identifier,
                    text[startOffset..index],
                    builder.ToString(),
                    "SquareBracket",
                    startOffset,
                    startLine,
                    startColumn);
            }

            builder.Append(Current);
            Advance();
        }

        throw Error(
            MetaTransformScriptOwnedSqlParserFailureKind.ParseError,
            "Unterminated bracketed identifier.");
    }

    private MetaTransformScriptOwnedSqlToken ReadDoubleQuotedIdentifier()
    {
        var startOffset = index;
        var startLine = line;
        var startColumn = column;
        Advance();

        var builder = new System.Text.StringBuilder();
        while (!IsEnd)
        {
            if (Current == '"')
            {
                if (Peek(1) == '"')
                {
                    builder.Append('"');
                    Advance();
                    Advance();
                    continue;
                }

                Advance();
                return new MetaTransformScriptOwnedSqlToken(
                    MetaTransformScriptOwnedSqlTokenKind.Identifier,
                    text[startOffset..index],
                    builder.ToString(),
                    "DoubleQuote",
                    startOffset,
                    startLine,
                    startColumn);
            }

            builder.Append(Current);
            Advance();
        }

        throw Error(
            MetaTransformScriptOwnedSqlParserFailureKind.ParseError,
            "Unterminated double-quoted identifier.");
    }

    private MetaTransformScriptOwnedSqlToken ReadStringLiteral()
    {
        var startOffset = index;
        var startLine = line;
        var startColumn = column;
        Advance();

        var builder = new System.Text.StringBuilder();
        while (!IsEnd)
        {
            if (Current == '\'')
            {
                if (Peek(1) == '\'')
                {
                    builder.Append('\'');
                    Advance();
                    Advance();
                    continue;
                }

                Advance();
                return new MetaTransformScriptOwnedSqlToken(
                    MetaTransformScriptOwnedSqlTokenKind.StringLiteral,
                    text[startOffset..index],
                    builder.ToString(),
                    string.Empty,
                    startOffset,
                    startLine,
                    startColumn);
            }

            builder.Append(Current);
            Advance();
        }

        throw Error(
            MetaTransformScriptOwnedSqlParserFailureKind.ParseError,
            "Unterminated string literal.");
    }

    private MetaTransformScriptOwnedSqlToken ReadNumberLiteral()
    {
        var startOffset = index;
        var startLine = line;
        var startColumn = column;

        while (!IsEnd && char.IsDigit(Current))
        {
            Advance();
        }

        if (!IsEnd && Current == '.' && char.IsDigit(Peek(1)))
        {
            Advance();
            while (!IsEnd && char.IsDigit(Current))
            {
                Advance();
            }
        }

        var value = text[startOffset..index];
        return new MetaTransformScriptOwnedSqlToken(
            MetaTransformScriptOwnedSqlTokenKind.NumberLiteral,
            value,
            value,
            string.Empty,
            startOffset,
            startLine,
            startColumn);
    }

    private MetaTransformScriptOwnedSqlToken ReadSingleCharacterToken(MetaTransformScriptOwnedSqlTokenKind kind)
    {
        var token = new MetaTransformScriptOwnedSqlToken(
            kind,
            Current.ToString(),
            Current.ToString(),
            string.Empty,
            index,
            line,
            column);
        Advance();
        return token;
    }

    private MetaTransformScriptOwnedSqlToken ReadGreaterThan()
    {
        var startOffset = index;
        var startLine = line;
        var startColumn = column;
        Advance();

        if (!IsEnd && Current == '=')
        {
            Advance();
            return new MetaTransformScriptOwnedSqlToken(
                MetaTransformScriptOwnedSqlTokenKind.GreaterThanOrEqual,
                ">=",
                ">=",
                string.Empty,
                startOffset,
                startLine,
                startColumn);
        }

        return new MetaTransformScriptOwnedSqlToken(
            MetaTransformScriptOwnedSqlTokenKind.GreaterThan,
            ">",
            ">",
            string.Empty,
            startOffset,
            startLine,
            startColumn);
    }

    private MetaTransformScriptOwnedSqlToken ReadLessThan()
    {
        var startOffset = index;
        var startLine = line;
        var startColumn = column;
        Advance();

        if (!IsEnd && Current == '=')
        {
            Advance();
            return new MetaTransformScriptOwnedSqlToken(
                MetaTransformScriptOwnedSqlTokenKind.LessThanOrEqual,
                "<=",
                "<=",
                string.Empty,
                startOffset,
                startLine,
                startColumn);
        }

        if (!IsEnd && Current == '>')
        {
            Advance();
            return new MetaTransformScriptOwnedSqlToken(
                MetaTransformScriptOwnedSqlTokenKind.NotEqual,
                "<>",
                "<>",
                string.Empty,
                startOffset,
                startLine,
                startColumn);
        }

        return new MetaTransformScriptOwnedSqlToken(
            MetaTransformScriptOwnedSqlTokenKind.LessThan,
            "<",
            "<",
            string.Empty,
            startOffset,
            startLine,
            startColumn);
    }

    private void Advance()
    {
        if (IsEnd)
        {
            return;
        }

        if (Current == '\r')
        {
            index++;
            if (!IsEnd && Current == '\n')
            {
                index++;
            }

            line++;
            column = 1;
            return;
        }

        if (Current == '\n')
        {
            index++;
            line++;
            column = 1;
            return;
        }

        index++;
        column++;
    }

    private static bool IsIdentifierStart(char value) =>
        char.IsLetter(value) || value is '_' or '@' or '#';

    private static bool IsIdentifierPart(char value) =>
        char.IsLetterOrDigit(value) || value is '_' or '@' or '#' or '$';

    private MetaTransformScriptOwnedSqlParserException Error(
        MetaTransformScriptOwnedSqlParserFailureKind failureKind,
        string message) =>
        new(failureKind, message, line, column, index);
}
