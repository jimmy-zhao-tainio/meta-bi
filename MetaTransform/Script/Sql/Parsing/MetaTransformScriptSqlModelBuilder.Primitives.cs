using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    public BuiltNode CreateIdentifier(string value, string quoteType)
    {
        var row = new Identifier
        {
            Id = NextId(nameof(Identifier)),
            Value = value,
            QuoteType = quoteType
        };
        model.IdentifierList.Add(row);
        return BuiltNode.Create((nameof(Identifier), row.Id));
    }

    public BuiltNode CreateIdentifierOrValueExpression(BuiltNode identifier)
    {
        var row = new IdentifierOrValueExpression
        {
            Id = NextId(nameof(IdentifierOrValueExpression))
        };
        model.IdentifierOrValueExpressionList.Add(row);
        model.IdentifierOrValueExpressionIdentifierLinkList.Add(new IdentifierOrValueExpressionIdentifierLink
        {
            Id = NextId(nameof(IdentifierOrValueExpressionIdentifierLink)),
            IdentifierOrValueExpression = row,
            Identifier = identifier.GetRef<Identifier>(nameof(Identifier))
        });
        return BuiltNode.Create((nameof(IdentifierOrValueExpression), row.Id));
    }

    public BuiltNode CreateMultiPartIdentifier(IReadOnlyList<BuiltNode> identifiers)
    {
        ArgumentNullException.ThrowIfNull(identifiers);
        if (identifiers.Count == 0)
        {
            throw new InvalidOperationException("MultiPartIdentifier requires at least one Identifier.");
        }

        var row = new MultiPartIdentifier
        {
            Id = NextId(nameof(MultiPartIdentifier)),
            Count = identifiers.Count.ToString(CultureInfo.InvariantCulture)
        };
        model.MultiPartIdentifierList.Add(row);

        for (var ordinal = 0; ordinal < identifiers.Count; ordinal++)
        {
            model.MultiPartIdentifierIdentifiersItemList.Add(new MultiPartIdentifierIdentifiersItem
            {
                Id = NextId(nameof(MultiPartIdentifierIdentifiersItem)),
                MultiPartIdentifier = row,
                Identifier = identifiers[ordinal].GetRef<Identifier>(nameof(Identifier)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(MultiPartIdentifier), row.Id));
    }

    public BuiltNode CreateSchemaObjectName(IReadOnlyList<BuiltNode> identifiers)
    {
        ArgumentNullException.ThrowIfNull(identifiers);
        if (identifiers.Count == 0)
        {
            throw new InvalidOperationException("SchemaObjectName requires at least one Identifier.");
        }

        var multiPart = CreateMultiPartIdentifier(identifiers);
        var baseIdentifier = identifiers[^1];
        var schemaIdentifier = identifiers.Count >= 2 ? identifiers[^2] : null;

        var row = new SchemaObjectName
        {
            Id = NextId(nameof(SchemaObjectName)),
            MultiPartIdentifier = multiPart.GetRef<MultiPartIdentifier>(nameof(MultiPartIdentifier))
        };
        model.SchemaObjectNameList.Add(row);
        model.SchemaObjectNameBaseIdentifierLinkList.Add(new SchemaObjectNameBaseIdentifierLink
        {
            Id = NextId(nameof(SchemaObjectNameBaseIdentifierLink)),
            SchemaObjectName = row,
            Identifier = baseIdentifier.GetRef<Identifier>(nameof(Identifier))
        });

        if (schemaIdentifier is not null)
        {
            model.SchemaObjectNameSchemaIdentifierLinkList.Add(new SchemaObjectNameSchemaIdentifierLink
            {
                Id = NextId(nameof(SchemaObjectNameSchemaIdentifierLink)),
                SchemaObjectName = row,
                Identifier = schemaIdentifier.GetRef<Identifier>(nameof(Identifier))
            });
        }

        return BuiltNode.Create(
            (nameof(MultiPartIdentifier), multiPart.GetId(nameof(MultiPartIdentifier))),
            (nameof(SchemaObjectName), row.Id));
    }

    public BuiltNode CreateStringLiteral(string value, bool isNational = false)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var valueExpression = new ValueExpression
        {
            Id = NextId(nameof(ValueExpression)),
            PrimaryExpression = primary
        };
        model.ValueExpressionList.Add(valueExpression);

        var literal = new Literal
        {
            Id = NextId(nameof(Literal)),
            ValueExpression = valueExpression,
            LiteralType = "String",
            Value = value
        };
        model.LiteralList.Add(literal);

        var stringLiteral = new StringLiteral
        {
            Id = NextId(nameof(StringLiteral)),
            Literal = literal,
            LiteralType = "String",
            IsNational = isNational ? "true" : string.Empty
        };
        model.StringLiteralList.Add(stringLiteral);

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ValueExpression), valueExpression.Id),
            (nameof(Literal), literal.Id),
            (nameof(StringLiteral), stringLiteral.Id));
    }

    public BuiltNode CreateNumberLiteral(string value)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var valueExpression = new ValueExpression
        {
            Id = NextId(nameof(ValueExpression)),
            PrimaryExpression = primary
        };
        model.ValueExpressionList.Add(valueExpression);

        var isInteger = !value.Contains('.', StringComparison.Ordinal);
        var literalType = isInteger ? "Integer" : "Numeric";
        var literal = new Literal
        {
            Id = NextId(nameof(Literal)),
            ValueExpression = valueExpression,
            LiteralType = literalType,
            Value = value
        };
        model.LiteralList.Add(literal);

        string derivedEntityName;
        string derivedId;
        if (isInteger)
        {
            derivedEntityName = nameof(IntegerLiteral);
            var row = new IntegerLiteral
            {
                Id = NextId(nameof(IntegerLiteral)),
                Literal = literal,
                LiteralType = literalType
            };
            derivedId = row.Id;
            model.IntegerLiteralList.Add(row);
        }
        else
        {
            derivedEntityName = nameof(NumericLiteral);
            var row = new NumericLiteral
            {
                Id = NextId(nameof(NumericLiteral)),
                Literal = literal,
                LiteralType = literalType
            };
            derivedId = row.Id;
            model.NumericLiteralList.Add(row);
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ValueExpression), valueExpression.Id),
            (nameof(Literal), literal.Id),
            (derivedEntityName, derivedId));
    }

    public BuiltNode CreateRealLiteral(string value)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var valueExpression = new ValueExpression
        {
            Id = NextId(nameof(ValueExpression)),
            PrimaryExpression = primary
        };
        model.ValueExpressionList.Add(valueExpression);

        var literal = new Literal
        {
            Id = NextId(nameof(Literal)),
            ValueExpression = valueExpression,
            LiteralType = "Real",
            Value = value
        };
        model.LiteralList.Add(literal);

        var realLiteral = new RealLiteral
        {
            Id = NextId(nameof(RealLiteral)),
            Literal = literal,
            LiteralType = "Real"
        };
        model.RealLiteralList.Add(realLiteral);

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ValueExpression), valueExpression.Id),
            (nameof(Literal), literal.Id),
            (nameof(RealLiteral), realLiteral.Id));
    }

    public BuiltNode CreateBinaryLiteral(string value)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var valueExpression = new ValueExpression
        {
            Id = NextId(nameof(ValueExpression)),
            PrimaryExpression = primary
        };
        model.ValueExpressionList.Add(valueExpression);

        var literal = new Literal
        {
            Id = NextId(nameof(Literal)),
            ValueExpression = valueExpression,
            LiteralType = "Binary",
            Value = value
        };
        model.LiteralList.Add(literal);

        var binaryLiteral = new BinaryLiteral
        {
            Id = NextId(nameof(BinaryLiteral)),
            Literal = literal,
            LiteralType = "Binary"
        };
        model.BinaryLiteralList.Add(binaryLiteral);

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ValueExpression), valueExpression.Id),
            (nameof(Literal), literal.Id),
            (nameof(BinaryLiteral), binaryLiteral.Id));
    }

    public BuiltNode CreateNullLiteral()
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var valueExpression = new ValueExpression
        {
            Id = NextId(nameof(ValueExpression)),
            PrimaryExpression = primary
        };
        model.ValueExpressionList.Add(valueExpression);

        var literal = new Literal
        {
            Id = NextId(nameof(Literal)),
            ValueExpression = valueExpression,
            LiteralType = "Null",
            Value = string.Empty
        };
        model.LiteralList.Add(literal);

        var nullLiteral = new NullLiteral
        {
            Id = NextId(nameof(NullLiteral)),
            Literal = literal,
            LiteralType = "Null"
        };
        model.NullLiteralList.Add(nullLiteral);

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ValueExpression), valueExpression.Id),
            (nameof(Literal), literal.Id),
            (nameof(NullLiteral), nullLiteral.Id));
    }

    public BuiltNode CreateMaxLiteral()
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var valueExpression = new ValueExpression
        {
            Id = NextId(nameof(ValueExpression)),
            PrimaryExpression = primary
        };
        model.ValueExpressionList.Add(valueExpression);

        var literal = new Literal
        {
            Id = NextId(nameof(Literal)),
            ValueExpression = valueExpression,
            LiteralType = "Max",
            Value = "max"
        };
        model.LiteralList.Add(literal);

        var maxLiteral = new MaxLiteral
        {
            Id = NextId(nameof(MaxLiteral)),
            Literal = literal,
            LiteralType = "Max"
        };
        model.MaxLiteralList.Add(maxLiteral);

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ValueExpression), valueExpression.Id),
            (nameof(Literal), literal.Id),
            (nameof(MaxLiteral), maxLiteral.Id));
    }
}
