using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    public BuiltNode CreateCastCall(BuiltNode parameter, BuiltNode dataTypeReference)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var castCall = new CastCall
        {
            Id = NextId(nameof(CastCall)),
            PrimaryExpressionId = primary.Id
        };
        model.CastCallList.Add(castCall);
        model.CastCallParameterLinkList.Add(new CastCallParameterLink
        {
            Id = NextId(nameof(CastCallParameterLink)),
            CastCallId = castCall.Id,
            ScalarExpressionId = parameter.GetId(nameof(ScalarExpression))
        });
        model.CastCallDataTypeLinkList.Add(new CastCallDataTypeLink
        {
            Id = NextId(nameof(CastCallDataTypeLink)),
            CastCallId = castCall.Id,
            DataTypeReferenceId = dataTypeReference.GetId(nameof(DataTypeReference))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(CastCall), castCall.Id));
    }

    public BuiltNode CreateTryCastCall(BuiltNode parameter, BuiltNode dataTypeReference)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var tryCastCall = new TryCastCall
        {
            Id = NextId(nameof(TryCastCall)),
            PrimaryExpressionId = primary.Id
        };
        model.TryCastCallList.Add(tryCastCall);
        model.TryCastCallParameterLinkList.Add(new TryCastCallParameterLink
        {
            Id = NextId(nameof(TryCastCallParameterLink)),
            TryCastCallId = tryCastCall.Id,
            ScalarExpressionId = parameter.GetId(nameof(ScalarExpression))
        });
        model.TryCastCallDataTypeLinkList.Add(new TryCastCallDataTypeLink
        {
            Id = NextId(nameof(TryCastCallDataTypeLink)),
            TryCastCallId = tryCastCall.Id,
            DataTypeReferenceId = dataTypeReference.GetId(nameof(DataTypeReference))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(TryCastCall), tryCastCall.Id));
    }

    public BuiltNode CreateConvertCall(BuiltNode dataTypeReference, BuiltNode parameter, BuiltNode? style = null)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var convertCall = new ConvertCall
        {
            Id = NextId(nameof(ConvertCall)),
            PrimaryExpressionId = primary.Id
        };
        model.ConvertCallList.Add(convertCall);
        model.ConvertCallDataTypeLinkList.Add(new ConvertCallDataTypeLink
        {
            Id = NextId(nameof(ConvertCallDataTypeLink)),
            ConvertCallId = convertCall.Id,
            DataTypeReferenceId = dataTypeReference.GetId(nameof(DataTypeReference))
        });
        model.ConvertCallParameterLinkList.Add(new ConvertCallParameterLink
        {
            Id = NextId(nameof(ConvertCallParameterLink)),
            ConvertCallId = convertCall.Id,
            ScalarExpressionId = parameter.GetId(nameof(ScalarExpression))
        });

        if (style is not null)
        {
            model.ConvertCallStyleLinkList.Add(new ConvertCallStyleLink
            {
                Id = NextId(nameof(ConvertCallStyleLink)),
                ConvertCallId = convertCall.Id,
                ScalarExpressionId = style.GetId(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ConvertCall), convertCall.Id));
    }

    public BuiltNode CreateTryConvertCall(BuiltNode dataTypeReference, BuiltNode parameter, BuiltNode? style = null)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var tryConvertCall = new TryConvertCall
        {
            Id = NextId(nameof(TryConvertCall)),
            PrimaryExpressionId = primary.Id
        };
        model.TryConvertCallList.Add(tryConvertCall);
        model.TryConvertCallDataTypeLinkList.Add(new TryConvertCallDataTypeLink
        {
            Id = NextId(nameof(TryConvertCallDataTypeLink)),
            TryConvertCallId = tryConvertCall.Id,
            DataTypeReferenceId = dataTypeReference.GetId(nameof(DataTypeReference))
        });
        model.TryConvertCallParameterLinkList.Add(new TryConvertCallParameterLink
        {
            Id = NextId(nameof(TryConvertCallParameterLink)),
            TryConvertCallId = tryConvertCall.Id,
            ScalarExpressionId = parameter.GetId(nameof(ScalarExpression))
        });

        if (style is not null)
        {
            model.TryConvertCallStyleLinkList.Add(new TryConvertCallStyleLink
            {
                Id = NextId(nameof(TryConvertCallStyleLink)),
                TryConvertCallId = tryConvertCall.Id,
                ScalarExpressionId = style.GetId(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(TryConvertCall), tryConvertCall.Id));
    }

    public BuiltNode CreateParseCall(BuiltNode stringValue, BuiltNode dataTypeReference, BuiltNode? culture = null)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var parseCall = new ParseCall
        {
            Id = NextId(nameof(ParseCall)),
            PrimaryExpressionId = primary.Id
        };
        model.ParseCallList.Add(parseCall);
        model.ParseCallStringValueLinkList.Add(new ParseCallStringValueLink
        {
            Id = NextId(nameof(ParseCallStringValueLink)),
            ParseCallId = parseCall.Id,
            ScalarExpressionId = stringValue.GetId(nameof(ScalarExpression))
        });
        model.ParseCallDataTypeLinkList.Add(new ParseCallDataTypeLink
        {
            Id = NextId(nameof(ParseCallDataTypeLink)),
            ParseCallId = parseCall.Id,
            DataTypeReferenceId = dataTypeReference.GetId(nameof(DataTypeReference))
        });

        if (culture is not null)
        {
            model.ParseCallCultureLinkList.Add(new ParseCallCultureLink
            {
                Id = NextId(nameof(ParseCallCultureLink)),
                ParseCallId = parseCall.Id,
                ScalarExpressionId = culture.GetId(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ParseCall), parseCall.Id));
    }

    public BuiltNode CreateTryParseCall(BuiltNode stringValue, BuiltNode dataTypeReference, BuiltNode? culture = null)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var tryParseCall = new TryParseCall
        {
            Id = NextId(nameof(TryParseCall)),
            PrimaryExpressionId = primary.Id
        };
        model.TryParseCallList.Add(tryParseCall);
        model.TryParseCallStringValueLinkList.Add(new TryParseCallStringValueLink
        {
            Id = NextId(nameof(TryParseCallStringValueLink)),
            TryParseCallId = tryParseCall.Id,
            ScalarExpressionId = stringValue.GetId(nameof(ScalarExpression))
        });
        model.TryParseCallDataTypeLinkList.Add(new TryParseCallDataTypeLink
        {
            Id = NextId(nameof(TryParseCallDataTypeLink)),
            TryParseCallId = tryParseCall.Id,
            DataTypeReferenceId = dataTypeReference.GetId(nameof(DataTypeReference))
        });

        if (culture is not null)
        {
            model.TryParseCallCultureLinkList.Add(new TryParseCallCultureLink
            {
                Id = NextId(nameof(TryParseCallCultureLink)),
                TryParseCallId = tryParseCall.Id,
                ScalarExpressionId = culture.GetId(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(TryParseCall), tryParseCall.Id));
    }

    public BuiltNode CreateAtTimeZoneCall(BuiltNode dateValue, BuiltNode timeZone)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var atTimeZoneCall = new AtTimeZoneCall
        {
            Id = NextId(nameof(AtTimeZoneCall)),
            PrimaryExpressionId = primary.Id
        };
        model.AtTimeZoneCallList.Add(atTimeZoneCall);
        model.AtTimeZoneCallDateValueLinkList.Add(new AtTimeZoneCallDateValueLink
        {
            Id = NextId(nameof(AtTimeZoneCallDateValueLink)),
            AtTimeZoneCallId = atTimeZoneCall.Id,
            ScalarExpressionId = dateValue.GetId(nameof(ScalarExpression))
        });
        model.AtTimeZoneCallTimeZoneLinkList.Add(new AtTimeZoneCallTimeZoneLink
        {
            Id = NextId(nameof(AtTimeZoneCallTimeZoneLink)),
            AtTimeZoneCallId = atTimeZoneCall.Id,
            ScalarExpressionId = timeZone.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(AtTimeZoneCall), atTimeZoneCall.Id));
    }

    public BuiltNode CreateNextValueForExpression(BuiltNode sequenceName)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var nextValueForExpression = new NextValueForExpression
        {
            Id = NextId(nameof(NextValueForExpression)),
            PrimaryExpressionId = primary.Id
        };
        model.NextValueForExpressionList.Add(nextValueForExpression);
        model.NextValueForExpressionSequenceNameLinkList.Add(new NextValueForExpressionSequenceNameLink
        {
            Id = NextId(nameof(NextValueForExpressionSequenceNameLink)),
            NextValueForExpressionId = nextValueForExpression.Id,
            SchemaObjectNameId = sequenceName.GetId(nameof(SchemaObjectName))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(NextValueForExpression), nextValueForExpression.Id));
    }

    public BuiltNode CreateParameterlessCall(string parameterlessCallType)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var parameterlessCall = new ParameterlessCall
        {
            Id = NextId(nameof(ParameterlessCall)),
            PrimaryExpressionId = primary.Id,
            ParameterlessCallType = parameterlessCallType
        };
        model.ParameterlessCallList.Add(parameterlessCall);

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ParameterlessCall), parameterlessCall.Id));
    }

    public BuiltNode CreateLeftFunctionCall(IReadOnlyList<BuiltNode> parameters)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var leftFunctionCall = new LeftFunctionCall
        {
            Id = NextId(nameof(LeftFunctionCall)),
            PrimaryExpressionId = primary.Id
        };
        model.LeftFunctionCallList.Add(leftFunctionCall);

        for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
        {
            model.LeftFunctionCallParametersItemList.Add(new LeftFunctionCallParametersItem
            {
                Id = NextId(nameof(LeftFunctionCallParametersItem)),
                LeftFunctionCallId = leftFunctionCall.Id,
                ScalarExpressionId = parameters[ordinal].GetId(nameof(ScalarExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(LeftFunctionCall), leftFunctionCall.Id));
    }

    public BuiltNode CreateRightFunctionCall(IReadOnlyList<BuiltNode> parameters)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var rightFunctionCall = new RightFunctionCall
        {
            Id = NextId(nameof(RightFunctionCall)),
            PrimaryExpressionId = primary.Id
        };
        model.RightFunctionCallList.Add(rightFunctionCall);

        for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
        {
            model.RightFunctionCallParametersItemList.Add(new RightFunctionCallParametersItem
            {
                Id = NextId(nameof(RightFunctionCallParametersItem)),
                RightFunctionCallId = rightFunctionCall.Id,
                ScalarExpressionId = parameters[ordinal].GetId(nameof(ScalarExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(RightFunctionCall), rightFunctionCall.Id));
    }

    public BuiltNode CreateGlobalVariableExpression(string name)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var valueExpression = new ValueExpression
        {
            Id = NextId(nameof(ValueExpression)),
            PrimaryExpressionId = primary.Id
        };
        model.ValueExpressionList.Add(valueExpression);

        var globalVariableExpression = new GlobalVariableExpression
        {
            Id = NextId(nameof(GlobalVariableExpression)),
            ValueExpressionId = valueExpression.Id,
            Name = name
        };
        model.GlobalVariableExpressionList.Add(globalVariableExpression);

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ValueExpression), valueExpression.Id),
            (nameof(GlobalVariableExpression), globalVariableExpression.Id));
    }

    public BuiltNode CreateUnaryExpression(BuiltNode expression, string unaryExpressionType)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var unaryExpression = new UnaryExpression
        {
            Id = NextId(nameof(UnaryExpression)),
            ScalarExpressionId = scalar.Id,
            UnaryExpressionType = unaryExpressionType
        };
        model.UnaryExpressionList.Add(unaryExpression);
        model.UnaryExpressionExpressionLinkList.Add(new UnaryExpressionExpressionLink
        {
            Id = NextId(nameof(UnaryExpressionExpressionLink)),
            UnaryExpressionId = unaryExpression.Id,
            ScalarExpressionId = expression.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(UnaryExpression), unaryExpression.Id));
    }

    public BuiltNode CreateSqlDataTypeReference(string sqlDataTypeOption, IReadOnlyList<BuiltNode>? parameters = null)
    {
        var typeIdentifier = CreateIdentifier(RenderSqlDataTypeIdentifierValue(sqlDataTypeOption), "NotQuoted");
        var schemaObjectName = CreateSchemaObjectName([typeIdentifier]);

        var dataTypeReference = new DataTypeReference
        {
            Id = NextId(nameof(DataTypeReference))
        };
        model.DataTypeReferenceList.Add(dataTypeReference);
        model.DataTypeReferenceNameLinkList.Add(new DataTypeReferenceNameLink
        {
            Id = NextId(nameof(DataTypeReferenceNameLink)),
            DataTypeReferenceId = dataTypeReference.Id,
            SchemaObjectNameId = schemaObjectName.GetId(nameof(SchemaObjectName))
        });

        var parameterizedDataTypeReference = new ParameterizedDataTypeReference
        {
            Id = NextId(nameof(ParameterizedDataTypeReference)),
            DataTypeReferenceId = dataTypeReference.Id
        };
        model.ParameterizedDataTypeReferenceList.Add(parameterizedDataTypeReference);

        var sqlDataTypeReference = new SqlDataTypeReference
        {
            Id = NextId(nameof(SqlDataTypeReference)),
            ParameterizedDataTypeReferenceId = parameterizedDataTypeReference.Id,
            SqlDataTypeOption = sqlDataTypeOption
        };
        model.SqlDataTypeReferenceList.Add(sqlDataTypeReference);

        if (parameters is not null)
        {
            for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
            {
                model.ParameterizedDataTypeReferenceParametersItemList.Add(new ParameterizedDataTypeReferenceParametersItem
                {
                    Id = NextId(nameof(ParameterizedDataTypeReferenceParametersItem)),
                    ParameterizedDataTypeReferenceId = parameterizedDataTypeReference.Id,
                    LiteralId = parameters[ordinal].GetId(nameof(Literal)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        return BuiltNode.Create(
            (nameof(DataTypeReference), dataTypeReference.Id),
            (nameof(ParameterizedDataTypeReference), parameterizedDataTypeReference.Id),
            (nameof(SqlDataTypeReference), sqlDataTypeReference.Id));
    }

    public BuiltNode AttachPrimaryExpressionCollation(BuiltNode primaryExpressionNode, BuiltNode collationIdentifier)
    {
        model.PrimaryExpressionCollationLinkList.Add(new PrimaryExpressionCollationLink
        {
            Id = NextId(nameof(PrimaryExpressionCollationLink)),
            PrimaryExpressionId = primaryExpressionNode.GetId(nameof(PrimaryExpression)),
            IdentifierId = collationIdentifier.GetId(nameof(Identifier))
        });

        return primaryExpressionNode;
    }

        private static string RenderSqlDataTypeIdentifierValue(string sqlDataTypeOption) =>
            MetaTransformScript.Sql.MetaTransformScriptSqlServerDataTypes.RenderSqlName(sqlDataTypeOption);
}
