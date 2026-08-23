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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var castCall = new CastCall
        {
            Id = NextId(nameof(CastCall)),
            PrimaryExpression = primary
        };
        model.CastCallList.Add(castCall);
        model.CastCallParameterLinkList.Add(new CastCallParameterLink
        {
            Id = NextId(nameof(CastCallParameterLink)),
            CastCall = castCall,
            ScalarExpression = parameter.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });
        model.CastCallDataTypeLinkList.Add(new CastCallDataTypeLink
        {
            Id = NextId(nameof(CastCallDataTypeLink)),
            CastCall = castCall,
            DataTypeReference = dataTypeReference.GetRef<DataTypeReference>(nameof(DataTypeReference))
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var tryCastCall = new TryCastCall
        {
            Id = NextId(nameof(TryCastCall)),
            PrimaryExpression = primary
        };
        model.TryCastCallList.Add(tryCastCall);
        model.TryCastCallParameterLinkList.Add(new TryCastCallParameterLink
        {
            Id = NextId(nameof(TryCastCallParameterLink)),
            TryCastCall = tryCastCall,
            ScalarExpression = parameter.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });
        model.TryCastCallDataTypeLinkList.Add(new TryCastCallDataTypeLink
        {
            Id = NextId(nameof(TryCastCallDataTypeLink)),
            TryCastCall = tryCastCall,
            DataTypeReference = dataTypeReference.GetRef<DataTypeReference>(nameof(DataTypeReference))
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var convertCall = new ConvertCall
        {
            Id = NextId(nameof(ConvertCall)),
            PrimaryExpression = primary
        };
        model.ConvertCallList.Add(convertCall);
        model.ConvertCallDataTypeLinkList.Add(new ConvertCallDataTypeLink
        {
            Id = NextId(nameof(ConvertCallDataTypeLink)),
            ConvertCall = convertCall,
            DataTypeReference = dataTypeReference.GetRef<DataTypeReference>(nameof(DataTypeReference))
        });
        model.ConvertCallParameterLinkList.Add(new ConvertCallParameterLink
        {
            Id = NextId(nameof(ConvertCallParameterLink)),
            ConvertCall = convertCall,
            ScalarExpression = parameter.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });

        if (style is not null)
        {
            model.ConvertCallStyleLinkList.Add(new ConvertCallStyleLink
            {
                Id = NextId(nameof(ConvertCallStyleLink)),
                ConvertCall = convertCall,
                ScalarExpression = style.GetRef<ScalarExpression>(nameof(ScalarExpression))
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var tryConvertCall = new TryConvertCall
        {
            Id = NextId(nameof(TryConvertCall)),
            PrimaryExpression = primary
        };
        model.TryConvertCallList.Add(tryConvertCall);
        model.TryConvertCallDataTypeLinkList.Add(new TryConvertCallDataTypeLink
        {
            Id = NextId(nameof(TryConvertCallDataTypeLink)),
            TryConvertCall = tryConvertCall,
            DataTypeReference = dataTypeReference.GetRef<DataTypeReference>(nameof(DataTypeReference))
        });
        model.TryConvertCallParameterLinkList.Add(new TryConvertCallParameterLink
        {
            Id = NextId(nameof(TryConvertCallParameterLink)),
            TryConvertCall = tryConvertCall,
            ScalarExpression = parameter.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });

        if (style is not null)
        {
            model.TryConvertCallStyleLinkList.Add(new TryConvertCallStyleLink
            {
                Id = NextId(nameof(TryConvertCallStyleLink)),
                TryConvertCall = tryConvertCall,
                ScalarExpression = style.GetRef<ScalarExpression>(nameof(ScalarExpression))
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var parseCall = new ParseCall
        {
            Id = NextId(nameof(ParseCall)),
            PrimaryExpression = primary
        };
        model.ParseCallList.Add(parseCall);
        model.ParseCallStringValueLinkList.Add(new ParseCallStringValueLink
        {
            Id = NextId(nameof(ParseCallStringValueLink)),
            ParseCall = parseCall,
            ScalarExpression = stringValue.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });
        model.ParseCallDataTypeLinkList.Add(new ParseCallDataTypeLink
        {
            Id = NextId(nameof(ParseCallDataTypeLink)),
            ParseCall = parseCall,
            DataTypeReference = dataTypeReference.GetRef<DataTypeReference>(nameof(DataTypeReference))
        });

        if (culture is not null)
        {
            model.ParseCallCultureLinkList.Add(new ParseCallCultureLink
            {
                Id = NextId(nameof(ParseCallCultureLink)),
                ParseCall = parseCall,
                ScalarExpression = culture.GetRef<ScalarExpression>(nameof(ScalarExpression))
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var tryParseCall = new TryParseCall
        {
            Id = NextId(nameof(TryParseCall)),
            PrimaryExpression = primary
        };
        model.TryParseCallList.Add(tryParseCall);
        model.TryParseCallStringValueLinkList.Add(new TryParseCallStringValueLink
        {
            Id = NextId(nameof(TryParseCallStringValueLink)),
            TryParseCall = tryParseCall,
            ScalarExpression = stringValue.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });
        model.TryParseCallDataTypeLinkList.Add(new TryParseCallDataTypeLink
        {
            Id = NextId(nameof(TryParseCallDataTypeLink)),
            TryParseCall = tryParseCall,
            DataTypeReference = dataTypeReference.GetRef<DataTypeReference>(nameof(DataTypeReference))
        });

        if (culture is not null)
        {
            model.TryParseCallCultureLinkList.Add(new TryParseCallCultureLink
            {
                Id = NextId(nameof(TryParseCallCultureLink)),
                TryParseCall = tryParseCall,
                ScalarExpression = culture.GetRef<ScalarExpression>(nameof(ScalarExpression))
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var atTimeZoneCall = new AtTimeZoneCall
        {
            Id = NextId(nameof(AtTimeZoneCall)),
            PrimaryExpression = primary
        };
        model.AtTimeZoneCallList.Add(atTimeZoneCall);
        model.AtTimeZoneCallDateValueLinkList.Add(new AtTimeZoneCallDateValueLink
        {
            Id = NextId(nameof(AtTimeZoneCallDateValueLink)),
            AtTimeZoneCall = atTimeZoneCall,
            ScalarExpression = dateValue.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });
        model.AtTimeZoneCallTimeZoneLinkList.Add(new AtTimeZoneCallTimeZoneLink
        {
            Id = NextId(nameof(AtTimeZoneCallTimeZoneLink)),
            AtTimeZoneCall = atTimeZoneCall,
            ScalarExpression = timeZone.GetRef<ScalarExpression>(nameof(ScalarExpression))
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var nextValueForExpression = new NextValueForExpression
        {
            Id = NextId(nameof(NextValueForExpression)),
            PrimaryExpression = primary
        };
        model.NextValueForExpressionList.Add(nextValueForExpression);
        model.NextValueForExpressionSequenceNameLinkList.Add(new NextValueForExpressionSequenceNameLink
        {
            Id = NextId(nameof(NextValueForExpressionSequenceNameLink)),
            NextValueForExpression = nextValueForExpression,
            SchemaObjectName = sequenceName.GetRef<SchemaObjectName>(nameof(SchemaObjectName))
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var parameterlessCall = new ParameterlessCall
        {
            Id = NextId(nameof(ParameterlessCall)),
            PrimaryExpression = primary,
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var leftFunctionCall = new LeftFunctionCall
        {
            Id = NextId(nameof(LeftFunctionCall)),
            PrimaryExpression = primary
        };
        model.LeftFunctionCallList.Add(leftFunctionCall);

        for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
        {
            model.LeftFunctionCallParametersItemList.Add(new LeftFunctionCallParametersItem
            {
                Id = NextId(nameof(LeftFunctionCallParametersItem)),
                LeftFunctionCall = leftFunctionCall,
                ScalarExpression = parameters[ordinal].GetRef<ScalarExpression>(nameof(ScalarExpression)),
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var rightFunctionCall = new RightFunctionCall
        {
            Id = NextId(nameof(RightFunctionCall)),
            PrimaryExpression = primary
        };
        model.RightFunctionCallList.Add(rightFunctionCall);

        for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
        {
            model.RightFunctionCallParametersItemList.Add(new RightFunctionCallParametersItem
            {
                Id = NextId(nameof(RightFunctionCallParametersItem)),
                RightFunctionCall = rightFunctionCall,
                ScalarExpression = parameters[ordinal].GetRef<ScalarExpression>(nameof(ScalarExpression)),
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var valueExpression = new ValueExpression
        {
            Id = NextId(nameof(ValueExpression)),
            PrimaryExpression = primary
        };
        model.ValueExpressionList.Add(valueExpression);

        var globalVariableExpression = new GlobalVariableExpression
        {
            Id = NextId(nameof(GlobalVariableExpression)),
            ValueExpression = valueExpression,
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
            ScalarExpression = scalar,
            UnaryExpressionType = unaryExpressionType
        };
        model.UnaryExpressionList.Add(unaryExpression);
        model.UnaryExpressionExpressionLinkList.Add(new UnaryExpressionExpressionLink
        {
            Id = NextId(nameof(UnaryExpressionExpressionLink)),
            UnaryExpression = unaryExpression,
            ScalarExpression = expression.GetRef<ScalarExpression>(nameof(ScalarExpression))
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
            DataTypeReference = dataTypeReference,
            SchemaObjectName = schemaObjectName.GetRef<SchemaObjectName>(nameof(SchemaObjectName))
        });

        var parameterizedDataTypeReference = new ParameterizedDataTypeReference
        {
            Id = NextId(nameof(ParameterizedDataTypeReference)),
            DataTypeReference = dataTypeReference
        };
        model.ParameterizedDataTypeReferenceList.Add(parameterizedDataTypeReference);

        var sqlDataTypeReference = new SqlDataTypeReference
        {
            Id = NextId(nameof(SqlDataTypeReference)),
            ParameterizedDataTypeReference = parameterizedDataTypeReference,
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
                    ParameterizedDataTypeReference = parameterizedDataTypeReference,
                    Literal = parameters[ordinal].GetRef<Literal>(nameof(Literal)),
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
            PrimaryExpression = primaryExpressionNode.GetRef<PrimaryExpression>(nameof(PrimaryExpression)),
            Identifier = collationIdentifier.GetRef<Identifier>(nameof(Identifier))
        });

        return primaryExpressionNode;
    }

        private static string RenderSqlDataTypeIdentifierValue(string sqlDataTypeOption) =>
            MetaTransformScript.Sql.MetaTransformScriptSqlServerDataTypes.RenderSqlName(sqlDataTypeOption);
}
