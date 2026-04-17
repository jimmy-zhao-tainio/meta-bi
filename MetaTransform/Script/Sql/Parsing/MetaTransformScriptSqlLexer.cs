namespace MetaTransformScript.Sql.Parsing;

internal enum MetaTransformScriptSqlTokenKind
{
    Identifier,
    StringLiteral,
    NumberLiteral,
    BinaryLiteral,
    Comma,
    Dot,
    Star,
    Slash,
    Percent,
    OpenParen,
    CloseParen,
    Plus,
    Minus,
    Equals,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    NotEqual,
    Semicolon,
    EndOfFile
}

internal readonly record struct MetaTransformScriptSqlToken(
    MetaTransformScriptSqlTokenKind Kind,
    string Text,
    string Value,
    string QuoteType,
    int Offset,
    int Line,
    int Column);

internal sealed class MetaTransformScriptSqlLexer
{
    private readonly string text;
    private int index;
    private int line = 1;
    private int column = 1;

    public MetaTransformScriptSqlLexer(string text)
    {
        this.text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public IReadOnlyList<MetaTransformScriptSqlToken> Tokenize()
    {
        var tokens = new List<MetaTransformScriptSqlToken>();

        while (true)
        {
            SkipTrivia();

            if (IsEnd)
            {
                tokens.Add(new MetaTransformScriptSqlToken(
                    MetaTransformScriptSqlTokenKind.EndOfFile,
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
                '`' => ReadBacktickQuotedIdentifier(),
                '\'' => ReadStringLiteral(),
                ',' => ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind.Comma),
                '.' => ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind.Dot),
                '*' => ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind.Star),
                '/' => ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind.Slash),
                '%' => ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind.Percent),
                '(' => ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind.OpenParen),
                ')' => ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind.CloseParen),
                '+' => ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind.Plus),
                '-' => ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind.Minus),
                ';' => ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind.Semicolon),
                '=' => ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind.Equals),
                '>' => ReadGreaterThan(),
                '<' => ReadLessThan(),
                _ when IsIdentifierStart(Current) => ReadIdentifier(),
                _ when char.IsDigit(Current) => ReadNumberLiteral(),
                _ => throw Error(
                    MetaTransformScriptSqlParserFailureKind.ParseError,
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
                        MetaTransformScriptSqlParserFailureKind.ParseError,
                        "Unterminated block comment.");
                }

                Advance();
                Advance();
                continue;
            }

            break;
        }
    }

    private MetaTransformScriptSqlToken ReadIdentifier()
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
        return new MetaTransformScriptSqlToken(
            MetaTransformScriptSqlTokenKind.Identifier,
            value,
            value,
            "NotQuoted",
            startOffset,
            startLine,
            startColumn);
    }

    private MetaTransformScriptSqlToken ReadBracketIdentifier()
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
                return new MetaTransformScriptSqlToken(
                    MetaTransformScriptSqlTokenKind.Identifier,
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
            MetaTransformScriptSqlParserFailureKind.ParseError,
            "Unterminated bracketed identifier.");
    }

    private MetaTransformScriptSqlToken ReadDoubleQuotedIdentifier()
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
                return new MetaTransformScriptSqlToken(
                    MetaTransformScriptSqlTokenKind.Identifier,
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
            MetaTransformScriptSqlParserFailureKind.ParseError,
            "Unterminated double-quoted identifier.");
    }

    private MetaTransformScriptSqlToken ReadBacktickQuotedIdentifier()
    {
        var startOffset = index;
        var startLine = line;
        var startColumn = column;
        Advance();

        var builder = new System.Text.StringBuilder();
        while (!IsEnd)
        {
            if (Current == '`')
            {
                if (Peek(1) == '`')
                {
                    builder.Append('`');
                    Advance();
                    Advance();
                    continue;
                }

                Advance();
                return new MetaTransformScriptSqlToken(
                    MetaTransformScriptSqlTokenKind.Identifier,
                    text[startOffset..index],
                    builder.ToString(),
                    "Backtick",
                    startOffset,
                    startLine,
                    startColumn);
            }

            builder.Append(Current);
            Advance();
        }

        throw Error(
            MetaTransformScriptSqlParserFailureKind.ParseError,
            "Unterminated backtick-quoted identifier.");
    }

    private MetaTransformScriptSqlToken ReadStringLiteral()
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
                return new MetaTransformScriptSqlToken(
                    MetaTransformScriptSqlTokenKind.StringLiteral,
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
            MetaTransformScriptSqlParserFailureKind.ParseError,
            "Unterminated string literal.");
    }

    private MetaTransformScriptSqlToken ReadNumberLiteral()
    {
        var startOffset = index;
        var startLine = line;
        var startColumn = column;

        if (Current == '0' && (Peek(1) is 'x' or 'X'))
        {
            Advance();
            Advance();
            if (IsEnd || !IsHexDigit(Current))
            {
                throw Error(
                    MetaTransformScriptSqlParserFailureKind.ParseError,
                    "Expected hexadecimal digits after binary literal prefix.");
            }

            while (!IsEnd && IsHexDigit(Current))
            {
                Advance();
            }

            var binaryValue = text[startOffset..index];
            return new MetaTransformScriptSqlToken(
                MetaTransformScriptSqlTokenKind.BinaryLiteral,
                binaryValue,
                binaryValue,
                string.Empty,
                startOffset,
                startLine,
                startColumn);
        }

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

        if (!IsEnd && (Current is 'E' or 'e'))
        {
            var exponentStart = index;
            Advance();

            if (!IsEnd && (Current is '+' or '-'))
            {
                Advance();
            }

            if (IsEnd || !char.IsDigit(Current))
            {
                index = exponentStart;
            }
            else
            {
                while (!IsEnd && char.IsDigit(Current))
                {
                    Advance();
                }
            }
        }

        var value = text[startOffset..index];
        return new MetaTransformScriptSqlToken(
            MetaTransformScriptSqlTokenKind.NumberLiteral,
            value,
            value,
            string.Empty,
            startOffset,
            startLine,
            startColumn);
    }

    private MetaTransformScriptSqlToken ReadSingleCharacterToken(MetaTransformScriptSqlTokenKind kind)
    {
        var token = new MetaTransformScriptSqlToken(
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

    private MetaTransformScriptSqlToken ReadGreaterThan()
    {
        var startOffset = index;
        var startLine = line;
        var startColumn = column;
        Advance();

        if (!IsEnd && Current == '=')
        {
            Advance();
            return new MetaTransformScriptSqlToken(
                MetaTransformScriptSqlTokenKind.GreaterThanOrEqual,
                ">=",
                ">=",
                string.Empty,
                startOffset,
                startLine,
                startColumn);
        }

        return new MetaTransformScriptSqlToken(
            MetaTransformScriptSqlTokenKind.GreaterThan,
            ">",
            ">",
            string.Empty,
            startOffset,
            startLine,
            startColumn);
    }

    private MetaTransformScriptSqlToken ReadLessThan()
    {
        var startOffset = index;
        var startLine = line;
        var startColumn = column;
        Advance();

        if (!IsEnd && Current == '=')
        {
            Advance();
            return new MetaTransformScriptSqlToken(
                MetaTransformScriptSqlTokenKind.LessThanOrEqual,
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
            return new MetaTransformScriptSqlToken(
                MetaTransformScriptSqlTokenKind.NotEqual,
                "<>",
                "<>",
                string.Empty,
                startOffset,
                startLine,
                startColumn);
        }

        return new MetaTransformScriptSqlToken(
            MetaTransformScriptSqlTokenKind.LessThan,
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

    private static bool IsHexDigit(char value) =>
        value is >= '0' and <= '9'
        or >= 'a' and <= 'f'
        or >= 'A' and <= 'F';

    private MetaTransformScriptSqlParserException Error(
        MetaTransformScriptSqlParserFailureKind failureKind,
        string message) =>
        new(failureKind, message, line, column, index);
}
