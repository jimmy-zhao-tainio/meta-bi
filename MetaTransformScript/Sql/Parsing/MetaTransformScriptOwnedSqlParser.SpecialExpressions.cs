using static MetaTransformScript.Sql.Parsing.MetaTransformScriptOwnedSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptOwnedSqlParser
{
    private sealed partial class Parser
    {
        private BuiltNode ParseFunctionLikeExpression(IReadOnlyList<MetaTransformScriptOwnedSqlToken> identifiers, BuiltNode? callTarget)
        {
            if (identifiers.Count == 0)
            {
                throw new InvalidOperationException("Function-like expression parsing requires at least one identifier.");
            }

            var functionNameToken = identifiers[^1];
            if (callTarget is null && identifiers.Count > 1)
            {
                callTarget = builder.CreateMultiPartIdentifierCallTarget(
                    builder.CreateMultiPartIdentifier(
                        identifiers.Take(identifiers.Count - 1)
                            .Select(token => builder.CreateIdentifier(token.Value, token.QuoteType))
                            .ToArray()));
            }

            var functionNameValue = functionNameToken.Value;

            if (callTarget is not null)
            {
                return ParseTrailingCollation(ParseGenericFunctionCall(functionNameToken, callTarget));
            }

            return functionNameValue.ToUpperInvariant() switch
            {
                "COALESCE" => ParseTrailingCollation(ParseCoalesceExpression()),
                "NULLIF" => ParseTrailingCollation(ParseNullIfExpression()),
                "IIF" => ParseTrailingCollation(ParseIIfCall()),
                "CAST" => ParseTrailingCollation(ParseCastCall()),
                "TRY_CAST" => ParseTrailingCollation(ParseTryCastCall()),
                "CONVERT" => ParseTrailingCollation(ParseConvertCall()),
                "TRY_CONVERT" => ParseTrailingCollation(ParseTryConvertCall()),
                _ => ParseTrailingCollation(ParseGenericFunctionCall(functionNameToken, callTarget: null))
            };
        }

        private BuiltNode ParseGenericFunctionCall(MetaTransformScriptOwnedSqlToken functionNameToken, BuiltNode? callTarget)
        {
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
            var parameters = new List<BuiltNode>();
            if (!Match(MetaTransformScriptOwnedSqlTokenKind.CloseParen))
            {
                if (Match(MetaTransformScriptOwnedSqlTokenKind.Star))
                {
                    parameters.Add(builder.CreateWildcardColumnReferenceExpression());
                    Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
                }
                else
                {
                    parameters.Add(ParseScalarExpression());
                    while (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
                    {
                        parameters.Add(ParseScalarExpression());
                    }

                    Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
                }
            }

            var functionName = builder.CreateIdentifier(functionNameToken.Value, functionNameToken.QuoteType);
            var functionCall = builder.CreateFunctionCall(functionName, parameters);
            if (callTarget is not null)
            {
                functionCall = builder.AttachFunctionCallCallTarget(functionCall, callTarget);
            }

            return PeekKeyword("OVER")
                ? builder.AttachOverClause(functionCall, ParseOverClause())
                : functionCall;
        }

        private BuiltNode ParseCoalesceExpression()
        {
            var arguments = ParseScalarArgumentList();
            return builder.CreateCoalesceExpression(arguments);
        }

        private BuiltNode ParseNullIfExpression()
        {
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
            var firstExpression = ParseScalarExpression();
            Expect(MetaTransformScriptOwnedSqlTokenKind.Comma);
            var secondExpression = ParseScalarExpression();
            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return builder.CreateNullIfExpression(firstExpression, secondExpression);
        }

        private BuiltNode ParseIIfCall()
        {
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
            var predicate = ParseBooleanExpression();
            Expect(MetaTransformScriptOwnedSqlTokenKind.Comma);
            var thenExpression = ParseScalarExpression();
            Expect(MetaTransformScriptOwnedSqlTokenKind.Comma);
            var elseExpression = ParseScalarExpression();
            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return builder.CreateIIfCall(predicate, thenExpression, elseExpression);
        }

        private BuiltNode ParseCastCall()
        {
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
            var parameter = ParseScalarExpression();
            ExpectKeyword("AS");
            var dataTypeReference = ParseDataTypeReference();
            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return builder.CreateCastCall(parameter, dataTypeReference);
        }

        private BuiltNode ParseTryCastCall()
        {
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
            var parameter = ParseScalarExpression();
            ExpectKeyword("AS");
            var dataTypeReference = ParseDataTypeReference();
            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return builder.CreateTryCastCall(parameter, dataTypeReference);
        }

        private BuiltNode ParseConvertCall()
        {
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
            var dataTypeReference = ParseDataTypeReference();
            Expect(MetaTransformScriptOwnedSqlTokenKind.Comma);
            var parameter = ParseScalarExpression();
            BuiltNode? style = null;
            if (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
            {
                style = ParseScalarExpression();
            }

            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return builder.CreateConvertCall(dataTypeReference, parameter, style);
        }

        private BuiltNode ParseTryConvertCall()
        {
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
            var dataTypeReference = ParseDataTypeReference();
            Expect(MetaTransformScriptOwnedSqlTokenKind.Comma);
            var parameter = ParseScalarExpression();
            BuiltNode? style = null;
            if (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
            {
                style = ParseScalarExpression();
            }

            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return builder.CreateTryConvertCall(dataTypeReference, parameter, style);
        }

        private BuiltNode ParseDataTypeReference()
        {
            var typeNameToken = Current;
            if (typeNameToken.Kind != MetaTransformScriptOwnedSqlTokenKind.Identifier)
            {
                throw ParseError($"Expected an identifier but found '{typeNameToken.Text}'.");
            }

            Advance();
            var mappedType = typeNameToken.Value.ToUpperInvariant() switch
            {
                "DECIMAL" => "Decimal",
                "INT" => "Int",
                "VARCHAR" => "VarChar",
                "DATETIME2" => "DateTime2",
                _ => throw Unsupported($"Data type '{typeNameToken.Value}' is not supported in the owned parser yet.")
            };

            List<BuiltNode>? parameters = null;
            if (Match(MetaTransformScriptOwnedSqlTokenKind.OpenParen))
            {
                parameters = new List<BuiltNode> { ParseDataTypeLiteralParameter() };
                while (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
                {
                    parameters.Add(ParseDataTypeLiteralParameter());
                }

                Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            }

            return builder.CreateSqlDataTypeReference(mappedType, parameters);
        }

        private BuiltNode ParseDataTypeLiteralParameter()
        {
            if (Current.Kind == MetaTransformScriptOwnedSqlTokenKind.NumberLiteral)
            {
                var token = Advance();
                return builder.CreateNumberLiteral(token.Value);
            }

            if (PeekKeyword("MAX"))
            {
                throw Unsupported("MAX data type parameters are not supported in the owned parser yet.");
            }

            throw ParseError($"Expected a data type parameter but found '{Current.Text}'.");
        }

        private List<BuiltNode> ParseScalarArgumentList()
        {
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
            var arguments = new List<BuiltNode> { ParseScalarExpression() };
            while (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
            {
                arguments.Add(ParseScalarExpression());
            }

            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return arguments;
        }

        private BuiltNode ParseTrailingCollation(BuiltNode expression)
        {
            while (MatchKeyword("COLLATE"))
            {
                expression = builder.AttachPrimaryExpressionCollation(expression, ParseIdentifier().Node);
            }

            return expression;
        }
    }
}
