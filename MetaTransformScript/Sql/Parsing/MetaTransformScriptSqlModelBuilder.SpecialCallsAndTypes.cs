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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var castCall = new CastCall
        {
            Id = NextId(nameof(CastCall)),
            BaseId = primary.Id
        };
        model.CastCallList.Add(castCall);
        model.CastCallParameterLinkList.Add(new CastCallParameterLink
        {
            Id = NextId(nameof(CastCallParameterLink)),
            OwnerId = castCall.Id,
            ValueId = parameter.GetId(nameof(ScalarExpression))
        });
        model.CastCallDataTypeLinkList.Add(new CastCallDataTypeLink
        {
            Id = NextId(nameof(CastCallDataTypeLink)),
            OwnerId = castCall.Id,
            ValueId = dataTypeReference.GetId(nameof(DataTypeReference))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var tryCastCall = new TryCastCall
        {
            Id = NextId(nameof(TryCastCall)),
            BaseId = primary.Id
        };
        model.TryCastCallList.Add(tryCastCall);
        model.TryCastCallParameterLinkList.Add(new TryCastCallParameterLink
        {
            Id = NextId(nameof(TryCastCallParameterLink)),
            OwnerId = tryCastCall.Id,
            ValueId = parameter.GetId(nameof(ScalarExpression))
        });
        model.TryCastCallDataTypeLinkList.Add(new TryCastCallDataTypeLink
        {
            Id = NextId(nameof(TryCastCallDataTypeLink)),
            OwnerId = tryCastCall.Id,
            ValueId = dataTypeReference.GetId(nameof(DataTypeReference))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var convertCall = new ConvertCall
        {
            Id = NextId(nameof(ConvertCall)),
            BaseId = primary.Id
        };
        model.ConvertCallList.Add(convertCall);
        model.ConvertCallDataTypeLinkList.Add(new ConvertCallDataTypeLink
        {
            Id = NextId(nameof(ConvertCallDataTypeLink)),
            OwnerId = convertCall.Id,
            ValueId = dataTypeReference.GetId(nameof(DataTypeReference))
        });
        model.ConvertCallParameterLinkList.Add(new ConvertCallParameterLink
        {
            Id = NextId(nameof(ConvertCallParameterLink)),
            OwnerId = convertCall.Id,
            ValueId = parameter.GetId(nameof(ScalarExpression))
        });

        if (style is not null)
        {
            model.ConvertCallStyleLinkList.Add(new ConvertCallStyleLink
            {
                Id = NextId(nameof(ConvertCallStyleLink)),
                OwnerId = convertCall.Id,
                ValueId = style.GetId(nameof(ScalarExpression))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var tryConvertCall = new TryConvertCall
        {
            Id = NextId(nameof(TryConvertCall)),
            BaseId = primary.Id
        };
        model.TryConvertCallList.Add(tryConvertCall);
        model.TryConvertCallDataTypeLinkList.Add(new TryConvertCallDataTypeLink
        {
            Id = NextId(nameof(TryConvertCallDataTypeLink)),
            OwnerId = tryConvertCall.Id,
            ValueId = dataTypeReference.GetId(nameof(DataTypeReference))
        });
        model.TryConvertCallParameterLinkList.Add(new TryConvertCallParameterLink
        {
            Id = NextId(nameof(TryConvertCallParameterLink)),
            OwnerId = tryConvertCall.Id,
            ValueId = parameter.GetId(nameof(ScalarExpression))
        });

        if (style is not null)
        {
            model.TryConvertCallStyleLinkList.Add(new TryConvertCallStyleLink
            {
                Id = NextId(nameof(TryConvertCallStyleLink)),
                OwnerId = tryConvertCall.Id,
                ValueId = style.GetId(nameof(ScalarExpression))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var parseCall = new ParseCall
        {
            Id = NextId(nameof(ParseCall)),
            BaseId = primary.Id
        };
        model.ParseCallList.Add(parseCall);
        model.ParseCallStringValueLinkList.Add(new ParseCallStringValueLink
        {
            Id = NextId(nameof(ParseCallStringValueLink)),
            OwnerId = parseCall.Id,
            ValueId = stringValue.GetId(nameof(ScalarExpression))
        });
        model.ParseCallDataTypeLinkList.Add(new ParseCallDataTypeLink
        {
            Id = NextId(nameof(ParseCallDataTypeLink)),
            OwnerId = parseCall.Id,
            ValueId = dataTypeReference.GetId(nameof(DataTypeReference))
        });

        if (culture is not null)
        {
            model.ParseCallCultureLinkList.Add(new ParseCallCultureLink
            {
                Id = NextId(nameof(ParseCallCultureLink)),
                OwnerId = parseCall.Id,
                ValueId = culture.GetId(nameof(ScalarExpression))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var tryParseCall = new TryParseCall
        {
            Id = NextId(nameof(TryParseCall)),
            BaseId = primary.Id
        };
        model.TryParseCallList.Add(tryParseCall);
        model.TryParseCallStringValueLinkList.Add(new TryParseCallStringValueLink
        {
            Id = NextId(nameof(TryParseCallStringValueLink)),
            OwnerId = tryParseCall.Id,
            ValueId = stringValue.GetId(nameof(ScalarExpression))
        });
        model.TryParseCallDataTypeLinkList.Add(new TryParseCallDataTypeLink
        {
            Id = NextId(nameof(TryParseCallDataTypeLink)),
            OwnerId = tryParseCall.Id,
            ValueId = dataTypeReference.GetId(nameof(DataTypeReference))
        });

        if (culture is not null)
        {
            model.TryParseCallCultureLinkList.Add(new TryParseCallCultureLink
            {
                Id = NextId(nameof(TryParseCallCultureLink)),
                OwnerId = tryParseCall.Id,
                ValueId = culture.GetId(nameof(ScalarExpression))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var atTimeZoneCall = new AtTimeZoneCall
        {
            Id = NextId(nameof(AtTimeZoneCall)),
            BaseId = primary.Id
        };
        model.AtTimeZoneCallList.Add(atTimeZoneCall);
        model.AtTimeZoneCallDateValueLinkList.Add(new AtTimeZoneCallDateValueLink
        {
            Id = NextId(nameof(AtTimeZoneCallDateValueLink)),
            OwnerId = atTimeZoneCall.Id,
            ValueId = dateValue.GetId(nameof(ScalarExpression))
        });
        model.AtTimeZoneCallTimeZoneLinkList.Add(new AtTimeZoneCallTimeZoneLink
        {
            Id = NextId(nameof(AtTimeZoneCallTimeZoneLink)),
            OwnerId = atTimeZoneCall.Id,
            ValueId = timeZone.GetId(nameof(ScalarExpression))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var nextValueForExpression = new NextValueForExpression
        {
            Id = NextId(nameof(NextValueForExpression)),
            BaseId = primary.Id
        };
        model.NextValueForExpressionList.Add(nextValueForExpression);
        model.NextValueForExpressionSequenceNameLinkList.Add(new NextValueForExpressionSequenceNameLink
        {
            Id = NextId(nameof(NextValueForExpressionSequenceNameLink)),
            OwnerId = nextValueForExpression.Id,
            ValueId = sequenceName.GetId(nameof(SchemaObjectName))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var parameterlessCall = new ParameterlessCall
        {
            Id = NextId(nameof(ParameterlessCall)),
            BaseId = primary.Id,
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var leftFunctionCall = new LeftFunctionCall
        {
            Id = NextId(nameof(LeftFunctionCall)),
            BaseId = primary.Id
        };
        model.LeftFunctionCallList.Add(leftFunctionCall);

        for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
        {
            model.LeftFunctionCallParametersItemList.Add(new LeftFunctionCallParametersItem
            {
                Id = NextId(nameof(LeftFunctionCallParametersItem)),
                OwnerId = leftFunctionCall.Id,
                ValueId = parameters[ordinal].GetId(nameof(ScalarExpression)),
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var rightFunctionCall = new RightFunctionCall
        {
            Id = NextId(nameof(RightFunctionCall)),
            BaseId = primary.Id
        };
        model.RightFunctionCallList.Add(rightFunctionCall);

        for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
        {
            model.RightFunctionCallParametersItemList.Add(new RightFunctionCallParametersItem
            {
                Id = NextId(nameof(RightFunctionCallParametersItem)),
                OwnerId = rightFunctionCall.Id,
                ValueId = parameters[ordinal].GetId(nameof(ScalarExpression)),
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var valueExpression = new ValueExpression
        {
            Id = NextId(nameof(ValueExpression)),
            BaseId = primary.Id
        };
        model.ValueExpressionList.Add(valueExpression);

        var globalVariableExpression = new GlobalVariableExpression
        {
            Id = NextId(nameof(GlobalVariableExpression)),
            BaseId = valueExpression.Id,
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
            BaseId = scalar.Id,
            UnaryExpressionType = unaryExpressionType
        };
        model.UnaryExpressionList.Add(unaryExpression);
        model.UnaryExpressionExpressionLinkList.Add(new UnaryExpressionExpressionLink
        {
            Id = NextId(nameof(UnaryExpressionExpressionLink)),
            OwnerId = unaryExpression.Id,
            ValueId = expression.GetId(nameof(ScalarExpression))
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
            OwnerId = dataTypeReference.Id,
            ValueId = schemaObjectName.GetId(nameof(SchemaObjectName))
        });

        var parameterizedDataTypeReference = new ParameterizedDataTypeReference
        {
            Id = NextId(nameof(ParameterizedDataTypeReference)),
            BaseId = dataTypeReference.Id
        };
        model.ParameterizedDataTypeReferenceList.Add(parameterizedDataTypeReference);

        var sqlDataTypeReference = new SqlDataTypeReference
        {
            Id = NextId(nameof(SqlDataTypeReference)),
            BaseId = parameterizedDataTypeReference.Id,
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
                    OwnerId = parameterizedDataTypeReference.Id,
                    ValueId = parameters[ordinal].GetId(nameof(Literal)),
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
            OwnerId = primaryExpressionNode.GetId(nameof(PrimaryExpression)),
            ValueId = collationIdentifier.GetId(nameof(Identifier))
        });

        return primaryExpressionNode;
    }

        private static string RenderSqlDataTypeIdentifierValue(string sqlDataTypeOption) =>
            MetaTransformScript.Sql.MetaTransformScriptSqlServerDataTypes.RenderSqlName(sqlDataTypeOption);
}
