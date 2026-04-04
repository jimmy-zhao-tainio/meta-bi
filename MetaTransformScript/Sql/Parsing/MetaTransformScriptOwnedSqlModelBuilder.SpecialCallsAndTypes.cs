using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptOwnedSqlModelBuilder
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

    public BuiltNode CreateSqlDataTypeReference(string sqlDataTypeOption, IReadOnlyList<BuiltNode>? parameters = null)
    {
        var typeIdentifier = CreateIdentifier(RenderSqlDataTypeIdentifierValue(sqlDataTypeOption), "NotQuoted");
        var schemaObjectName = CreateSchemaObjectName(null, typeIdentifier);

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
        sqlDataTypeOption switch
        {
            "Decimal" => "decimal",
            "Int" => "int",
            "VarChar" => "varchar",
            "DateTime2" => "datetime2",
            _ => throw new InvalidOperationException($"Unsupported SqlDataTypeOption '{sqlDataTypeOption}'.")
        };
}
